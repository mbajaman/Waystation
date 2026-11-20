using Scriptable_Objects;
using UnityEngine;

/// <summary>
/// The passport as a thing in the world. Rolls its own passport on Awake for now,
/// later this is where grabbing, visuals, UI etc. go.
/// </summary>
public class PassportObject : MonoBehaviour
{
	private const string WearablesPath = "Items/Wearables";

	/// <summary>
	/// The rolled passport, use Passport.GetMissing(outfit) to check the player against it.
	/// </summary>
	public Passport Passport { get; private set; }

	private void Awake()
	{
		Generate();
	}

	/// <summary>
	/// Rolls a fresh passport from everything in the wearables folder.
	/// </summary>
	[ContextMenu("Generate")]
	public void Generate()
	{
		ItemDefinition[] pool = Resources.LoadAll<ItemDefinition>(WearablesPath);
		Passport = Passport.CreateRandom(pool);
		Debug.Log($"{name} rolled: {Passport}", this);
	}
}
