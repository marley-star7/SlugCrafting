using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace SlugCrafting.Items;

sealed class LizardHideFisob : Fisob
{
    public static readonly AbstractPhysicalObject.AbstractObjectType abstractObjectType = new("LizardLeather", true);
    public static readonly MultiplayerUnlocks.SandboxUnlockID sandboxUnlockID = new("LizardLeather", true);

    public LizardHideFisob() : base(abstractObjectType)
    {
        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);

        RegisterUnlock(sandboxUnlockID, parent: MultiplayerUnlocks.SandboxUnlockID.Slugcat, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        // TODO: add data later for this.

        var result = new AbstractLizardHide(world, saveData.Pos, saveData.ID);
        return result;
    }

    private static readonly LizardHideProperties properties = new();

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return properties;
    }
}