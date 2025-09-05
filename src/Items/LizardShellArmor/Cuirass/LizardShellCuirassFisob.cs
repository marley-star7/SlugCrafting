using MRCustom.Json;

namespace SlugCrafting.Items;

public abstract class LizardShellCuirassFisob : Fisob
{
    public abstract LizardShellCuirassItemProperties ItemProperties { get; }

    public LizardShellCuirassFisob(AbstractPhysicalObject.AbstractObjectType abstractObjectType) : base(abstractObjectType)
    {
        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
    }

    public void RegisterItemPropertiesType(AbstractPhysicalObject.AbstractObjectType abstractObjectType, LizardShellCuirassItemProperties itemProperties)
    {
        LizardShellCuirassItemProperties.typesProperties.Add(abstractObjectType, itemProperties);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        var result = new AbstractLizardShellCuirass(world, Type, saveData.Pos, saveData.ID);
        LizardShellArmorFisob.ParseArmorData(result, saveData);

        return result;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return ItemProperties;
    }
}