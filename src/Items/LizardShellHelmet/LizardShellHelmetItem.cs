namespace SlugCrafting.Items;

// TODO: maybe the solution is having most code run through an accessory object, which does the graphics and such, and is passed between the object during creation and the player when attached.
// TODO: keep developing like this though, changing to that will only require minor changes to the code, and will allow for more complex accessories in the future.

public class LizardShellHelmetItem : PlayerCarryableItem, IDrawable, IEquippable
{
    public LizardShellHelmet lizardShellHelmet;

    private Creature? _wearer;
    public Creature? Wearer
    {
        get => _wearer;
    }

    public AbstractLizardShellHelmet abstractLizardShellHelmet;

    /// <summary>
    /// Rotation is always normalized vector pointing.
    /// Z axis of rotation is used for the effect of looking towards the camera.
    /// </summary>
    public Vector3 rotation;
    public Vector3 lastRotation;

    public float maxHealth = 2;

    public const int WearingBodyChunkIndex = 0;

    public LizardShellHelmetItem(AbstractLizardShellHelmet abstractHeadAccessory, LizardShellHelmet lizardShellHelmet) : base(abstractHeadAccessory)
    {
        this.lizardShellHelmet = lizardShellHelmet;
        abstractLizardShellHelmet = abstractHeadAccessory;

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

    public void Equip(Player wearer)
    {
        new LizardShellHelmetAccessory(wearer, lizardShellHelmet);
        this.Destroy();
    }

    public override void Update(bool eu)
    {
        lastRotation = rotation;

        base.Update(eu);
        lizardShellHelmet.Update(eu);

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

        lizardShellHelmet.DoDeflectEffects(firstChunk, firstChunk.pos, weapon.firstChunk.vel, 1, 0);
    }

    public override void TerrainImpact(int chunkIndex, IntVector2 direction, float speed, bool firstContact)
    {
        base.TerrainImpact(chunkIndex, direction, speed, firstContact);

        var directionVec2 = new Vector2(direction.x, direction.y);
        lizardShellHelmet.DoTerrainImpactEffects(bodyChunks[chunkIndex], directionVec2, speed, firstContact);
    }

    //
    // IDrawable and IDynamicCosmetic
    //

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        lizardShellHelmet.InitiateSprites(sLeaser, rCam);
        AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        if (Wearer != null)
            return; // Use on wearer draw instead.

        Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);

        Vector3 rotVec = Vector3.Slerp(lastRotation, rotation, timeStacker);
        float rot = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), rotVec);

        // The higher the z angle is, the closer it is to "facing" the player, change the angle based on that.
        string angle;
        if (Math.Abs(rotVec.z) > 0.6f)
        {
            angle = "A0";
        }
        else if (Math.Abs(rotVec.z) > 0.3f)
        {
            angle = "A1";
        }
        else
        {
            angle = "A0";
        }

        pos -= camPos; // Offset by camera position to draw in the correct place.

        LizardShellHelmet.DrawSpritesContext context = new LizardShellHelmet.DrawSpritesContext(pos.x, pos.y, 1, 1, rot, angle, 0, 0);
        lizardShellHelmet.DrawSprites(sLeaser, rCam, timeStacker, camPos, context);

        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        lizardShellHelmet.ApplyPalette(sLeaser, rCam, palette);
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
