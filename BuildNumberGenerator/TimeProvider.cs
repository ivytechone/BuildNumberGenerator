namespace BuildNumberGenerator
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime CurrentTimeUTC
        {
            get
            {
                return DateTime.UtcNow;
            }
        }
    }
}