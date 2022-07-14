namespace BuildNumberGenerator
{
    class BuildNumber
    {
        public BuildNumber(string id, DateTime buildTime)
        {
            Id = id;
            Number = 1;
            BuildTime = buildTime;
        }

        public int Number {get; set;}
        public string Id {get; private set;}
        public DateTime BuildTime {get;set;}  // Always UTC
    }

    public class Generator : IGenerator
    {
        private readonly ITimeProvider _timeProvider;
        private Dictionary<string, BuildNumber> _builds;
        private Queue<BuildNumber> _buildQueue;
        private readonly object _sync;

        public Generator(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
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
                    buildNumber = new BuildNumber(buildKey, currentTimeUTC);
                    _builds.Add(buildKey, buildNumber);  
                    _buildQueue.Enqueue(buildNumber);    
                }
                else
                {
                    buildNumber.Number++;
                }
            }

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