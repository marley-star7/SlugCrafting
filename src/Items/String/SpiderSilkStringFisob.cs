namespace SlugCrafting.Items;

public class SpiderSilkStringFisob : Fisob
{
    public static readonly SpiderSilkStringProperties properties = new();

    public SpiderSilkStringFisob() : base(Enums.AbstractObjectType.SpiderSilkString)
    {
        CordProperties.typesProperties.Add(Enums.AbstractObjectType.SpiderSilkString, properties);

        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
        RegisterUnlock(Enums.SandboxUnlockID.SpiderSilkString, parent: MultiplayerUnlocks.SandboxUnlockID.Spider, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        var result = new AbstractCord(Enums.AbstractObjectType.SpiderSilkString, world, saveData.Pos, saveData.ID);
        return result;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return properties;
    }
}
