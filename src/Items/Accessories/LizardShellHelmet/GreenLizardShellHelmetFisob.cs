using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace SlugCrafting.Items;

sealed class GreenLizardShellHelmetFisob : Fisob
{
    public static readonly AbstractPhysicalObject.AbstractObjectType abstractObjectType = new("GreenLizardShellHelmet", true);
    public static readonly MultiplayerUnlocks.SandboxUnlockID sandboxUnlockID = new("GreenLizardShellHelmet", true);

    public static readonly GreenLizardShellHelmetProperties properties = new();

    public GreenLizardShellHelmetFisob() : base(abstractObjectType)
    {
        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
        RegisterUnlock(sandboxUnlockID, parent: MultiplayerUnlocks.SandboxUnlockID.Slugcat, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        var result = new AbstractLizardShellHelmet(world, abstractObjectType, saveData.Pos, saveData.ID);
        return result;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return properties;
    }
}