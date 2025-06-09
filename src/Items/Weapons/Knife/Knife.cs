using RWCustom;
using UnityEngine;

using MRCustom.Math;

namespace SlugCrafting.Items.Weapons;

// TODO: steal spear spragment texture to do for the basic knife :] (cuz its spear)

sealed class Knife : Weapon
{
    private const float SpriteDirectionBlendTime = 0.5f;
    private const float RotationBlendTime = 0.35f;
    private const float YDiffRotationModifier = 2f;
    private static float Rand => UnityEngine.Random.value;

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

            //
            // ROTATION X CALCULATION
            //

            // Flip if the grabber is facing left.
            float rotationXOffset = 0;
            if (graspChunk.pos.x < grabberChunk.pos.x)
                rotationXOffset = 180;

            var flipped = MarMathf.InverseLerpNegToPos(180, 0, rotationXOffset);

            //
            // ADDITIONAL COSMETIC Y ROTATION FOR MOVERY
            //

            // Add slight rotation to the knife based off vertical difference between the two chunks (to simulate where the wrist would be resting).
            var yDiff = graspChunk.pos.y - grabberChunk.pos.y;
            var addedRotY = -yDiff * YDiffRotationModifier * flipped;
            // Add the visual rotation to position in hand.
            addedRotY += GrabbedRotationYOffset * flipped;

            //
            // ACTUAL SETTAGE
            //

            rotation = Vector2.Lerp(rotation, graspChunk.Rotation + new Vector2(rotationXOffset, 0), RotationBlendTime);
            rotation.y += addedRotY;
            lastRotation = rotation;
        }
        if (setRotation != null)
        {
            rotation = setRotation.Value;
            setRotation = null;
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
        Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
        Vector2 rot = Vector3.Slerp(lastRotation, rotation, timeStacker);

        // TODO: The fact this does not account for over 180 degrees is a known bug, I'm literally too maff dumb rn to fix it
        float spriteScaleX = Mathf.Sign(MarMathf.InverseLerpNegToPos(180, 0, rotation.x));

        float spriteRotation = rot.y;

        // Reposition sprite to fit more naturally on hand.
        pos.x += 3.2f * faceDirection;
        pos.y += 3f;

        // Loop through and update the sprites.
        for (int i = 0; i < TotalSprites; i++)
        {
            sLeaser.sprites[i].x = pos.x - camPos.x;
            sLeaser.sprites[i].y = pos.y - camPos.y;
            sLeaser.sprites[i].rotation = spriteRotation;
            sLeaser.sprites[i].scaleX = spriteScaleX;
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