using Xunit;
using BuildNumberGenerator;
using Xunit.Abstractions;

public class GeneratorTests
{
    private readonly ITestOutputHelper _output;

    public GeneratorTests(ITestOutputHelper output)
    {
        timeProvider = new MockTimeProvider("2022-01-01T12:00Z");
        generator = new Generator(timeProvider);
        _output = output;
    }

    private readonly IGenerator generator;
    private readonly MockTimeProvider timeProvider;

    [Fact]
    public void SingleBranchFirstTime()
    {
        Assert.Equal("branch1.20220101.1", generator.GetNextBuildNumber("id1", "branch1", TimeZoneInfo.Utc));
    }

    [Fact]
    public void TwoBranchFirstTime()
    {
        Assert.Equal("branch1.20220101.1", generator.GetNextBuildNumber("id1", "branch1", TimeZoneInfo.Utc));
        Assert.Equal("branch2.20220101.1", generator.GetNextBuildNumber("id1", "branch2", TimeZoneInfo.Utc));
    }

    [Fact]
    public void SameBranchDifferentId()
    {
        Assert.Equal("branch1.20220101.1", generator.GetNextBuildNumber("id1", "branch1", TimeZoneInfo.Utc));
        Assert.Equal("branch1.20220101.1", generator.GetNextBuildNumber("id2", "branch1", TimeZoneInfo.Utc));
    }

    [Fact]
    public void SameBranchSameId()
    {
        Assert.Equal("branch1.20220101.1", generator.GetNextBuildNumber("id1", "branch1", TimeZoneInfo.Utc));
        Assert.Equal("branch1.20220101.2", generator.GetNextBuildNumber("id1", "branch1", TimeZoneInfo.Utc));
    }

    [Fact]
    public void NextDay()
    {
        Assert.Equal("branch1.20220101.1", generator.GetNextBuildNumber("id1", "branch1", TimeZoneInfo.Utc));
        Assert.Equal("branch1.20220101.2", generator.GetNextBuildNumber("id1", "branch1", TimeZoneInfo.Utc));
        timeProvider.SetMockTimeUTC("2022-01-02T12:00Z");
        Assert.Equal("branch1.20220102.1", generator.GetNextBuildNumber("id1", "branch1", TimeZoneInfo.Utc));      
    }

    [Fact]
    public void MinuteBeforeMidnightPST()
    {
        var localTime = new DateTime(2022, 1, 1, 23, 59, 00, DateTimeKind.Unspecified);
        TimeZoneInfo tzPST = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
        _output.WriteLine("\nDEBUG--tzPST---");
        _output.WriteLine(tzPST.ToSerializedString());
        _output.WriteLine("DEBUG--tzPST---\n");

        var UTCTime = TimeZoneInfo.ConvertTimeToUtc(localTime, tzPST);

        _output.WriteLine("\nDEBUG--UTCTime---");
        _output.WriteLine(UTCTime.ToString());
        _output.WriteLine("DEBUG--UTCTime---\n");

        timeProvider.SetMockTimeUTC(UTCTime);
        Assert.Equal("branch1.20220101.1", generator.GetNextBuildNumber("id1", "branch1", TimeZoneInfo.Local));
        
        timeProvider.SetMockTimeUTC(UTCTime.AddMinutes(1));
        Assert.Equal("branch1.20220102.1", generator.GetNextBuildNumber("id1", "branch1", TimeZoneInfo.Local));
    }    
}