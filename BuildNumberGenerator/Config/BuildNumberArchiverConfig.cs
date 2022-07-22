namespace BuildNumberGenerator
{
    public class BuildNumberArchiverConfig : IBuildNumberArchiverConfig
    {
        public int SendCount {get; set;}
        public int SendDelay {get; set;}
        public string? ElasticSearchURL {get; set;}
        public string? ApiKey {get; set;}
    }
}