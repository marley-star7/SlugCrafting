namespace SlugCrafting.Items;

public abstract class AbstractLizardShellArmor : AbstractPhysicalObject
{
    public Color shellColor;
    public float health;

    public AbstractLizardShellArmor(LizardShellArmorItemProperties itemProperties, World world, AbstractObjectType type, WorldCoordinate pos, EntityID ID) : base(world, type, null, pos, ID)
    {
        this.shellColor = itemProperties.ArmorAccessoryProperties.DefaultShellColor;
        this.health = itemProperties.ArmorAccessoryProperties.MaxHealth;
    }

    public AbstractLizardShellArmor(AbstractLizardHeadShell abstractLizardShell, LizardShellArmorItemProperties itemProperties, AbstractObjectType type, EntityID ID) : base(abstractLizardShell.world, type, null, abstractLizardShell.pos, ID)
    {
        this.shellColor = abstractLizardShell.shellColor;
        this.health = itemProperties.ArmorAccessoryProperties.MaxHealth;
    }

    public override string ToString()
    {
        return this.SaveToString($"{shellColor};{health};");
    }
}
