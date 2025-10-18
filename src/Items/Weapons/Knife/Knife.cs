using MRCustom.Modules.Weapons.Rotations;
using SlugCrafting.Modules.Weapons;

namespace SlugCrafting.Items.Weapons;

public class Knife : Weapon
{
    public LodgeInCreatureWeaponModule lodgeInCreatureModule;
    public SmallWeaponRotationModule smallWeaponRotationModule;
    public ThrowViolenceWeaponModule throwViolenceModule;
    public StabWeaponModule stabWeaponModule;

    public virtual float DefaultAnchorY => 0.5f;
    public virtual float CarriedAnchorY => DefaultAnchorY - 0.25f;
    public virtual float LodgedAnchorY => DefaultAnchorY + 0.3f;

    public float stabDamageBonus = 0.4f;

    private static float Rand => UnityEngine.Random.value;

    private float faceDirection = 1f;

    protected float adjustmentUpWhenCarried = 2;

    public int TotalSprites = 2;

    private Color handleColor;
    public Color HandleColor
    {
        get => handleColor;
        set => handleColor = value;
    }
    private Color bladeColor;
    public Color BladeColor
    {
        get => bladeColor;
        set => bladeColor = value;
    }

    private readonly float GrabbedRotationYOffset = 30;
    public Knife(AbstractKnife  abstr, Vector2 pos, Vector2 vel) 
        : base(abstr, abstr.world)
    {
        bodyChunks = new[] {
            new BodyChunk(this, 0, pos, 0.5f, 0.14f)
        };
        bodyChunks[0].lastPos = bodyChunks[0].pos = pos;
        bodyChunks[0].vel = vel;

        bodyChunkConnections = new BodyChunkConnection[0];

        throwViolenceModule = new SeverPoleMimicThrowViolenceWeaponModule(this)
        {
            damageBonus = 0.4f,
            creatureKnockbackMultiplier = 2f,
            hitSound = SoundID.Spear_Stick_In_Creature,
        };
        this.AddModule(throwViolenceModule);

        this.smallWeaponRotationModule = new SmallWeaponRotationModule(this)
        {
            rotationDegreesOffset = -60f
        };
        this.AddModule(smallWeaponRotationModule);

        lodgeInCreatureModule = new LodgeInCreatureWeaponModule(this, firstChunk)
        {
            stickSound = SoundID.Spear_Stick_In_Creature,
        };
        this.AddModule(lodgeInCreatureModule);

        stabWeaponModule = new StabWeaponModule(this)
        {
            damage = 1f,
            timeStabOccurs = 30 // Base game maul occurence time is 40, a little more leniancy here.
        };
        this.AddModule(stabWeaponModule);

        airFriction = 0.999f;
        gravity = 0.9f;
        bounce = 0.1f;
        surfaceFriction = 0.45f;
        collisionLayer = 1;
        waterFriction = 0.92f;
        buoyancy = 0.75f;

        rotation = new Vector2(0, Rand * 360f);

        soundLoop = new ChunkDynamicSoundLoop(base.firstChunk);
    }

    public override void ChangeMode(Mode newMode)
    {
        // If old mode was stuck in creature.
        if (base.mode == Mode.StuckInCreature)
        {
            lodgeInCreatureModule.PullFromStuckObject();
        }

        base.ChangeMode(newMode);

        if (newMode == Mode.Carried)
        {
            rotationSpeed = 0;
        }
    }

    public override void SetRandomSpin()
    {
        if (room != null)
        {
            rotationSpeed = (1f) * Mathf.Lerp(50f, 150f, UnityEngine.Random.value) * Mathf.Lerp(0.05f, 1f, room.gravity);
        }
    }

    public override void Thrown(Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        base.Thrown(thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
        //-- MS7: The pitch for throwing knives is much smaller than spears, so it communicates itself as a knife and less like a spear.
        room?.PlaySound(SoundID.Slugcat_Throw_Spear, base.firstChunk, false, 0.9f, UnityEngine.Random.Range(1.5f, 1.9f));

        SetRandomSpin();
    }

    public override void HitWall()
    {
        if (room.BeingViewed)
        {
            for (int i = 0; i < 4; i++)
            {
                room.AddObject(new Spark(base.firstChunk.pos + throwDir.ToVector2() * (base.firstChunk.rad - 1f), Custom.DegToVec(Random.value * 360f) * 10f * Random.value + -throwDir.ToVector2() * 10f, new Color(1f, 1f, 1f), null, 2, 4));
            }
        }
        room.ScreenMovement(base.firstChunk.pos, throwDir.ToVector2() * 1.2f, 0f);
        room.PlaySound(SoundID.Spear_Bounce_Off_Wall, base.firstChunk, false, 1f, UnityEngine.Random.Range(1.4f, 1.8f));
        SetRandomSpin();
        ChangeMode(Mode.Free);
        base.forbiddenToPlayer = 10;
    }

    public override bool HitSomething(SharedPhysics.CollisionResult result, bool eu)
    {
        if (throwViolenceModule.HitSomething(firstChunk, result, eu))
        {
            lodgeInCreatureModule.StickToCollisionResult(result, eu);
            return true;
        }
        return false;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        smallWeaponRotationModule.Update();
        lodgeInCreatureModule.Update(eu);

        ChangeCollisionLayer(grabbedBy.Count == 0 ? 2 : 1);
        firstChunk.collideWithTerrain = grabbedBy.Count == 0;
        firstChunk.collideWithSlopes = grabbedBy.Count == 0;

        soundLoop.sound = SoundID.None;
        if (firstChunk.vel.magnitude > 5f)
        {
            if (mode == Mode.Thrown)
                soundLoop.sound = SoundID.Spear_Thrown_Through_Air_LOOP;
            else if (mode == Mode.Free)
                soundLoop.sound = SoundID.Spear_Spinning_Through_Air_LOOP;

            // Copied from source for how spears set their sound loop volume.
            soundLoop.Volume = Mathf.InverseLerp(5f, 15f, base.firstChunk.vel.magnitude);
            //-- MS7: The pitch for throwing knives is much smaller than spears, so it communicates itself as a knife and less like a spear.
            soundLoop.Pitch = 1.5f;
        }
        soundLoop.Update();
    }

    //
    // IDRAWABLES
    //

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = new FSprite("knifeBlade", true);
        sLeaser.sprites[1] = new FSprite("knifeHandle", true);
        AddToContainer(sLeaser, rCam, null);
    }

    // TODO: fix this visually to act more like a tiny spear, copy whatever code is used for the spear.
    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
        Vector2 rot3 = Vector3.Slerp(lastRotation, rotation, timeStacker);
        float scaleX = Mathf.Lerp(smallWeaponRotationModule.LastScaleX, smallWeaponRotationModule.ScaleX, timeStacker);

        float anchorY;

        // Reposition sprite to fit more naturally on hand.
        if (base.mode == Mode.Carried)
        {
            // Need to move the sprites a little further up hand, and since rotation is directional we can use that.
            //pos += Custom.rotateVectorDeg(rot3, -90 * scaleX) * adjustmentUpWhenCarried; // Disabled, need to use direction from shoulder to hand to calculate.

            anchorY = CarriedAnchorY;
        }
        else if (base.mode == Mode.StuckInCreature)
        {
            anchorY = LodgedAnchorY;
        }
        else
        {
            anchorY = DefaultAnchorY;
        }

        // Loop through and update the sprites.

        float finalRotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), rot3);

        for (int i = 0; i < TotalSprites; i++)
        {
            sLeaser.sprites[i].x = pos.x - camPos.x;
            sLeaser.sprites[i].y = pos.y - camPos.y;
            sLeaser.sprites[i].rotation = finalRotation;
            sLeaser.sprites[i].scaleX = scaleX;
            sLeaser.sprites[i].anchorY = anchorY;
        }

        if (blink > 0 && UnityEngine.Random.value < 0.5f)
        {
            sLeaser.sprites[0].color = base.blinkColor;
            sLeaser.sprites[1].color = base.blinkColor;
        }
        else
        {
            sLeaser.sprites[0].color = bladeColor;
            sLeaser.sprites[1].color = handleColor;
        }
        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        handleColor = palette.blackColor; //Color.Lerp(palette.blackColor, new Color(0.9f, 0.9f, 0.9f), 0.9f);
        bladeColor = palette.blackColor;
    }

    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContainer)
    {
        newContainer ??= rCam.ReturnFContainer("Items");

        foreach (FSprite fsprite in sLeaser.sprites)
        {
            fsprite.RemoveFromContainer();
            newContainer.AddChild(fsprite);
        }
    }
}