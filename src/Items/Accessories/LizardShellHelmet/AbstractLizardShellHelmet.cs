using SlugCrafting.Accessories;

namespace SlugCrafting.Items;

public class AbstractLizardShellHelmet : AbstractPhysicalObject
{
    public AbstractLizardShellHelmet(World world, AbstractObjectType type, WorldCoordinate pos, EntityID ID) : base(world, type, null, pos, ID)
    {
    }

    public override void Realize()
    {
        base.Realize();
        if (realizedObject == null)
            realizedObject = new LizardShellHelmet(this);
    }
}
