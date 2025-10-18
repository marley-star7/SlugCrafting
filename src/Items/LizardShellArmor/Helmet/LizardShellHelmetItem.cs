namespace SlugCrafting.Items;

public class LizardShellHelmetItem : LizardShellArmorItem
{
    public LizardShellHelmet LizardShellHelmet
    {
        get => (LizardShellHelmet)base.lizardShellArmor;
        set => base.lizardShellArmor = value;
    }

    private Creature? _wearer;
    public Creature? Wearer
    {
        get => _wearer;
    }

    public AbstractLizardShellHelmet abstractLizardShellHelmet;

    public LizardShellHelmetItem(AbstractLizardShellHelmet abstractLizardShellHelmet, LizardShellHelmet lizardShellHelmet) : base(abstractLizardShellHelmet, lizardShellHelmet)
    {
        if (LizardShellHelmet == null)
        {
            LizardShellHelmet = new LizardShellHelmet(abstractLizardShellHelmet, LizardShellHelmetItemProperties.GetPropertiesForType(abstractLizardShellHelmet.type), new LizardShellEffectsModule(this, abstractLizardShellHelmet.shellColor));
        }

        var pos = abstractPhysicalObject.Room.realizedRoom.MiddleOfTile(abstractPhysicalObject.pos.Tile);

        bodyChunks = new[] {
            new BodyChunk(this, 0, pos, 1, 0.1f),
        };

        bodyChunkConnections = new BodyChunkConnection[0];

        airFriction = 0.97f;
        gravity = 0.9f;
        bounce = 0.1f;
        surfaceFriction = 0.45f;
        collisionLayer = 1;
        waterFriction = 0.92f;
        buoyancy = 0.75f;
    }

    public override void Equip(Player wearer)
    {
        EquipLizardShellArmorAccessory(new LizardShellHelmetAccessory(wearer, LizardShellHelmet));
    }

    public override void Update(bool eu)
    {
        lastRotation = rotation;

        base.Update(eu);

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

    //
    // IDrawable and IDynamicCosmetic
    //

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        if (Wearer != null)
            return; // Use on wearer draw instead.

        Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);

        Vector3 rotVec = Vector3.Slerp(lastRotation, rotation, timeStacker);
        float rot = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), rotVec);

        string angle = LizardShellCuirass.GetAngleForRotationZ(rotVec.z);

        pos -= camPos; // Offset by camera position to draw in the correct place.

        LizardShellHelmet.DrawSpritesContext context = new LizardShellHelmet.DrawSpritesContext(
            posX: pos.x,
            posY: pos.y,
            scaleX: 1,
            scaleY: 1,
            rotation: rot,
            spriteAngle: angle,
            lookDirX: 0,
            lookDirY: 0
        );
        LizardShellHelmet.DrawSprites(sLeaser, rCam, timeStacker, camPos, context);

        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }
}
