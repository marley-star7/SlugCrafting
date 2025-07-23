namespace SlugCrafting.Scavenges;

public class PinkLizardScavengeData : CreatureScavengeData
{
    public PinkLizardScavengeData(Creature creature) : base(creature) { }

    protected override void RealizeScavengeData()
    {
        var lizard = creature as Lizard;

        ScavengeSpots = new Dictionary<ScavengeSpot, AbstractPhysicalObjectScavenge>()
        {
            // Head index is 0
            { new ScavengeSpot(0, 0, 0), new LizardShellScavenge(lizard)
                {
                    scavengeTime = 200,
                    handAnimation = SlugCraftingEnums.HandAnimationIndex.SawBackForthScavenge,
                }
            },
            // Body Index is 1
            { new ScavengeSpot(1, 0, 0), new LizardHideScavenge(lizard)
                {
                    scavengeTime = 100,
                    handAnimation = SlugCraftingEnums.HandAnimationIndex.SawBackForthScavenge,
                }
            },
            // Body Index is 1
            { new ScavengeSpot(2, 0, 0), new LizardHideScavenge(lizard)
                {
                    scavengeTime = 100,
                    handAnimation = SlugCraftingEnums.HandAnimationIndex.SawBackForthScavenge,
                }
            },
        };
    }
}
