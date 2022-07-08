namespace BuildNumberGenerator
{
    class BuildNumber
    {
        public BuildNumber(DateTime resetAt)
        {
            Number = 1;
            ResetAt = resetAt;
        }

        public int Number {get; set;}
        public DateTime ResetAt {get;set;}  // Always UTC
    }

    public class Generator : IGenerator
    {
        private readonly ITimeProvider _timeProvider;

        public Generator(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
            builds = new Dictionary<string, BuildNumber>();
        }

        private Dictionary<string, BuildNumber> builds;

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

            BuildNumber? buildNum;

            if (!builds.TryGetValue(buildKey, out buildNum))
            {
                buildNum = new BuildNumber(currentTimeUTC.AddDays(1)); // purge from database after 24 hours
                builds.Add(buildKey, buildNum);           
            }
            else
            {
                buildNum.Number++;
            }

            return $"{buildKey.Substring(buildKey.IndexOf('.')+1)}.{buildNum.Number}";
        }
    }
}