namespace BuildNumberGenerator 
{
    public interface IBuildNumberArchiver
    {
        void Queue(BuildNumber buildNumber);
        Task StopAndWaitForFlush();
    }
}