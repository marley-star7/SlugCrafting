using SlugCrafting.Items;

public class AbstractLizardShellCuirass : AbstractLizardShellArmor
{
    public AbstractLizardShellCuirass(World world, AbstractPhysicalObject.AbstractObjectType type, WorldCoordinate pos, EntityID ID) : base(LizardShellCuirassItemProperties.GetPropertiesForType(type), world, type, pos, ID)
    {
    }

    public AbstractLizardShellCuirass(AbstractLizardHeadShell abstractLizardShell, AbstractObjectType type, EntityID ID) : base(abstractLizardShell, LizardShellCuirassItemProperties.GetPropertiesForType(type), type, ID)
    {
    }

    public override void Realize()
    {
        base.Realize();

        LizardShellCuirassItemProperties itemProperties = LizardShellCuirassItemProperties.GetPropertiesForType(type);

        if (realizedObject == null)
            realizedObject = new LizardShellCuirassItem(this);
    }
}
