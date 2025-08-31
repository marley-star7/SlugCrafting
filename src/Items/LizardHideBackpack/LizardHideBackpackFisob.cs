using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace SlugCrafting.Items;

sealed class LizardHideBackpackFisob : Fisob
{
    public LizardHideBackpackFisob() : base(Enums.AbstractObjectType.LizardHideBackpack)
    {
        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
        RegisterUnlock(Enums.SandboxUnlockID.LizardHideBackpack, parent: MultiplayerUnlocks.SandboxUnlockID.Slugcat, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        var result = new AbstractLizardHideBackpack(world, saveData.Pos, saveData.ID);
        return result;
    }

    public static readonly LizardHideBackpackProperties properties = new();

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return properties;
    }
}