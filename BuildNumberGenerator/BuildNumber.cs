 namespace BuildNumberGenerator
 {
    public class BuildNumber
    {
        public BuildNumber(string id, string branch, DateTime buildTime)
        {
            Id = id;
            Branch = branch;
            Number = 1;
            BuildTime = buildTime;
        }

        public string Id {get; private set;}
        public string Branch {get; private set;}
        public int Number {get; set;}
        public DateTime BuildTime {get;set;}  // Always UTC
    }
 }