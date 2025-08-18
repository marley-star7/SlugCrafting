using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace SlugCrafting.Items;

/// <summary>
/// The generic Lizard Shell Fisob, usually for modded lizards or lizards without custom shell implementation/crafts yet.
/// </summary>
sealed class LizardHeadShellFisob : Fisob
{
    public LizardHeadShellFisob() : base(SlugCraftingEnums.AbstractObjectType.LizardHeadShell)
    {

    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        // TODO: add data later for this.

        var result = new AbstractLizardHeadShell(world, CreatureTemplate.Type.LizardTemplate, saveData.Pos, saveData.ID);
        return result;
    }

    public static readonly LizardHeadShellProperties properties = new(null);

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return properties;
    }
}