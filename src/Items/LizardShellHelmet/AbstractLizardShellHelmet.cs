using Fisobs.Core;

namespace SlugCrafting.Items;

public class AbstractLizardShellHelmet : AbstractPhysicalObject
{
    public Color shellColor;
    public float health;

    public LizardShellHelmetProperties GetPropertiesForType(AbstractObjectType type)
    {
        if (LizardShellHelmetProperties.typesProperties.TryGetValue(type, out var properties))
            return properties;
        else
            return new LizardShellHelmetProperties();
    }

    public AbstractLizardShellHelmet(World world, AbstractObjectType type, WorldCoordinate pos, EntityID ID) : base(world, type, null, pos, ID)
    {
        LizardShellHelmetProperties properties = GetPropertiesForType(type);

        this.shellColor = properties.defaultShellColor;
        this.health = properties.maxHealth;
    }

    public AbstractLizardShellHelmet(AbstractLizardShell abstractLizardShell, AbstractObjectType type, EntityID ID) : base(abstractLizardShell.world, type, null, abstractLizardShell.pos, ID)
    {
        LizardShellHelmetProperties properties = GetPropertiesForType(type);

        this.shellColor = abstractLizardShell.shellColor;
        this.health = properties.maxHealth;
    }

    public override void Realize()
    {
        base.Realize();

        LizardShellHelmetProperties properties = GetPropertiesForType(type);

        if (realizedObject == null)
            realizedObject = new LizardShellHelmetItem(this, new LizardShellHelmet(this, properties));
    }

    public override string ToString()
    {
        return this.SaveToString($"{shellColor};{health};");
    }
}
