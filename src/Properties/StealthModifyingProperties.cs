namespace SlugCrafting.Properties;

public interface IStealthModifyingProperties
{
    /// <summary>
    /// The extra tiles you are spottable from when creatures check if they can see you
    /// <see href="https://rainworld.miraheze.org/wiki/Slugcat">Table of Slugcat Differences</see>
    /// </summary>
    public float GeneralVisibilityBonusMultiplier { get; }
    /// <summary>
    /// Multiplier ontop of your current passive loudness
    /// <see href="https://rainworld.miraheze.org/wiki/Slugcat">Table of Slugcat Differences</see>
    /// </summary>
    public float LoudnessMultiplier { get; }
}
