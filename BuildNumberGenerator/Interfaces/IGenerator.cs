namespace BuildNumberGenerator
{
    public interface IGenerator
    {
        string GetNextBuildNumber(string id, string branch, TimeZoneInfo tz);
    }
}