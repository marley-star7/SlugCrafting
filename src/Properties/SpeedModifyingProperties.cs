namespace SlugCrafting.Properties;

public interface ISpeedModifyingProperties
{
    public float RunSpeedLinearModifier { get; }
    public float PoleClimbSpeedMultiplier { get; }
    public float CorridorClimbSpeedMultiplier { get; }

    public float SwimForceMultiplier { get; }
    public float SwimBoostMultiplier { get; }
}
