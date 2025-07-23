namespace SlugCrafting.Scavenges;

/// <summary>
/// Default scavenge data for lizards, used for non-specificed lizard type scavenges.
/// Only contains a single Hide and Shell scavenge spot.
/// </summary>
public class LizardScavengeData : CreatureScavengeData
{
    public LizardScavengeData(Lizard lizard) : base(lizard) { }

    protected override void RealizeScavengeData()
    {
        var lizard = creature as Lizard;

        ScavengeSpots = new Dictionary<ScavengeSpot, AbstractPhysicalObjectScavenge>()
        {
            {
                new ScavengeSpot(0, 0, 0), new LizardShellScavenge(lizard)
                {
                    scavengeTime = 200,
                    handAnimation = SlugCraftingEnums.HandAnimationIndex.SawBackForthScavenge,
                }
            },
            {   
                new ScavengeSpot(1, 0, 0), new LizardHideScavenge(lizard)
                {
                    scavengeTime = 100,
                    handAnimation =  SlugCraftingEnums.HandAnimationIndex.SawBackForthScavenge,
                }
            },
        };
    }
}
