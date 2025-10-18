namespace SlugCrafting.Items;

public class AbstractLizardShellHelmet : AbstractLizardShellArmor
{
    public AbstractLizardShellHelmet(World world, AbstractPhysicalObject.AbstractObjectType type, WorldCoordinate pos, EntityID ID) : base(LizardShellHelmetItemProperties.GetPropertiesForType(type), world, type, pos, ID)
    {
    }

    public AbstractLizardShellHelmet(AbstractLizardHeadShell abstractLizardShell, AbstractObjectType type, EntityID ID) : base(abstractLizardShell, LizardShellHelmetItemProperties.GetPropertiesForType(type), type, ID)
    {
    }

    public override void Realize()
    {
        base.Realize();

        LizardShellHelmetItemProperties properties = LizardShellHelmetItemProperties.GetPropertiesForType(type);

        if (realizedObject == null)
            realizedObject = new LizardShellHelmetItem(this, null);
    }
}
