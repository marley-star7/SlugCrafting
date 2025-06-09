using SlugCrafting.Items;

namespace SlugCrafting.Items;

public class LizardHideScavenge : AbstractPhysicalObjectScavenge
{
    public LizardHideScavenge(Lizard lizard) :  base(lizard) { }

    public override AbstractPhysicalObject Scavenge()
    {
        var lizard = owner as Lizard;
        var leather = new AbstractLizardHide(lizard.room.world, lizard.coord, lizard.room.game.GetNewID());
        canScavenge = false; // Prevents the scavenge from being used again.

        return leather;
    }
}
