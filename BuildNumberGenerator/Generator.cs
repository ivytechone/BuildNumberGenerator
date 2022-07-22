namespace BuildNumberGenerator
{
    public class Generator : IGenerator
    {
        private readonly ITimeProvider _timeProvider;
        private readonly Dictionary<string, BuildNumber> _builds;
        private readonly Queue<BuildNumber> _buildQueue;
        private readonly IBuildNumberArchiver? _archiver;
        private readonly object _sync;

        public Generator(ITimeProvider timeProvider, IBuildNumberArchiver? archiver)
        {
            _timeProvider = timeProvider;
            _archiver = archiver;
            _builds = new Dictionary<string, BuildNumber>();
            _buildQueue = new Queue<BuildNumber>();
            _sync = new Object();
        }

        public string GetNextBuildNumber(string id, string branch, TimeZoneInfo timeZone)
        {
            if (id is null || id.Contains('.'))
            {
                throw new ArgumentException("id must provided and must not contain '.'");
            }

            if (branch is null || branch.Contains('.'))
            {
                throw new ArgumentException("branch must be provided and must not contain '.'");
            }

            if (timeZone is null)
            {
                throw new ArgumentNullException("timezone");
            }

            var currentTimeUTC = _timeProvider.CurrentTimeUTC;
            var userLocalTime = TimeZoneInfo.ConvertTime(currentTimeUTC, timeZone);

            var buildKey = $"{id}.{branch}.{userLocalTime.ToString("yyyyMMdd")}";

            BuildNumber? buildNumber;

            lock(_sync)
            {
                if (!_builds.TryGetValue(buildKey, out buildNumber))
                {
                    buildNumber = new BuildNumber(buildKey, branch, currentTimeUTC);
                    _builds.Add(buildKey, buildNumber);  
                    _buildQueue.Enqueue(buildNumber);    
                }
                else
                {
                    buildNumber.Number++;
                }
            }
            _archiver?.Queue(buildNumber);
            return $"{buildKey.Substring(buildKey.IndexOf('.')+1)}.{buildNumber.Number}";
        }

        public int PurgeBuilds()
        {
            var currentTimeUTC = _timeProvider.CurrentTimeUTC;
            int count = 0;
            while(true)
            {
                lock(_sync)
                {
                    _buildQueue.TryPeek(out BuildNumber? build);

                    if (build is not null && build.BuildTime.AddDays(1) < currentTimeUTC)
                    {
                        _buildQueue.Dequeue();
                        if (!_builds.Remove(build.Id))
                        {
                            throw new Exception("build not found in dictionary");
                        }
                        count++;
                    }
                    else
                    {
                        return count;
                    }
                }
            }
        }
    }
}