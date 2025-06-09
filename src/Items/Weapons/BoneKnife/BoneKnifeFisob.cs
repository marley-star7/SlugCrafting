using Fisobs.Core;
using Fisobs.Items;
using Fisobs.Properties;
using Fisobs.Sandbox;

namespace SlugCrafting.Items.Weapons;
	/*
sealed class BoneKnifeFisob : Fisob
{
	public static readonly AbstractPhysicalObject.AbstractObjectType AbstractBoneKnife = new("BoneKnife", true);
	public static readonly MultiplayerUnlocks.SandboxUnlockID BoneKnife = new("BoneKnife", true);

	public BoneKnifeFisob() : base(AbstractBoneKnife)
	{
		SandboxPerformanceCost = new(linear: 0.3f, exponential: 0f);

		RegisterUnlock(BoneKnife, parent: MultiplayerUnlocks.SandboxUnlockID.Slugcat, data: 0);
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
	*/