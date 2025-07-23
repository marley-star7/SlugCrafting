using RWCustom;
using UnityEngine;

using MRCustom.Math;
using System.Numerics;

namespace SlugCrafting.Items.Weapons;

// TODO: steal spear spragment texture to do for the basic knife :] (cuz its spear)

sealed class Knife : Weapon
{
    public int stuckInChunkIndex = -1;

    public PhysicalObject stuckInObject = null;
    public BodyChunk stuckInChunk => stuckInObject.bodyChunks[stuckInChunkIndex];
    public Appendage.Pos stuckInAppendage;

    public float stabDamageBonus = 0.4f;
    public float throwDamageBonus = 0.4f;

    public float stuckRotation;

    private static float Rand => UnityEngine.Random.value;

    private float faceDirection = 1f;

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
    public Knife(AbstractKnife abstr, Vector2 pos, Vector2 vel) 
        : base(abstr, abstr.world)
    {
        bodyChunks = new[] {
            new BodyChunk(this, 0, pos, 0.5f, 0.14f)
        };
        bodyChunks[0].lastPos = bodyChunks[0].pos = pos;
        bodyChunks[0].vel = vel;

        bodyChunkConnections = new BodyChunkConnection[0];

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
        if (base.mode == Mode.StuckInCreature)
        {
            rotationSpeed = 0;
            if (room != null)
                room.PlaySound(SoundID.Spear_Dislodged_From_Creature, base.firstChunk, false, 1f, UnityEngine.Random.Range(1.4f, 1.8f));

            PulledOutOfStuckObject();
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
        //-- MR7: The pitch for throwing knives is much smaller than spears, so it communicates itself as a knife and less like a spear.
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
        if (result.obj == null)
            return false;
        if (result.obj.abstractPhysicalObject.rippleLayer != abstractPhysicalObject.rippleLayer && !result.obj.abstractPhysicalObject.rippleBothSides && !abstractPhysicalObject.rippleBothSides)
            return false;

        if (result.obj is Creature)
        {
            var hitCreature = (Creature)result.obj;
            hitCreature.Violence(this.firstChunk, base.firstChunk.vel * base.firstChunk.mass * 2f, result.chunk, result.onAppendagePos, Creature.DamageType.Stab, throwDamageBonus, 10f);
            room.PlaySound(SoundID.Spear_Stick_In_Creature, base.firstChunk, false, 1f, UnityEngine.Random.Range(1.4f, 1.8f));
            LodgeInCreature(result, eu);

            return true;
        }
        else if (result.chunk != null)
            result.chunk.vel += base.firstChunk.vel * base.firstChunk.mass / result.chunk.mass; // TODO: for simplicity maybe add an extension method for ApplyForceOnChunk or something like that, so don't have to re-use formula constantly?
        else if (result.onAppendagePos != null)
            (result.obj as IHaveAppendages).ApplyForceOnAppendage(result.onAppendagePos, base.firstChunk.vel * base.firstChunk.mass);

        return false;
    }

    private void LodgeInCreature(SharedPhysics.CollisionResult result, bool eu)
    {
        stuckInObject = result.obj;
        ChangeMode(Mode.StuckInCreature);

        if (result.chunk != null)
        {
            stuckInChunkIndex = result.chunk.index;
            if (throwDamageBonus > 0.9f && room.GetTile(room.GetTilePosition(stuckInChunk.pos) + throwDir).Terrain == Room.Tile.TerrainType.Solid && room.GetTile(stuckInChunk.pos).Terrain == Room.Tile.TerrainType.Air)
            {
                stuckRotation = Custom.VecToDeg(rotation);
            }
            base.firstChunk.MoveWithOtherObject(eu, stuckInChunk, new Vector2(0f, 0f));
        }
        else if (result.onAppendagePos != null)
        {
            stuckInChunkIndex = 0;
            stuckInAppendage = result.onAppendagePos;
            stuckRotation = Custom.VecToDeg(rotation) - Custom.VecToDeg(stuckInAppendage.appendage.OnAppendageDirection(stuckInAppendage));
        }
        if (room.BeingViewed)
        {
            for (int i = 0; i < 8; i++)
            {
                room.AddObject(new WaterDrip(result.collisionPoint, -base.firstChunk.vel * UnityEngine.Random.value * 0.5f + Custom.DegToVec(360f * UnityEngine.Random.value) * base.firstChunk.vel.magnitude * UnityEngine.Random.value * 0.5f, waterColor: false));
            }
        }
    }

    public void PulledOutOfStuckObject()
    {
        for (int i = 0; i < abstractPhysicalObject.stuckObjects.Count; i++)
        {
            if (abstractPhysicalObject.stuckObjects[i] is AbstractPhysicalObject.AbstractSpearStick && (abstractPhysicalObject.stuckObjects[i] as AbstractPhysicalObject.AbstractSpearStick).Spear == abstractPhysicalObject)
            {
                abstractPhysicalObject.stuckObjects[i].Deactivate();
                break;
            }
            if (abstractPhysicalObject.stuckObjects[i] is AbstractPhysicalObject.AbstractSpearAppendageStick && (abstractPhysicalObject.stuckObjects[i] as AbstractPhysicalObject.AbstractSpearAppendageStick).Spear == abstractPhysicalObject)
            {
                abstractPhysicalObject.stuckObjects[i].Deactivate();
                break;
            }
        }
        stuckInObject = null;
        stuckInAppendage = null;
        stuckInChunkIndex = 0;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);

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
            //-- MR7: The pitch for throwing knives is much smaller than spears, so it communicates itself as a knife and less like a spear.
            soundLoop.Pitch = 1.5f;
        }
        soundLoop.Update();

        if (base.mode == Mode.Carried)
        {
            var grasp = grabbedBy[0];
            var graspChunk = grasp.grabbedChunk;

            var grabber = grasp.grabber;
            var grabberBodyChunk = grabber.mainBodyChunk;

            faceDirection = Mathf.Sign(graspChunk.pos.x - grabberBodyChunk.pos.x);
            //-- MR7: Rotation when carried follows the grabbers body to the grasped chunk,
            // as to be sraight as if down the arm and to the wrist.
            rotation = (grabberBodyChunk.pos - graspChunk.pos).normalized;
            // Rotated slightly to orient on the hand.
            rotation += Custom.DegToVec(80 * faceDirection);
        }
        else if (base.mode == Mode.StuckInCreature)
        {
            if (stuckInAppendage != null)
            {
                setRotation = Custom.DegToVec(stuckRotation + Custom.VecToDeg(stuckInAppendage.appendage.OnAppendageDirection(stuckInAppendage)));
                base.firstChunk.pos = stuckInAppendage.appendage.OnAppendagePosition(stuckInAppendage);
            }
            else
            {
                base.firstChunk.vel = stuckInChunk.vel;
                if (!room.BeingViewed)
                {
                    setRotation = Custom.DegToVec(stuckRotation + Custom.VecToDeg(stuckInChunk.Rotation));
                    base.firstChunk.MoveWithOtherObject(eu, stuckInChunk, new Vector2(0f, 0f));
                }
            }
            if (stuckInChunk.owner.slatedForDeletetion)
            {
                ChangeMode(Mode.Free);
            }
        }
        else
        {
            // Stop spinning when touching surface
            if (firstChunk.ContactPoint != new IntVector2(0, 0))
            {
                rotationSpeed = 0;
            }
        }

        if (setRotation != null)
        {
            rotation = setRotation.Value;
            setRotation = null;
        }
        lastRotation = rotation;
    }

    //
    // IDRAWABLES
    //

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = new FSprite("knifeBlade", true);
        sLeaser.sprites[0].anchorY -= 0.2f;
        sLeaser.sprites[1] = new FSprite("knifeHandle", true);
        sLeaser.sprites[1].anchorY -= 0.2f;
        AddToContainer(sLeaser, rCam, null);
    }

    // TODO: fix this visually to act more like a tiny spear, copy whatever code is used for the spear.
    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);

        // Reposition sprite to fit more naturally on hand.
        if (base.mode == Mode.Carried)
        {
            pos.x += 2f * faceDirection;
            pos.y += 3f;
        }

        // Loop through and update the sprites.
        Vector3 rot3 = Vector3.Slerp(lastRotation, rotation, timeStacker);
        float finalRotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), rot3);

        float spriteScaleX = faceDirection;

        for (int i = 0; i < TotalSprites; i++)
        {
            sLeaser.sprites[i].x = pos.x - camPos.x;
            sLeaser.sprites[i].y = pos.y - camPos.y;
            sLeaser.sprites[i].rotation = finalRotation;
            sLeaser.sprites[i].scaleX = spriteScaleX;
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