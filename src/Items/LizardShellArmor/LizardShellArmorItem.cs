using SlugCrafting.Core;

namespace SlugCrafting.Items;

public abstract class LizardShellArmorItem : PlayerCarryableItem, IDrawable, IEquippable
{
    public AbstractLizardShellArmor abstractLizardShellArmor;

    public LizardShellArmor lizardShellArmor;

    private Creature? _wearer;
    public Creature? Wearer
    {
        get => _wearer;
    }

    /// <summary>
    /// Rotation is always normalized vector pointing.
    /// Z axis of rotation is used for the effect of looking towards the camera.
    /// </summary>
    public Vector3 rotation;
    public Vector3 lastRotation;

    public float maxHealth = 2;

    public LizardShellArmorItem(AbstractLizardShellArmor abstractLizardShellArmor, LizardShellArmor lizardShellArmor) : base(abstractLizardShellArmor)
    {
        this.abstractLizardShellArmor = abstractLizardShellArmor;
        this.lizardShellArmor = lizardShellArmor;

        var pos = abstractPhysicalObject.Room.realizedRoom.MiddleOfTile(abstractPhysicalObject.pos.Tile);

        bodyChunks = new[] {
            new BodyChunk(this, 0, pos, 1, 0.1f),
        };

        bodyChunkConnections = new BodyChunkConnection[0];
    }

    public override void Update(bool eu)
    {
        lastRotation = rotation;

        base.Update(eu);
        lizardShellArmor.Update(eu);

        //-- MS7: Effect for when being held to look more 3d.
        if (grabbedBy.Count > 0)
        {
            var dirAndMagnitudeToGrabber = firstChunk.pos - grabbedBy[0].grabber.firstChunk.pos;
            rotation = dirAndMagnitudeToGrabber.normalized;
            rotation.z = Mathf.InverseLerp(0, 1, dirAndMagnitudeToGrabber.magnitude);
        }
        else
        {
            rotation = 0.9f * Custom.DirVec(firstChunk.lastPos, firstChunk.pos) * Custom.Dist(firstChunk.lastPos, firstChunk.pos);
        }
    }

    public override void HitByWeapon(Weapon weapon)
    {
        base.HitByWeapon(weapon);

        lizardShellArmor.DoDeflectEffects(firstChunk, firstChunk.pos, weapon.firstChunk.vel, 1, 0);
    }

    public override void TerrainImpact(int chunkIndex, IntVector2 direction, float speed, bool firstContact)
    {
        base.TerrainImpact(chunkIndex, direction, speed, firstContact);

        var directionVec2 = new Vector2(direction.x, direction.y);
        lizardShellArmor.DoTerrainImpactEffects(bodyChunks[chunkIndex], directionVec2, speed, firstContact);
    }

    public abstract void Equip(Player wearer);

    public void EquipLizardShellArmorAccessory(LizardShellArmorAccessory lizardShellArmorAccessory)
    {
        lizardShellArmor.lizardShellEffectsModule.Owner = lizardShellArmorAccessory;

        abstractPhysicalObject.realizedObject.AllGraspsLetGoOfThisObject(true);
        abstractPhysicalObject.Abstractize(abstractPhysicalObject.pos);
    }

    //
    // IDrawable and IDynamicCosmetic
    //

    public virtual void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        lizardShellArmor.InitiateSprites(sLeaser, rCam);
        AddToContainer(sLeaser, rCam, null);
    }

    public abstract void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos);

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        lizardShellArmor.ApplyPalette(sLeaser, rCam, palette);
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
    {
        newContainer ??= rCam.ReturnFContainer("Items");

        foreach (FSprite fsprite in sLeaser.sprites)
        {
            fsprite.RemoveFromContainer();
            newContainer.AddChild(fsprite);
        }
    }
}
