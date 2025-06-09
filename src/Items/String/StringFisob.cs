using RWCustom;
using UnityEngine;

using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace SlugCrafting.Items;

public class StringFisob : Fisob
{
    public static readonly AbstractPhysicalObject.AbstractObjectType abstractObjectType = new("String", true);
    public static readonly MultiplayerUnlocks.SandboxUnlockID stringUnlockID = new("String", true);

    public static readonly StringProperties properties = new();

    public StringFisob() : base(abstractObjectType)
    {
        SandboxPerformanceCost = new(linear: 0.1f, exponential: 0f);
        RegisterUnlock(stringUnlockID, parent: MultiplayerUnlocks.SandboxUnlockID.Slugcat, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        var result = new AbstractString(world, saveData.Pos, saveData.ID);
        return result;
    }

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        return properties;
    }
}
