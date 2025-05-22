using SlugCrafting.Items;
using SlugCrafting.Scavenges.ScavengeSpots;

namespace SlugCrafting.Scavenges;

public class PinkLizardScavengeData : CreatureScavengeData
{
    public LizardGraphics lizardGraphics;

    public PinkLizardScavengeData(Creature creature) : base(creature) { }

    protected override void RealizeScavengeData()
    {
        var lizard = creature as Lizard;
        lizardGraphics = lizard.graphicsModule as LizardGraphics;

        ScavengeSpots = new Dictionary<ScavengeSpot, AbstractPhysicalObjectScavenge>()
        {
            // Head index is 0
            { new ScavengeSpot(0, 0, 0), new LizardShellScavenge(lizard, PinkLizardShellFisob.abstractObjectType)
                {
                    scavengeTime = 200,
                    handAnimation = Enums.HandAnimationIndex.SawBackForthScavenge,
                }
            },
            // Body Index is 1
            { new ScavengeSpot(1, 0, 0), new LizardHideScavenge(lizard)
                {
                    scavengeTime = 100,
                    handAnimation = Enums.HandAnimationIndex.SawBackForthScavenge,
                }
            },
            // Body Index is 1
            { new ScavengeSpot(2, 0, 0), new LizardHideScavenge(lizard)
                {
                    scavengeTime = 100,
                    handAnimation = Enums.HandAnimationIndex.SawBackForthScavenge,
                }
            },
        };
    }
}
