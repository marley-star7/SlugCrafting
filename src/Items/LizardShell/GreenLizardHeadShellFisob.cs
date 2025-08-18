using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace SlugCrafting.Items;

sealed class GreenLizardShellFisob : Fisob
{
    public static readonly GreenLizardShellProperties properties = new();

    public GreenLizardShellFisob() : base(SlugCraftingEnums.AbstractObjectType.GreenLizardHeadShell)
    {
        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
        RegisterUnlock(SlugCraftingEnums.SandboxID.GreenLizardHeadShell, parent: MultiplayerUnlocks.SandboxUnlockID.GreenLizard, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        // TODO: add data later for this.

        var result = new AbstractLizardHeadShell(world, CreatureTemplate.Type.GreenLizard, saveData.Pos, saveData.ID);
        return result;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return properties;
    }
}