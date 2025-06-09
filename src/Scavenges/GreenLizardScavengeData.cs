using SlugCrafting.Items;

namespace SlugCrafting.Scavenges;

public class GreenLizardScavengeData : CreatureScavengeData
{
    public GreenLizardScavengeData(Lizard lizard) : base(lizard) { }

    protected override void RealizeScavengeData()
    {
        var lizard = creature as Lizard;

        ScavengeSpots = new Dictionary<ScavengeSpot, AbstractPhysicalObjectScavenge>()
        {
            { new ScavengeSpot(0, 0, 0), new LizardShellScavenge(lizard)
                {
                    scavengeTime = 200,
                    handAnimation = Enums.HandAnimationIndex.SawBackForthScavenge,
                }
            },
            { new ScavengeSpot(1, 0, 0), new LizardHideScavenge(lizard)
                {
                    scavengeTime = 100,
                    handAnimation =  Enums.HandAnimationIndex.SawBackForthScavenge,
                }
            },
        };
    }
}
