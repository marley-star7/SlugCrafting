﻿using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace SlugCrafting.Items;

sealed class GreenLizardShellFisob : Fisob
{
    public static readonly AbstractPhysicalObject.AbstractObjectType abstractObjectType = new("GreenLizardShell", true);
    public static readonly MultiplayerUnlocks.SandboxUnlockID sandboxUnlockID = new("GreenLizardShell", true);

    public static readonly PinkLizardShellProperties properties = new();

    public GreenLizardShellFisob() : base(abstractObjectType)
    {
        LizardShellProperties.PropertiesOfTemplateType[CreatureTemplate.Type.GreenLizard] = properties;

        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
        RegisterUnlock(sandboxUnlockID, parent: MultiplayerUnlocks.SandboxUnlockID.Slugcat, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        // TODO: add data later for this.

        var result = new AbstractLizardShell(world, CreatureTemplate.Type.GreenLizard, saveData.Pos, saveData.ID);
        return result;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return properties;
    }
}