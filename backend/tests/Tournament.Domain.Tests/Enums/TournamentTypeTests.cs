using SportsGurukul.Domain.Enums;

namespace Tournament.Domain.Tests.Enums;

public class TournamentTypeTests
{
    [Fact]
    public void TournamentType_League_ShouldBeZero()
    {
        ((int)TournamentType.League).Should().Be(0);
    }

    [Fact]
    public void TournamentType_Knockout_ShouldBeOne()
    {
        ((int)TournamentType.Knockout).Should().Be(1);
    }

    [Fact]
    public void TournamentType_RoundRobin_ShouldBeTwo()
    {
        ((int)TournamentType.RoundRobin).Should().Be(2);
    }

    [Fact]
    public void TournamentType_Swiss_ShouldBeThree()
    {
        ((int)TournamentType.Swiss).Should().Be(3);
    }

    [Fact]
    public void TournamentType_DoubleElimination_ShouldBeFour()
    {
        ((int)TournamentType.DoubleElimination).Should().Be(4);
    }

    [Fact]
    public void TournamentType_Mixed_ShouldBeFive()
    {
        ((int)TournamentType.Mixed).Should().Be(5);
    }

    [Fact]
    public void AllTypes_ShouldBeDefined()
    {
        var values = Enum.GetValues<TournamentType>();

        values.Should().Contain(TournamentType.League);
        values.Should().Contain(TournamentType.Knockout);
        values.Should().Contain(TournamentType.RoundRobin);
        values.Should().Contain(TournamentType.Swiss);
        values.Should().Contain(TournamentType.DoubleElimination);
        values.Should().Contain(TournamentType.Mixed);
    }

    [Fact]
    public void TypeValues_ShouldBeSequential()
    {
        ((int)TournamentType.League).Should().Be(0);
        ((int)TournamentType.Knockout).Should().Be(1);
        ((int)TournamentType.RoundRobin).Should().Be(2);
        ((int)TournamentType.Swiss).Should().Be(3);
        ((int)TournamentType.DoubleElimination).Should().Be(4);
        ((int)TournamentType.Mixed).Should().Be(5);
    }

    [Fact]
    public void TournamentType_ShouldHaveSixValues()
    {
        var values = Enum.GetValues<TournamentType>();

        values.Length.Should().Be(6);
    }
}
