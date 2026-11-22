using System.Collections.Generic;
using System.Linq;
using Scriptable_Objects;

/// <summary>
/// The look the player has to match. Rolled at runtime, one required item per clothing type.
/// Matching only cares about type + color, so any lookalike item counts.
/// </summary>
public class Passport
{
	//Wearables
	public IReadOnlyList<ItemDefinition> Required => _required;
	private readonly List<ItemDefinition> _required;
	//Passport info
	public PassportDate BirthDate { get; }

	private Passport(List<ItemDefinition> required, PassportDate birthDate)
	{
		_required = required;
		BirthDate = birthDate;
	}

	/// <summary>
	/// Rolls a new passport by picking one random item per clothing type from the pool, then a birth date.
	/// Types with nothing in the pool just get skipped. The same seed and pool always give the same passport.
	/// </summary>
	/// <param name="seed">Seed for every roll in this passport</param>
	/// <param name="pool">Every item the passport is allowed to pick from</param>
	public static Passport CreateRandom(int seed, IEnumerable<ItemDefinition> pool)
	{
		var rng = new System.Random(seed);

		// sorted so the result doesn't depend on the order the pool was loaded in
		List<ItemDefinition> picks = pool
			.Where(item => item != null)
			.OrderBy(item => item.type)
			.ThenBy(item => item.name, System.StringComparer.Ordinal)
			.GroupBy(item => item.type)
			.Select(group =>
			{
				List<ItemDefinition> options = group.ToList();
				return options[rng.Next(options.Count)];
			})
			.ToList();
		PassportDate birthDate = PassportDate.CreateRandom(rng);
		return new Passport(picks, birthDate);
	}

	/// <summary>
	/// Returns whatever the outfit doesn't cover yet. Empty list means it's a match.
	/// </summary>
	/// <param name="outfit">What the player is currently wearing</param>
	public List<ItemDefinition> GetMissing(IEnumerable<ItemDefinition> outfit)
	{
		List<ItemDefinition> worn = outfit.Where(item => item != null).ToList();
		return _required
			.Where(req => !worn.Any(item => item.type == req.type && item.color == req.color))
			.ToList();
	}

	public override string ToString() => $"{string.Join(", ", _required.Select(item => $"{item.color} {item.type}"))} | born {BirthDate}";
}
