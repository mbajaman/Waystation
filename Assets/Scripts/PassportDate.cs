/// <summary>
/// Calendar date for passport info.
/// </summary>
[System.Serializable]
public readonly struct PassportDate : System.IEquatable<PassportDate>
{
	public readonly int Year;
	public readonly int Month;
	public readonly int Day;

	public PassportDate(int year, int month, int day)
	{
		Year = year;
		Month = month;
		Day = day;
	}

	/// <summary>
	/// Rolls a valid date between minYear and maxYear defaults to between year 3000 and 5000
	/// </summary>
	public static PassportDate CreateRandom(System.Random rng, int minYear = 3000, int maxYear = 5000)
	{
		int year = rng.Next(minYear, maxYear + 1);
		int month = rng.Next(1, 13);
		int day = rng.Next(1, System.DateTime.DaysInMonth(year, month) + 1);
		return new PassportDate(year, month, day);
	}

	public bool Equals(PassportDate other) => Year == other.Year && Month == other.Month && Day == other.Day;

	public override bool Equals(object obj) => obj is PassportDate other && Equals(other);

	public override int GetHashCode() => System.HashCode.Combine(Year, Month, Day);

	public static bool operator ==(PassportDate left, PassportDate right) => left.Equals(right);

	public static bool operator !=(PassportDate left, PassportDate right) => !left.Equals(right);

	public override string ToString() => $"{Day:00}/{Month:00}/{Year}";
}
