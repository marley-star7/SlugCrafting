using SlugCrafting.Animations.Scavenging;
using SlugCrafting.Items;

namespace SlugCrafting.Scavenges.ScavengeSpots
{
    public class LizardLeatherScavenge : AbstractPhysicalObjectScavenge
    {
        public LizardLeatherScavenge(Lizard lizard) :  base(lizard) { }

        public override AbstractPhysicalObject Scavenge()
        {
            var lizard = owner as Lizard;
            var leather = new AbstractLizardLeather(lizard.room.world, lizard.coord, lizard.room.game.GetNewID());
            canScavenge = false; // Prevents the scavenge from being used again.

            return leather;
        }
    }
}
