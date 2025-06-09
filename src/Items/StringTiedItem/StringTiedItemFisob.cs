using RWCustom;
using UnityEngine;

using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;


namespace SlugCrafting.Items;

public sealed class StringTiedItemFisob : Fisob
{
    public static readonly AbstractPhysicalObject.AbstractObjectType abstractObjectType = new("StringTiedObject", true);

    public static readonly StringProperties properties = new();

    public StringTiedItemFisob() : base(abstractObjectType)
    {

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
