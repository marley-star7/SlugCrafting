using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace SlugCrafting.Items;

sealed class KnifeFisob : Fisob
{
    public static readonly AbstractPhysicalObject.AbstractObjectType abstractObjectType = new("Knife", true);
    public static readonly MultiplayerUnlocks.SandboxUnlockID knife = new("Knife", true);

    public KnifeFisob() : base(abstractObjectType)
    {
        // Fisobs auto-loads the `icon_CentiShield` embedded resource as a texture.
        // See `CentiShields.csproj` for how you can add embedded resources to your project.

        // If you want a simple grayscale icon, you can omit the following line.
        //Icon = new CentiShieldIcon();

        SandboxPerformanceCost = new(linear: 0.3f, exponential: 0f);

        RegisterUnlock(knife, parent: MultiplayerUnlocks.SandboxUnlockID.Slugcat, data: 0);
    }

    public override AbstractPhysicalObject Parse(World world, EntitySaveData saveData, SandboxUnlock? unlock)
    {
        // TODO: add data later for this.

        var result = new AbstractKnife(world, saveData.Pos, saveData.ID);
        return result;
    }

    private static readonly KnifeProperties properties = new();

    public override ItemProperties Properties(PhysicalObject forObject)
    {
        // If you need to use the forObject parameter, pass it to your ItemProperties class's constructor.
        return properties;
    }
}