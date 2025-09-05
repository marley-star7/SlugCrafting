namespace SlugCrafting.Properties;

public interface IDurabilityAccessoryProperties : IAccessoryProperties
{
    /// <summary>
    /// How much damage the item can take before breaking.
    /// </summary>
    public float MaxHealth { get; }
}
