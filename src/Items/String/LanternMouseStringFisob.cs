namespace SlugCrafting.Items;

public class LanternMouseStringFisob : Fisob
{
    public static readonly LanternMouseStringProperties properties = new();

    public LanternMouseStringFisob() : base(SlugCraftingEnums.AbstractObjectType.LanternMouseString)
    {
        CordProperties.typesProperties.Add(SlugCraftingEnums.AbstractObjectType.LanternMouseString, properties);

        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
        RegisterUnlock(SlugCraftingEnums.SandboxID.LanternMouseString, parent: MultiplayerUnlocks.SandboxUnlockID.LanternMouse, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        var result = new AbstractCord(SlugCraftingEnums.AbstractObjectType.LanternMouseString, world, saveData.Pos, saveData.ID);
        return result;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return properties;
    }
}
