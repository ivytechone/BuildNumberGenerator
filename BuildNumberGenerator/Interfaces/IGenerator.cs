namespace BuildNumberGenerator.Interfaces
{
    public interface IGenerator
    {
        string GetNextBuildNumber(string id, string branch);
    }
}