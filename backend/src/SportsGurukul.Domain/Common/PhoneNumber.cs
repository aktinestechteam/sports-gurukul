namespace SportsGurukul.Domain.Common;

public sealed class PhoneNumber : IEquatable<PhoneNumber>
{
    public string CountryCode { get; }
    public string Number { get; }
    public bool IsVerified { get; }

    public PhoneNumber(string countryCode, string number, bool isVerified = false)
    {
        CountryCode = countryCode;
        Number = number;
        IsVerified = isVerified;
    }

    public string FullNumber => $"{CountryCode}{Number}";

    public override bool Equals(object? obj) => Equals(obj as PhoneNumber);

    public bool Equals(PhoneNumber? other)
    {
        if (other is null) return false;
        return CountryCode == other.CountryCode
            && Number == other.Number
            && IsVerified == other.IsVerified;
    }

    public override int GetHashCode() => HashCode.Combine(CountryCode, Number, IsVerified);

    public static bool operator ==(PhoneNumber? left, PhoneNumber? right) => Equals(left, right);
    public static bool operator !=(PhoneNumber? left, PhoneNumber? right) => !Equals(left, right);

    public override string ToString() => FullNumber;
}
