namespace SlugCrafting.Items;

public class StringScavenge : AbstractPhysicalObjectScavenge
{
    public StringScavenge(LanternMouse mouse) : base(mouse) { }

    public override AbstractPhysicalObject Scavenge()
    {
        var mouse = owner as LanternMouse;
        var stringItem = new AbstractString(mouse.room.world, mouse.coord, mouse.room.game.GetNewID());
        canScavenge = false; // Prevents the scavenge from being used again.

        return stringItem;
    }
}
