namespace SlugCrafting.Items;

public class LanternMouseStringScavenge : AbstractPhysicalObjectScavenge
{
    public LanternMouseStringScavenge(LanternMouse mouse) : base(mouse) { }

    public override AbstractPhysicalObject Scavenge()
    {
        var mouse = owner as LanternMouse;
        var stringItem = new AbstractCord(SlugCraftingEnums.AbstractObjectType.LanternMouseString, mouse.room.world, mouse.coord, mouse.room.game.GetNewID());
        canScavenge = false; // Prevents the scavenge from being used again.

        return stringItem;
    }
}
