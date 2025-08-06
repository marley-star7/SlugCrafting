using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace SlugCrafting.Items;

sealed class GreenLizardShellHelmetFisob : Fisob
{
    public static readonly GreenLizardShellHelmetProperties properties = new();

    public GreenLizardShellHelmetFisob() : base(SlugCraftingEnums.AbstractObjectType.GreenLizardShellHelmet)
    {
        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
        RegisterUnlock(SlugCraftingEnums.SandboxID.GreenLizardShellHelmet, parent: MultiplayerUnlocks.SandboxUnlockID.Slugcat, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        // Centi shield data is just floats separated by ; characters.
        string[] parsedData = saveData.CustomData.Split(';');

        if (parsedData.Length < 2)
        {
            parsedData = new string[2];
        }

        var result = new AbstractLizardShellHelmet(world, SlugCraftingEnums.AbstractObjectType.GreenLizardShellHelmet, properties, saveData.Pos, saveData.ID);

        if (MarColorExtensions.TryParse(parsedData[0], out var shellColorParsed))
            result.shellColor = shellColorParsed;

        if (float.TryParse(parsedData[0], out var healthParsed))
            result.health = healthParsed;

        return result;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return properties;
    }
}