namespace SlugCrafting.Items.Weapons;

public class KingVultureSpearFisobs : Fisob
{
    public KingVultureSpearProperties properties = new KingVultureSpearProperties();

    public KingVultureSpearFisobs() : base(SlugCraftingEnums.AbstractObjectType.KingVultureSpear)
    {
        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
        RegisterUnlock(SlugCraftingEnums.SandboxUnlockID.KingVultureSpear, parent: MultiplayerUnlocks.SandboxUnlockID.KingVulture, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        var result = new AbstractKingVultureSpear(world, null, saveData.Pos, saveData.ID);
        return result;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return properties;
    }
}

