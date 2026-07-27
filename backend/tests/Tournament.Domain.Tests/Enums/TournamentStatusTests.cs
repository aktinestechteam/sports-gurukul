using SportsGurukul.Domain.Enums;

namespace Tournament.Domain.Tests.Enums;

public class TournamentStatusTests
{
    [Fact]
    public void TournamentStatus_Draft_ShouldBeZero()
    {
        ((int)TournamentStatus.Draft).Should().Be(0);
    }

    [Fact]
    public void TournamentStatus_Published_ShouldBeOne()
    {
        ((int)TournamentStatus.Published).Should().Be(1);
    }

    [Fact]
    public void TournamentStatus_RegistrationOpen_ShouldBeTwo()
    {
        ((int)TournamentStatus.RegistrationOpen).Should().Be(2);
    }

    [Fact]
    public void TournamentStatus_RegistrationClosed_ShouldBeThree()
    {
        ((int)TournamentStatus.RegistrationClosed).Should().Be(3);
    }

    [Fact]
    public void TournamentStatus_FixtureGeneration_ShouldBeFour()
    {
        ((int)TournamentStatus.FixtureGeneration).Should().Be(4);
    }

    [Fact]
    public void TournamentStatus_Live_ShouldBeFive()
    {
        ((int)TournamentStatus.Live).Should().Be(5);
    }

    [Fact]
    public void TournamentStatus_Paused_ShouldBeSix()
    {
        ((int)TournamentStatus.Paused).Should().Be(6);
    }

    [Fact]
    public void TournamentStatus_Completed_ShouldBeSeven()
    {
        ((int)TournamentStatus.Completed).Should().Be(7);
    }

    [Fact]
    public void TournamentStatus_Archived_ShouldBeEight()
    {
        ((int)TournamentStatus.Archived).Should().Be(8);
    }

    [Fact]
    public void AllStatusValues_ShouldBeDefined()
    {
        var values = Enum.GetValues<TournamentStatus>();

        values.Should().Contain(TournamentStatus.Draft);
        values.Should().Contain(TournamentStatus.Published);
        values.Should().Contain(TournamentStatus.RegistrationOpen);
        values.Should().Contain(TournamentStatus.RegistrationClosed);
        values.Should().Contain(TournamentStatus.FixtureGeneration);
        values.Should().Contain(TournamentStatus.Live);
        values.Should().Contain(TournamentStatus.Paused);
        values.Should().Contain(TournamentStatus.Completed);
        values.Should().Contain(TournamentStatus.Archived);
    }

    [Fact]
    public void StatusValues_ShouldBeSequential()
    {
        ((int)TournamentStatus.Draft).Should().Be(0);
        ((int)TournamentStatus.Published).Should().Be(1);
        ((int)TournamentStatus.RegistrationOpen).Should().Be(2);
        ((int)TournamentStatus.RegistrationClosed).Should().Be(3);
        ((int)TournamentStatus.FixtureGeneration).Should().Be(4);
        ((int)TournamentStatus.Live).Should().Be(5);
        ((int)TournamentStatus.Paused).Should().Be(6);
        ((int)TournamentStatus.Completed).Should().Be(7);
        ((int)TournamentStatus.Archived).Should().Be(8);
    }

    [Fact]
    public void TournamentStatus_ShouldHaveNineValues()
    {
        var values = Enum.GetValues<TournamentStatus>();

        values.Length.Should().Be(9);
    }
}
