using System;
using Scriptable_Objects;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// The passport as a thing in the world. Rolls its own passport on Awake for now,
/// later this is where grabbing, visuals, UI etc. go.
/// </summary>
public class PassportObject : MonoBehaviour
{
    private const string WearablesPath = "Items/Wearables";

    [Tooltip("Rolls a new seed on every Generate. Turn off to regenerate the same passport from Seed.")]
    [SerializeField]
    private bool randomSeed = true;

    [SerializeField] private int seed;

    /// <summary>
    ///     The rolled passport, use Passport.GetMissing(outfit) to check the player against it.
    /// </summary>
    public Passport Passport { get; private set; }

    /// <summary>
    ///     Fires after every Generate with the new passport, PassportView listens to this to bake.
    /// </summary>
    public event Action<Passport> Generated;

    private void Awake()
    {
        Generate();
    }

    /// <summary>
    ///     Rolls a fresh passport from everything in the wearables folder.
    /// </summary>
    [ContextMenu("Generate")]
    public void Generate()
    {
        var pool = Resources.LoadAll<ItemDefinition>(WearablesPath);
        if (randomSeed) seed = Random.Range(int.MinValue, int.MaxValue);
        Passport = Passport.CreateRandom(seed, pool);
        Debug.Log($"{name} rolled (seed {seed}): {Passport}", this);
        Generated?.Invoke(Passport);
    }
}