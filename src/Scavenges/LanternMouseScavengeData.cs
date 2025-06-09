using SlugCrafting.Items;

namespace SlugCrafting.Scavenges;

public class LanternMouseScavengeData : CreatureScavengeData
{
    public LanternMouseScavengeData(LanternMouse mouse) : base(mouse) { }

    protected override void RealizeScavengeData()
    {
        var mouse = creature as LanternMouse;

        ScavengeSpots = new Dictionary<ScavengeSpot, AbstractPhysicalObjectScavenge>()
        {
            {
                new ScavengeSpot(0, 0, 0), new StringScavenge(mouse)
                {
                    scavengeTime = 50,
                    handAnimation = Enums.HandAnimationIndex.SawBackForthScavenge,
                    requiresKnife = false, // LanternMouse can scavenge without a knife
                }
            },
        };
    }
}
