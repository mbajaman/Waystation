using System.Collections.Generic;
using System.Linq;
using Scriptable_Objects;
using UnityEngine;

/// <summary>
/// The look the player has to match. Rolled at runtime, one required item per clothing type.
/// Matching only cares about type + color, so any lookalike item counts.
/// </summary>
public class Passport
{
	public IReadOnlyList<ItemDefinition> Required => _required;
	private readonly List<ItemDefinition> _required;

	private Passport(List<ItemDefinition> required)
	{
		_required = required;
	}

	/// <summary>
	/// Rolls a new passport by picking one random item per clothing type from the pool.
	/// Types with nothing in the pool just get skipped.
	/// </summary>
	/// <param name="pool">Every item the passport is allowed to pick from</param>
	public static Passport CreateRandom(IEnumerable<ItemDefinition> pool)
	{
		List<ItemDefinition> picks = pool
			.Where(item => item != null)
			.GroupBy(item => item.type)
			.Select(group =>
			{
				List<ItemDefinition> options = group.ToList();
				return options[Random.Range(0, options.Count)];
			})
			.ToList();

		return new Passport(picks);
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

	public override string ToString() => string.Join(", ", _required.Select(item => $"{item.color} {item.type}"));
}
