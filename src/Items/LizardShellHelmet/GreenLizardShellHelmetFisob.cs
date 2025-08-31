using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;
using MRCustom.Json;

namespace SlugCrafting.Items;

sealed class GreenLizardShellHelmetFisob : Fisob
{
    public static readonly GreenLizardShellHelmetProperties properties = new();

    public GreenLizardShellHelmetFisob() : base(Enums.AbstractObjectType.GreenLizardShellHelmet)
    {
        LizardShellHelmetProperties.typesProperties.Add(Enums.AbstractObjectType.GreenLizardShellHelmet, properties);

        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
        RegisterUnlock(Enums.SandboxUnlockID.GreenLizardShellHelmet, parent: MultiplayerUnlocks.SandboxUnlockID.GreenLizard, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        // Centi shield data is just floats separated by ; characters.
        string[] parsedData = saveData.CustomData.Split(';');

        if (parsedData.Length < 2)
        {
            parsedData = new string[2];
        }

        var result = new AbstractLizardShellHelmet(world, Enums.AbstractObjectType.GreenLizardShellHelmet, saveData.Pos, saveData.ID);

        if (MRJson.TryParseColor(parsedData[0], out var shellColorParsed))
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