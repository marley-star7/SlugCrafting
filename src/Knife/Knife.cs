using RWCustom;
using UnityEngine;

namespace SlugCrafting.Items;

sealed class Knife : Weapon
{
    private const float SPRITE_DIRECTION_BLEND_TIME = 0.5f;
    private const float ROTATION_BLEND_TIME = 0.35f;
    private const float Y_DIFF_ROTATION_MODIFIER = 2f;

    private static float Rand => UnityEngine.Random.value;

    private int flipped = 1;

    new private float rotation;
    new private float lastRotation;

    public float rotVel;
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

    private readonly float rotationOffset = 110;
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

        rotation = Rand * 360f;
    }
    public override void Update(bool eu)
    {
        ChangeCollisionLayer(grabbedBy.Count == 0 ? 2 : 1);
        firstChunk.collideWithTerrain = grabbedBy.Count == 0;
        firstChunk.collideWithSlopes = grabbedBy.Count == 0;

        base.Update(eu);

        if (grabbedBy.Count > 0)
        {
            var grasp = grabbedBy[0];
            var graspChunk = grasp.grabbedChunk;

            var grabber = grasp.grabber;
            var grabberChunk = grabber.mainBodyChunk;

            // Flip if the grabber is facing left.
            float newRotationOffset;
            if (graspChunk.pos.x < grabberChunk.pos.x)
            {
                flipped = -1;
                newRotationOffset = 180f - rotationOffset;
            }
            else
            {
                flipped = 1;
                newRotationOffset = rotationOffset;
            }

            faceDirection = Mathf.Lerp(faceDirection, flipped, SPRITE_DIRECTION_BLEND_TIME);

            var chunk = firstChunk;

            lastRotation = rotation;
            rotation = Mathf.Lerp(rotation, graspChunk.Rotation.GetAngle() + newRotationOffset, ROTATION_BLEND_TIME);

            // Add slight rotation to the knife based off vertical difference between the two chunks (to simulate where the wrist would be resting).
            var yDiff = graspChunk.pos.y - grabberChunk.pos.y;
            rotation -= yDiff * flipped * Y_DIFF_ROTATION_MODIFIER;

            //rotation += 0.9f * Vector2.Distance(chunk.lastPos, chunk.pos);
        }
    }

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
        Vector2 pos = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);

        // Reposition sprite to fit more naturally on hand.
        pos.x += 3.2f * faceDirection;
        pos.y += 3f;

        // Loop through and update the sprites.
        for (int i = 0; i < TotalSprites; i++)
        {
            sLeaser.sprites[i].x = pos.x - camPos.x;
            sLeaser.sprites[i].y = pos.y - camPos.y;
            sLeaser.sprites[i].rotation = Mathf.Lerp(lastRotation, rotation, timeStacker);
            sLeaser.sprites[i].scaleX = flipped;
        }

        sLeaser.sprites[0].color = bladeColor;
        sLeaser.sprites[1].color = handleColor;

        // TODO: need to look how lighting is normally calculated?

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