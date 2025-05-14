using SlugCrafting.Animations.Scavenging;
using SlugCrafting.Items;
using SlugCrafting.Scavenges.ScavengeSpots;

namespace SlugCrafting.Scavenges;

public class GreenLizardScavengeData : CreatureScavengeData
{
    public Lizard lizard;
    public LizardGraphics lizardGraphics;

    public GreenLizardScavengeData(Lizard lizard) : base(lizard) { }

    protected override void RealizeScavengeData()
    {
        lizard = creature as Lizard;
        lizardGraphics = lizard.graphicsModule as LizardGraphics;

        ScavengeSpots = new Dictionary<ScavengeSpot, AbstractPhysicalObjectScavenge>()
        {
            { new ScavengeSpot(0, 0, 0), new LizardShellScavenge(lizard, GreenLizardShellFisob.abstractObjectType)
                {
                    scavengeTime = 250,
                    animation = new SawBackForthScavengeAnimation(250),
                }
            },
            { new ScavengeSpot(1, 0, 0), new LizardHideScavenge(lizard)
                {
                    scavengeTime = 100,
                    animation = new SawBackForthScavengeAnimation(100),
                }
            },
        };
    }
}
