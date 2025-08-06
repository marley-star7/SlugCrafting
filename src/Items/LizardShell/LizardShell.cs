using RWCustom;
using SlugCrafting.Creatures;
using UnityEngine;

namespace SlugCrafting.Items;

// TODO: take jawOpenAngle, and jawOpenFacotr stuff from source.
// TODO: get pickup blink working.

class LizardShell : PlayerCarryableItem, IDrawable
{
    public override float ThrowPowerFactor => 1f;

    LizardShellColorGraphics lizardShellColorGraphics;

    public const int TotalSprites = 3;
    public const int SpriteJawStart = 1;

    public string[] HeadSprites;

    private bool flipX = false;

    public Vector2 rotation;
    public Vector2 lastRotation;

    public Vector2 jawRotation;
    public Vector2 lastJawRotation;

    private Vector2 rotVel;

    public float donned = 0;

    public float jawOpenRatio = 0;
    public const float jawOpenSensitivity = 20f;
    public const float jawVelocityOverOpenSensitivity = 2.5f;

    private bool facingRight;

    public readonly AbstractLizardShell abstractLizardShell;
    public LizardShell(AbstractLizardShell abstractPhysicalObject)
        : base(abstractPhysicalObject)
    {
        abstractLizardShell = abstractPhysicalObject;

        var pos = abstractPhysicalObject.Room.realizedRoom.MiddleOfTile(abstractPhysicalObject.pos.Tile);

        base.bodyChunks = new[] {
                new BodyChunk(this, 0, pos, abstractLizardShell.rad, abstractLizardShell.mass),
            };

        // Cloth is made up of 3 small chunks to fake physics.
        base.bodyChunkConnections = new BodyChunkConnection[0];

        base.airFriction = 0.97f;
        base.gravity = 0.9f;
        base.bounce = 0.1f;
        base.surfaceFriction = 0.45f;
        base.collisionLayer = 1;
        base.waterFriction = 0.92f;
        base.buoyancy = 0.75f;

        rotation = Vector2.zero;
        lastRotation = rotation;

        jawRotation = Vector2.zero;
        lastJawRotation = jawRotation;

        facingRight = abstractLizardShell.scaleX > 0;

        // Initialize HeadSprites to avoid nullability issues
        HeadSprites = new string[TotalSprites];
        lizardShellColorGraphics = new LizardShellColorGraphics(abstractLizardShell.shellColor);
    }

    private static float Rand => UnityEngine.Random.value;
    public void HitEffect(Vector2 impactVelocity)
    {
        var sparkColor = lizardShellColorGraphics.ShellColor(abstractLizardShell.health, abstractLizardShell.maxHealth);

        var num = UnityEngine.Random.Range(3, 8);
        for (int k = 0; k < num; k++)
        {
            //-- MS7: Figure out how to make sparks have the lizard graphics thing where they change color, without NEEDING lizard graphics.
            Vector2 pos = firstChunk.pos + Custom.DegToVec(Rand * 360f) * 5f * Rand;
            Vector2 vel = -impactVelocity * -0.1f + Custom.DegToVec(Rand * 360f) * Mathf.Lerp(0.2f, 0.4f, Rand) * impactVelocity.magnitude;
            room.AddObject(new Spark(pos, vel, sparkColor, null, 10, 170));
        }

        room.AddObject(new StationaryEffect(firstChunk.pos, new Color(1f, 1f, 1f), null, StationaryEffect.EffectType.FlashingOrb));
    }

    public void Shatter()
    {
        room.PlaySound(SoundID.Spear_Fragment_Bounce, firstChunk.pos, 0.35f, 2f);
        for (int k = 0; k < 6; k++)
        {
            room.AddObject(new LizardShellFragment(firstChunk.pos, Custom.RNV() * Mathf.Lerp(5f, 15f, UnityEngine.Random.value), lizardShellColorGraphics.ShellColor(abstractLizardShell.health, abstractLizardShell.maxHealth)));
        }
        Destroy();
    }

    public override void PickedUp(Creature upPicker)
    {
        room.PlaySound(SoundID.Lizard_Light_Terrain_Impact, firstChunk);
        lizardShellColorGraphics.Flicker(20);
    }

    public override void HitByWeapon(Weapon weapon)
    {
        base.HitByWeapon(weapon);

        AddDamage(weapon.HeavyWeapon ? 0.5f : 0.2f);
        lizardShellColorGraphics.WhiteFlicker(20);
        lizardShellColorGraphics.Flicker(30);

        if (grabbedBy.Count > 0)
        {
            Creature grabber = grabbedBy[0].grabber;
            Vector2 push = firstChunk.vel * firstChunk.mass / grabber.firstChunk.mass;
            grabber.firstChunk.vel += push;
        }

        firstChunk.vel = Vector2.zero;

        HitEffect(weapon.firstChunk.vel);
    }

    public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        base.TerrainImpact(chunk, direction, speed, firstContact);

        if (speed > 10)
        {
            room.PlaySound(SoundID.Spear_Fragment_Bounce, firstChunk);
            lizardShellColorGraphics.Flicker(20);
        }
    }

    public void AddDamage(float damage)
    {
        abstractLizardShell.health -= damage;

        if (abstractLizardShell.health <= 0)
            Shatter();
    }

    public override void Update(bool eu)
    {
        lastRotation = rotation;
        lastJawRotation = jawRotation;

        base.Update(eu);
        lizardShellColorGraphics.Update();

        var chunk = firstChunk;

        rotation = Custom.DegToVec(Custom.VecToDeg(rotation) + rotVel.x);
        jawRotation = Custom.DegToVec(Custom.VecToDeg(jawRotation) + rotVel.x);

        // Jaw opens the more the velocity is against the current rotation of the head, 90 degrees same velocity makes jaw shut, opposite angle entirely jaw is full open.
        jawOpenRatio = Math.Abs(
            Mathf.Clamp(
                Vector2.Dot(rotation, bodyChunks[0].vel)
                - 0.6f,
                -1 - chunk.vel.magnitude * abstractLizardShell.jawOpenAngle, // Jaw can open more if moving faster.
                0)
            );

        /*
        if (flipJaw && jawRotation > headRotation + 10)
        {
            jawRotation = headRotation + 10;
        }
        else if (!flipJaw && jawRotation < headRotation - 10)
        {
            jawRotation = headRotation - 10;
        }
        */

        // TODO: Get this scraping sound working damnit!
        /*
        if (IsTileSolid(0, 0, firstChunk.contactPoint.y) && Math.Abs(firstChunk.vel.x) > 0.1f)
        {
            room.PlaySound(SoundID.Lizard_Belly_Drag_LOOP, firstChunk.pos, 2f, 2f);
        }
        */

        rotVel = Vector2.ClampMagnitude(rotVel, 50f);
        rotVel *= Custom.LerpMap(rotVel.magnitude, 5f, 50f, 1f, 0.8f);

        facingRight = Custom.VecToDeg(rotation) > 0;

        bool flipJaw = abstractLizardShell.scaleX > 0 ^ flipX;

        var isDonned = 0f;

        if (grabbedBy.Count > 0)
        {
            var grabber = grabbedBy[0].grabber;

            if (grabber is Player scug && scug.privSneak > 0.5f)
            {
                Vector2 faceDir = Custom.DegToVec(Custom.AimFromOneVectorToAnother(scug.bodyChunks[1].pos, scug.bodyChunks[0].pos));

                isDonned = scug.privSneak;

                rotation = faceDir;

                if (faceDir.x > 0 == abstractLizardShell.scaleX > 0)
                {
                    flipX = true;
                }
                else
                {
                    flipX = false;
                }
            }
            else
            {
                rotation = abstractLizardShell.scaleX < 0 ? Custom.RotateAroundOrigo(Custom.PerpendicularVector(Custom.DirVec(chunk.pos, grabber.mainBodyChunk.pos)), 180) : Custom.PerpendicularVector(Custom.DirVec(chunk.pos, grabber.mainBodyChunk.pos));
                rotation.y = Mathf.Abs(rotation.y);

                flipX = false;
            }
        }
        else if (firstChunk.ContactPoint.y < 0)
        {
            Vector2 b;

            b = Custom.DegToVec(90f * (facingRight ? 1 : -1));

            rotation = Vector2.Lerp(rotation, b, UnityEngine.Random.value);
            rotVel *= UnityEngine.Random.value;
        }
        else if (Vector2.Distance(firstChunk.lastPos, firstChunk.pos) > 5f && rotVel.magnitude < 7f)
        {
            rotVel += Custom.RNV() * (Mathf.Lerp(7f, 25f, UnityEngine.Random.value) + firstChunk.vel.magnitude * 2f);
        }

        if (!Custom.DistLess(chunk.lastPos, chunk.pos, 3f) && room.GetTile(chunk.pos).Solid && !room.GetTile(chunk.lastPos).Solid)
        {
            var firstSolid = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(room, room.GetTilePosition(chunk.lastPos), room.GetTilePosition(chunk.pos));
            if (firstSolid != null)
            {
                FloatRect floatRect = Custom.RectCollision(chunk.pos, chunk.lastPos, room.TileRect(firstSolid.Value).Grow(2f));
                chunk.pos = floatRect.GetCorner(FloatRect.CornerLabel.D);

                if (floatRect.GetCorner(FloatRect.CornerLabel.B).x < 0f)
                {
                    chunk.vel.x = Mathf.Abs(chunk.vel.x) * 0.15f;
                }
                else if (floatRect.GetCorner(FloatRect.CornerLabel.B).x > 0f)
                {
                    chunk.vel.x = -Mathf.Abs(chunk.vel.x) * 0.15f;
                }
                else if (floatRect.GetCorner(FloatRect.CornerLabel.B).y < 0f)
                {
                    chunk.vel.y = Mathf.Abs(chunk.vel.y) * 0.15f;
                }
                else if (floatRect.GetCorner(FloatRect.CornerLabel.B).y > 0f)
                {
                    chunk.vel.y = -Mathf.Abs(chunk.vel.y) * 0.15f;
                }
            }
        }

        //-- MS7: Lowkey I stole this from Fisobs CentiShields lol...
        // It seems to be responsible for the ACTUAL collision causing the spear to bounce off, but haven't deciphered it well enough yet.
        if (!Custom.DistLess(chunk.lastPos, chunk.pos, 3f) && room.GetTile(chunk.pos).Solid && !room.GetTile(chunk.lastPos).Solid)
        {
            var firstSolid = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(room, room.GetTilePosition(chunk.lastPos), room.GetTilePosition(chunk.pos));
            if (firstSolid != null)
            {
                FloatRect floatRect = Custom.RectCollision(chunk.pos, chunk.lastPos, room.TileRect(firstSolid.Value).Grow(2f));
                chunk.pos = floatRect.GetCorner(FloatRect.CornerLabel.D);

                if (floatRect.GetCorner(FloatRect.CornerLabel.B).x < 0f)
                {
                    chunk.vel.x = Mathf.Abs(chunk.vel.x) * 0.15f;
                }
                else if (floatRect.GetCorner(FloatRect.CornerLabel.B).x > 0f)
                {
                    chunk.vel.x = -Mathf.Abs(chunk.vel.x) * 0.15f;
                }
                else if (floatRect.GetCorner(FloatRect.CornerLabel.B).y < 0f)
                {
                    chunk.vel.y = Mathf.Abs(chunk.vel.y) * 0.15f;
                }
                else if (floatRect.GetCorner(FloatRect.CornerLabel.B).y > 0f)
                {
                    chunk.vel.y = -Mathf.Abs(chunk.vel.y) * 0.15f;
                }
            }
        }

        donned = Custom.LerpAndTick(donned, isDonned, 0.11f, 0.033333335f);
    }

    //
    // SPRITES
    //

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        int length = abstractLizardShell.headSprite0Jaw.Length;
        char headAngleNum = abstractLizardShell.headSprite3Head[length - 3];

        sLeaser.sprites = new FSprite[TotalSprites]
        {
            new FSprite(abstractLizardShell.headSprite0Jaw, true),
            //new FSprite(abstractLizardShell.headSprite1LowerTeeth, true),
            //new FSprite(abstractLizardShell.headSprite2UpperTeeth, true),
            new FSprite(abstractLizardShell.headSprite3Head, true),
            new FSprite(abstractLizardShell.headSprite4Eyes, true),
        };

        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            // Set the color of the sprites
            sLeaser.sprites[i].color = abstractLizardShell.shellColor;

            // Save the head sprites with the angle number removed, for our maffs later.
            var headSpriteName = sLeaser.sprites[i].element.name;
            HeadSprites[i] = headSpriteName.Remove(headSpriteName.Length - 3, 1);
        }

        AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        lizardShellColorGraphics.DrawSpritesUpdate();

        Vector2 pos = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
        Vector2 rot = Vector3.Slerp(lastRotation, rotation, timeStacker);
        Vector2 jawRot = Vector3.Slerp(lastJawRotation, jawRotation, timeStacker);

        string headAngleNum = "0";
        if (Math.Abs(rotation.x) > 45f)
            headAngleNum = "1";
        else if (Math.Abs(rotation.x) > 90f)
            headAngleNum = "2";
        else
            headAngleNum = "0";

        //
        // ROTATION CALCULATION CODE
        //

        // Not sure what this "VecToDeg" is but it looks better than normal non-vector2 calculated rotations lol.
        float finalHeadRotation = Custom.VecToDeg(rot);
        float finalJawRotation = Custom.VecToDeg(jawRot);
                    // Jaw has maximum amount it can open.
        finalJawRotation = -Mathf.Clamp(jawOpenRatio * abstractLizardShell.jawOpenMoveJawsApart, 0, abstractLizardShell.jawOpenAngle); ;

        float totalVel = Math.Abs(bodyChunks[0].vel.x) + Math.Abs(bodyChunks[0].vel.y);

        // Make sure the rotation is between 0 and 360 degrees.
        finalHeadRotation %= 360f;
        finalJawRotation %= 360f;

        //
        // ACTUAL UPDATING THE SPRITES
        //

        var effectColor = lizardShellColorGraphics.ShellColor(abstractLizardShell.health, abstractLizardShell.maxHealth);
        // HEAD SPRITES UPDATE
        for (int i = SpriteJawStart; i < TotalSprites; i++)
        {
            sLeaser.sprites[i].element = Futile.atlasManager.GetElementWithName(HeadSprites[i].Insert(HeadSprites[i].Length - 2, headAngleNum));
            sLeaser.sprites[i].x = pos.x - camPos.x;
            sLeaser.sprites[i].y = pos.y - camPos.y;
            sLeaser.sprites[i].rotation = finalHeadRotation;
            sLeaser.sprites[i].color = effectColor;
        }

        // Eye sprite specifically is just a slightly darker version of shell to show the head is gone. X_X
        sLeaser.sprites[TotalSprites - 1].color = Color.Lerp(effectColor, new Color(0f, 0f, 0f), 0.5f);

        // JAW SPRITES UPDATE
        for (int i = 0; i < SpriteJawStart; i++)
        {
            sLeaser.sprites[i].element = Futile.atlasManager.GetElementWithName(HeadSprites[i].Insert(HeadSprites[i].Length - 2, headAngleNum));
            sLeaser.sprites[i].x = pos.x - camPos.x;
            sLeaser.sprites[i].y = pos.y - camPos.y;
            sLeaser.sprites[i].rotation = finalJawRotation;
            sLeaser.sprites[i].color = effectColor;
        }

        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        lizardShellColorGraphics.ApplyPalette(palette);
        // If teeth is enabled.
        //sLeaser.sprites[2].color = palette.blackColor;
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
