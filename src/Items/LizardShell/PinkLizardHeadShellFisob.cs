using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace SlugCrafting.Items;

sealed class PinkLizardShellFisob : Fisob
{
    public static readonly PinkLizardShellProperties properties = new();

    public PinkLizardShellFisob() : base(Enums.AbstractObjectType.PinkLizardHeadShell)
    {
        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
        RegisterUnlock(Enums.SandboxUnlockID.PinkLizardHeadShell, parent: MultiplayerUnlocks.SandboxUnlockID.PinkLizard, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        // TODO: add data later for this.

        var result = new AbstractLizardHeadShell(world, CreatureTemplate.Type.PinkLizard, saveData.Pos, saveData.ID);
        return result;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return properties;
    }
}