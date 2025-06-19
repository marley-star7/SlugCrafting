using RWCustom;
using UnityEngine;

namespace SlugCrafting.Items;

class LizardShell : PlayerCarryableItem, IDrawable
{
    public override float ThrowPowerFactor => 1f;

    private int whiteFlicker = 0;
    private int flicker;
    private float flickerColor = 0;

    public const int SourceCodeLizardsFlickerThreshold = 10;
    public const int SourceCodeLizardsWhiteFlickerThreshold = 15;

    public RoomPalette palette;

    public const int TotalSprites = 3;
    public const int SpriteJawStart = 1;

    public string[] HeadSprites;

    public Vector2 rotation;
    public Vector2 lastRotation;

    public float jawRotation;
    public float lastJawRotation;

    public const float MaxJawRotation = 100f;
    public const float JawOpenSensitivity = 20f;
    public const float JawVelocityOverOpenSensitivity = 2.5f;

    public readonly AbstractLizardShell abstractLizardShell;
    public LizardShell(AbstractLizardShell abstractPhysicalObject)
        : base(abstractPhysicalObject)
    {
        abstractLizardShell = abstractPhysicalObject;

        var pos = abstractPhysicalObject.Room.realizedRoom.MiddleOfTile(abstractPhysicalObject.pos.Tile);

        base.bodyChunks = new[] {
                new BodyChunk(this, 0, pos, abstractLizardShell.headBodyChunkRadius, abstractLizardShell.headBodyChunkMass),
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

        lastJawRotation = 0f;
        jawRotation = 0f;

        // Initialize HeadSprites to avoid nullability issues
        HeadSprites = new string[TotalSprites];
    }
    private static float Rand => UnityEngine.Random.value;
    public void HitEffect(Vector2 impactVelocity)
    {
        var num = UnityEngine.Random.Range(3, 8);
        for (int k = 0; k < num; k++)
        {
            //-- MR7: Figure out how to make sparks have the lizard graphics thing where they change color, without NEEDING lizard graphics.
            Vector2 pos = firstChunk.pos + Custom.DegToVec(Rand * 360f) * 5f * Rand;
            Vector2 vel = -impactVelocity * -0.1f + Custom.DegToVec(Rand * 360f) * Mathf.Lerp(0.2f, 0.4f, Rand) * impactVelocity.magnitude;
            room.AddObject(new Spark(pos, vel, new Color(1f, 1f, 1f), null, 10, 170));
        }

        room.AddObject(new StationaryEffect(firstChunk.pos, new Color(1f, 1f, 1f), null, StationaryEffect.EffectType.FlashingOrb));
    }

    public void Shatter()
    {
        room.PlaySound(SoundID.Spear_Fragment_Bounce, firstChunk.pos, 0.35f, 2f);
        Destroy();
    }

    public override void PickedUp(Creature upPicker)
    {
        room.PlaySound(SoundID.Spear_Fragment_Bounce, firstChunk);
    }

    public override void HitByWeapon(Weapon weapon)
    {
        base.HitByWeapon(weapon);

        AddDamage(weapon.HeavyWeapon ? 0.5f : 0.2f);
        WhiteFlicker(20);
        Flicker(30);

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
        base.Update(eu);

        var chunk = firstChunk;

        if (flicker > 0)
            flicker--;
        if (whiteFlicker > 0)
            whiteFlicker--;

        // TODO: Get this scraping sound working damnit!
        /*
        if (IsTileSolid(0, 0, firstChunk.contactPoint.y) && Math.Abs(firstChunk.vel.x) > 0.1f)
        {
            room.PlaySound(SoundID.Lizard_Belly_Drag_LOOP, firstChunk.pos, 2f, 2f);
        }
        */

        if (grabbedBy.Count > 0)
        {
            var grabber = grabbedBy[0].grabber;

            // CUSTOM FUNNY MIMICK LIZARD SLUGCAT
            if (grabber is Player && (grabber as Player).Sneak > 0.01f)
            {
                // If the player is sneaking, we want to make sure the shell is facing the same direction as the player.
                var scug = grabber as Player;

                float tillFullCrouch = Mathf.InverseLerp(0.5f, 0, scug.Sneak);
                float tillFullUncrouch = Mathf.InverseLerp(0, 0.5f, scug.Sneak);
                Vector2 scugAimDir = new Vector2(scug.ThrowDirection, 0);

                if (scug.Sneak < 0.5f)
                {
                    Vector2 faceDir = new Vector2(scug.ThrowDirection, 0);

                    bodyChunks[0].pos = Vector2.Lerp(bodyChunks[0].pos, bodyChunks[0].pos + faceDir * 15, 0.3f); // Move into position.
                }
                bodyChunks[0].vel = Vector2.Lerp(bodyChunks[0].vel, Vector2.zero, 0.3f); // Move velocity to zero to prevent interference with da looks.
                // Added extra position when determining rotation based off throw direction to ensure rotation is correct,
                rotation = Vector2.Lerp(rotation, Custom.DirVec(grabber.mainBodyChunk.pos + scugAimDir * 2, chunk.pos), 0.3f);
            }
            else
            {
                // TODO: make this work with rotation in front and behind the player.
                rotation = Custom.PerpendicularVector(Custom.DirVec(chunk.pos, grabber.mainBodyChunk.pos));
                rotation.y = Mathf.Abs(rotation.y);
            }
        }
        else
        {
            rotation += 0.9f * Custom.DirVec(chunk.lastPos, chunk.pos) * Custom.Dist(chunk.lastPos, chunk.pos);
        }

        //-- MR7: Lowkey I stole this from Fisobs CentiShields lol...
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

        lastRotation = rotation;
    }

    //
    // SPRITES
    //

    public Color effectColor
    {
        get
        {
            if (abstractLizardShell.templateType == CreatureTemplate.Type.BlackLizard)
            {
                return palette.blackColor;
            }
            return abstractLizardShell.shellColor;
        }
    }

    private Color HeadColor1
    {
        get
        {
        /*
        if (abstractLizardShell.templateType == CreatureTemplate.Type.WhiteLizard)
        {
            return Color.Lerp(new Color(1f, 1f, 1f), whiteCamoColor, whiteCamoColorAmount);
        }
        if (abstractLizardShell.templateType == CreatureTemplate.Type.Salamander)
        {
            return SalamanderColor;
        }
        if (snowAccCosmetic != null)
        {
            return Color.Lerp(palette.blackColor, effectColor, Mathf.Min(1f, snowAccCosmetic.DebrisSaturation * 1.5f));
        }
        */
        return palette.blackColor;
        }
    }

    private Color HeadColor2
    {
        get
        {
            /*
            if abstractLizardShell.templateType == CreatureTemplate.Type.WhiteLizard)
            {
                return Color.Lerp(palette.blackColor, whiteCamoColor, whiteCamoColorAmount);
            }
            */
            return effectColor;
        }
    }

    public Color HeadColor(float timeStacker)
    {
        if (whiteFlicker > SourceCodeLizardsWhiteFlickerThreshold)
        {
            return new Color(1f, 1f, 1f);
        }
        float a = Mathf.InverseLerp(0, abstractLizardShell.clampedHealth, abstractLizardShell.health);
        if (flicker > SourceCodeLizardsFlickerThreshold)
        {
            a = flickerColor;
        }
        return Color.Lerp(HeadColor1, HeadColor2, a);
    }

    public void Flicker(int fl)
    {
        if (fl > flicker)
            flicker = fl;
    }

    public void WhiteFlicker(int fl)
    {
        if (fl > whiteFlicker)
            whiteFlicker = fl;
    }

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
        Vector2 pos = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);
        Vector2 rot = Vector3.Slerp(lastRotation, rotation, timeStacker);

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
        float headRotation = Custom.VecToDeg(rot);

        float totalVel = Math.Abs(bodyChunks[0].vel.x) + Math.Abs(bodyChunks[0].vel.y);
        // Jaw opens the more the velocity is against the current rotation of the head, 90 degrees same velocity makes jaw shut, opposite angle entirely jaw is full open.
        float jawOpenRatio = Math.Abs(
            Mathf.Clamp(
                Vector2.Dot(rot, bodyChunks[0].vel)
                - 0.6f,
                -1 - totalVel * JawVelocityOverOpenSensitivity, // Jaw can open more if moving faster.
                0)
            );

        // Save previous jaw rotation for next frame.
        lastJawRotation = jawRotation;
        // Jaw has maximum amount it can open.
        float desiredJawRotation = -Mathf.Clamp(jawOpenRatio * JawOpenSensitivity, 0, MaxJawRotation);
        jawRotation = Mathf.Lerp(lastJawRotation, headRotation + desiredJawRotation, 0.25f);

        // Make sure the jaw doesn't rotate too far INSIDE of the head.
        if (jawRotation > headRotation + 10)
            jawRotation = headRotation + 10;

        // Make sure the rotation is between 0 and 360 degrees.
        headRotation %= 360f;
        jawRotation %= 360f;

        //
        // ACTUAL UPDATING THE SPRITES
        //

        //-- flicker code stolen from source.
        if (flicker > SourceCodeLizardsFlickerThreshold)
        {
            flickerColor = UnityEngine.Random.value;
        }

        var effectColor = HeadColor(timeStacker);
        // HEAD SPRITES UPDATE
        for (int i = SpriteJawStart; i < TotalSprites; i++)
        {
            sLeaser.sprites[i].element = Futile.atlasManager.GetElementWithName(HeadSprites[i].Insert(HeadSprites[i].Length - 2, headAngleNum));
            sLeaser.sprites[i].x = pos.x - camPos.x;
            sLeaser.sprites[i].y = pos.y - camPos.y;
            sLeaser.sprites[i].rotation = headRotation;
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
            sLeaser.sprites[i].rotation = jawRotation;
            sLeaser.sprites[i].color = HeadColor(timeStacker);
        }

        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        this.palette = palette;
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
