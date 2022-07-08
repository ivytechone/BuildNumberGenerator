namespace BuildNumberGenerator
{
    public interface ITimeProvider
    {
        DateTime CurrentTimeUTC{ get; }
    }
}