namespace SlugCrafting.Items.Weapons;

public class AbstractKingVultureSpear : AbstractSpear
{
    public AbstractKingVultureSpear(World world, Spear realizedObject, WorldCoordinate pos, EntityID ID)
        : base(world, realizedObject, pos, ID, explosive: false)
    {
    }

    public override void Realize()
    {
        base.Realize();
        realizedObject = new KingVultureSpear(this);
    }
}
