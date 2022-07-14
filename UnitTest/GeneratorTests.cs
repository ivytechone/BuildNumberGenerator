using Xunit;
using BuildNumberGenerator;
using Xunit.Abstractions;

public class GeneratorTests
{
    public GeneratorTests(ITestOutputHelper output)
    {
        timeProvider = new MockTimeProvider("2022-01-01T12:00Z");
        generator = new Generator(timeProvider);
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
        var UTCTime = TimeZoneInfo.ConvertTimeToUtc(localTime, tzPST);

        timeProvider.SetMockTimeUTC(UTCTime);
        Assert.Equal("branch1.20220101.1", generator.GetNextBuildNumber("id1", "branch1", tzPST));
        
        timeProvider.SetMockTimeUTC(UTCTime.AddMinutes(1));
        Assert.Equal("branch1.20220102.1", generator.GetNextBuildNumber("id1", "branch1", tzPST));
    }
    
    [Fact]
    public void PurgeBuildsTest()
    {
        Assert.Equal("branch1.20220101.1", generator.GetNextBuildNumber("id1", "branch1", TimeZoneInfo.Utc));
        timeProvider.SetMockTimeUTC("2022-01-01T12:10Z");
        Assert.Equal("branch1.20220101.2", generator.GetNextBuildNumber("id1", "branch1", TimeZoneInfo.Utc));
        timeProvider.SetMockTimeUTC("2022-01-02T12:01Z"); 
        Assert.Equal(1, generator.PurgeBuilds());
        Assert.Equal("branch1.20220102.1", generator.GetNextBuildNumber("id1", "branch1", TimeZoneInfo.Utc));    
        timeProvider.SetMockTimeUTC("2022-01-02T18:00Z");
        Assert.Equal(0, generator.PurgeBuilds());            
    }

    [Fact]
    public void PurgeMultipleBuildsTest()
    {
        Assert.Equal("branch1.20220101.1", generator.GetNextBuildNumber("id1", "branch1", TimeZoneInfo.Utc));
        timeProvider.SetMockTimeUTC("2022-01-02T12:10Z");
        Assert.Equal("branch1.20220102.1", generator.GetNextBuildNumber("id1", "branch1", TimeZoneInfo.Utc));
        timeProvider.SetMockTimeUTC("2022-01-03T12:30Z"); 
        Assert.Equal(2, generator.PurgeBuilds());
        Assert.Equal(0, generator.PurgeBuilds());     // empty queue        
    }
}