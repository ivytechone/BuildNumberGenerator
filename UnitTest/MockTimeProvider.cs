using System.Globalization;
using BuildNumberGenerator;

public class MockTimeProvider : ITimeProvider
{
    public MockTimeProvider(string startTime)
    {
        SetMockTimeUTC(startTime);
    }

    public DateTime CurrentTimeUTC 
    {
        get
        {
            return _currentMockTime;
        }
    }

    public void SetMockTimeUTC(string time)
    {
        _currentMockTime = DateTime.ParseExact(time,
                                       "yyyy-MM-dd'T'HH:mm'Z'",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.AssumeUniversal);
    }

    public void SetMockTimeUTC(DateTime time)
    {
        if (time.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("time must be utc");
        }
        _currentMockTime = time;
    }

    private DateTime _currentMockTime;
}