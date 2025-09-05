namespace SlugCrafting.Properties;

public interface IArmorAccessoryProperties : IAccessoryProperties
{
    /// <summary>
    /// The modifier to any damage done to this armor, how resistant it is to damage.
    /// Also due to damage logic, effects how long stuns last for.
    /// </summary>
    public float Toughness { get; }
    /// <summary>
    /// The modifier to any explosive damage done to this armor, how resistant it is to explosive damage.
    /// (Stacks with normal toughness)
    /// </summary>
    public float ExplosiveToughness { get; }
}
