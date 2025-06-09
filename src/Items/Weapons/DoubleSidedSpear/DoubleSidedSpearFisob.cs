/*
using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace SlugCrafting.Weapons;

sealed class DoubleSidedSpearFisob : Fisob
{
    public static readonly AbstractPhysicalObject.AbstractObjectType abstractObjectType = new("DoubleSidedSpear", true);

    public DoubleSidedSpearFisob() : base(abstractObjectType)
    {
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        // TODO: add data later for this.

        var result = new AbstractDoubleSidedSpear(null, world, null, saveData.Pos, saveData.ID);
        return result;
    }

    private static readonly KnifeProperties properties = new();

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        // If you need to use the forObject parameter, pass it to your ItemProperties class's constructor.
        return properties;
    }
}
*/