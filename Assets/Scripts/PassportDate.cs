/// <summary>
/// Calendar date for passport info.
/// </summary>
[System.Serializable]
public readonly struct PassportDate
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

	public override string ToString() => $"{Day:00}/{Month:00}/{Year}";
}
