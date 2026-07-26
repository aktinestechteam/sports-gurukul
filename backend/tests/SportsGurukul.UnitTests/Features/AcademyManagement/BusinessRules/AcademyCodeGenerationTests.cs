using System.Text.RegularExpressions;
using FluentAssertions;

namespace SportsGurukul.UnitTests.Features.AcademyManagement.BusinessRules;

public class AcademyCodeGenerationTests
{
    private static string GenerateAcademyCode()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = Guid.NewGuid().ToString("N")[..4].ToUpperInvariant();
        return $"ACAD-{datePart}-{randomPart}";
    }

    [Fact]
    public void AcademyCode_StartsWithACAD()
    {
        var code = GenerateAcademyCode();

        code.Should().StartWith("ACAD-");
    }

    [Fact]
    public void AcademyCode_ContainsDateFormat()
    {
        var code = GenerateAcademyCode();

        var datePart = code.Substring(5, 8);
        datePart.Should().MatchRegex(@"^\d{8}$");

        var year = int.Parse(datePart[..4]);
        var month = int.Parse(datePart[4..6]);
        var day = int.Parse(datePart[6..8]);

        year.Should().BeInRange(2020, 2030);
        month.Should().BeInRange(1, 12);
        day.Should().BeInRange(1, 31);
    }

    [Fact]
    public void AcademyCode_ContainsRandomSuffix()
    {
        var code = GenerateAcademyCode();

        var suffix = code.Substring(14, 4);
        suffix.Should().MatchRegex(@"^[A-Z0-9]{4}$");
    }

    [Fact]
    public void AcademyCode_IsExactly18Characters()
    {
        var code = GenerateAcademyCode();

        code.Length.Should().Be(18);
    }

    [Fact]
    public void AcademyCode_HasCorrectStructure()
    {
        var code = GenerateAcademyCode();

        code.Should().Contain("-");
        var parts = code.Split('-');
        parts.Should().HaveCount(3);
        parts[0].Should().Be("ACAD");
        parts[1].Should().HaveLength(8);
        parts[2].Should().HaveLength(4);
    }

    [Fact]
    public void AcademyCode_GeneratedCodesAreUnique()
    {
        var codes = new HashSet<string>();
        for (int i = 0; i < 100; i++)
        {
            codes.Add(GenerateAcademyCode());
        }

        codes.Should().HaveCount(100);
    }
}
