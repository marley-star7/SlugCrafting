using Fisobs.Core;

namespace SlugCrafting.Items;

public class AbstractLizardShellHelmet : AbstractPhysicalObject
{
    public LizardShellHelmetProperties properties;

    public Color shellColor;
    public float health;

    public AbstractLizardShellHelmet(World world, AbstractObjectType type, LizardShellHelmetProperties properties, WorldCoordinate pos, EntityID ID) : base(world, type, null, pos, ID)
    {
        this.properties = properties;

        this.shellColor = properties.defaultShellColor;
        this.health = properties.maxHealth;
    }

    public override void Realize()
    {
        base.Realize();
        if (realizedObject == null)
            realizedObject = new LizardShellHelmetItem(this, new LizardShellHelmet(this, properties));
    }

    public override string ToString()
    {
        return this.SaveToString($"{shellColor};{health};");
    }
}
