using BuildNumberGenerator.Interfaces;

namespace BuildNumberGenerator
{
    public class Generator : IGenerator
    {
        public Generator()
        {
        }

        public string GetNextBuildNumber(string id, string branch)
        {
            return "Hello World";
        }
    }
}