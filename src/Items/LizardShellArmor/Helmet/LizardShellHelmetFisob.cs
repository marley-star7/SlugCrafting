using MRCustom.Json;

namespace SlugCrafting.Items;

public abstract class LizardShellHelmetFisob : Fisob
{
    public abstract LizardShellHelmetItemProperties ItemProperties { get; }

    public LizardShellHelmetFisob(AbstractPhysicalObject.AbstractObjectType abstractObjectType) : base(abstractObjectType)
    {
        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
    }

    public void RegisterItemPropertiesType(AbstractPhysicalObject.AbstractObjectType abstractObjectType, LizardShellHelmetItemProperties itemProperties)
    {
        LizardShellHelmetItemProperties.typesProperties.Add(abstractObjectType, itemProperties);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        var result = new AbstractLizardShellHelmet(world, Type, saveData.Pos, saveData.ID);
        LizardShellArmorFisob.ParseArmorData(result, saveData);

        return result;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return ItemProperties;
    }
}