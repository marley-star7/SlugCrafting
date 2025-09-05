using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace SlugCrafting.Items;

// TODO: Ms7: Actually make a generic lizardHeadShellFisob item thats used instead of this,

/// <summary>
/// The generic Lizard Shell Fisob, usually for modded lizards or lizards without custom shell implementation/crafts yet.
/// </summary>
public abstract class LizardHeadShellFisob : Fisob
{
    public CreatureTemplate.Type CreatureType;

    public abstract LizardHeadShellItemProperties ItemProperties { get; }

    public LizardHeadShellFisob(AbstractPhysicalObject.AbstractObjectType abstractObjectType, CreatureTemplate.Type creatureTemplateType) : base(abstractObjectType)
    {
        this.CreatureType = creatureTemplateType;

        RegisterItemPropertiesCreatureTemplateType(creatureTemplateType, ItemProperties);

        Icon = new LizardHeadShellIcon(ItemProperties.DefaultShellColor);
        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
    }

    private void RegisterItemPropertiesCreatureTemplateType(CreatureTemplate.Type lizardType, LizardHeadShellItemProperties itemProperties)
    {
        LizardHeadShellItemProperties.PropertiesOfTemplateType.Add(lizardType, itemProperties);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        // TODO: add data later for this.

        var result = new AbstractLizardHeadShell(world, CreatureTemplate.Type.LizardTemplate, saveData.Pos, saveData.ID);
        return result;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return ItemProperties;
    }
}