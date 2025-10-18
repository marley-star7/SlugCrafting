namespace SlugCrafting.Creatures;

public static class PoleMimicExtensions
{
    public static void SeverAndCordify(this PoleMimic poleMimic, PhysicalObject.Appendage.Pos atAppendage)
    {
        new AbstractCord(Enums.AbstractObjectType.Cord, poleMimic.room.world, poleMimic.abstractCreature.pos, poleMimic.room.game.GetNewID()).RealizeInRoom();
        poleMimic.Sever(atAppendage);
    }
}
