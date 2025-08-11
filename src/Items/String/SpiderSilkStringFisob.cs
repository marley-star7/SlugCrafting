namespace SlugCrafting.Items;

public class SpiderSilkStringFisob : Fisob
{
    public static readonly SpiderSilkStringProperties properties = new();

    public SpiderSilkStringFisob() : base(SlugCraftingEnums.AbstractObjectType.SpiderSilkString)
    {
        CordProperties.typesProperties.Add(SlugCraftingEnums.AbstractObjectType.SpiderSilkString, properties);

        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
        RegisterUnlock(SlugCraftingEnums.SandboxID.SpiderSilkString, parent: MultiplayerUnlocks.SandboxUnlockID.Spider, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        var result = new AbstractCord(SlugCraftingEnums.AbstractObjectType.SpiderSilkString, world, saveData.Pos, saveData.ID);
        return result;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return properties;
    }
}
