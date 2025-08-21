namespace SlugCrafting.Items;

public class CordFisob : Fisob
{
    public static readonly CordProperties properties = new();

    public CordFisob() : base(SlugCraftingEnums.AbstractObjectType.Cord)
    {
        CordProperties.typesProperties.Add(SlugCraftingEnums.AbstractObjectType.Cord, properties);

        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
        RegisterUnlock(SlugCraftingEnums.SandboxUnlockID.Cord, parent: MultiplayerUnlocks.SandboxUnlockID.PoleMimic, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        var result = new AbstractCord(SlugCraftingEnums.AbstractObjectType.Cord, world, saveData.Pos, saveData.ID);
        return result;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return properties;
    }
}
