using System;
using RWCustom;
using UnityEngine;

namespace SlugCrafting.Items;

class LizardHide : PlayerCarryableItem, IDrawable
{
    public float rotation;
    public float lastRotation;

    public int TotalSprites;
    public int leatherSprite;

    public bool lastGrabbed;

    private Vector2[,,] leatherPoints;
    private Color leatherColor;

    private int divs;
    private float leatherScale;

    private float looseness;

    private Vector2 movementDir;

    private float movement;
    private float lastMovement;

    public override float ThrowPowerFactor => .7f;
    public LizardHide(AbstractLizardLeather abstractLizardLeather, Vector2 pos) 
        : base(abstractLizardLeather)
    {
        base.bodyChunks = new[] {
            new BodyChunk(this, 0, pos, 3f, 0.11f),
            new BodyChunk(this, 0, pos - new Vector2(3f, 0), 3f, 0.11f),
            new BodyChunk(this, 0, pos + new Vector2(3f, 0), 3f, 0.11f)
        };
        // Cloth is made up of 3 small chunks to fake physics.
        base.bodyChunkConnections = new BodyChunkConnection[] {
            new BodyChunkConnection(
                bodyChunks[0],
                bodyChunks[1],
                3f,
                BodyChunkConnection.Type.Normal,
                0.14f,
                0.3f
            ),
            new BodyChunkConnection(
                bodyChunks[1],
                bodyChunks[2],
                3f,
                BodyChunkConnection.Type.Normal,
                0.14f,
                0.3f
            )
        };

        divs = 7;
        leatherScale = 8;
        looseness = 2f;

        TotalSprites = 1;
        leatherSprite = 0;

        leatherPoints = new Vector2[divs, divs, 3];
        ResetVertices();

        base.airFriction = 0.97f;
        base.surfaceFriction = 0.45f;
        base.gravity = 0.9f;
        base.collisionLayer = 0;
        base.waterFriction = 0.92f;
        base.buoyancy = 0.75f;

        rotation = UnityEngine.Random.value * 360f;
        lastRotation = rotation;
    }
    public override void Update(bool eu)
    {
        base.Update(eu);

        // GRABBABILITY
        if (grabbedBy.Count > 0)
        {

        }
        else if (lastGrabbed)
        {
        }

        lastMovement = movement;
        movement = Mathf.InverseLerp(0f, 12f, Vector2.Distance(base.firstChunk.lastPos, base.firstChunk.pos));
        movementDir = Custom.DirVec(base.firstChunk.lastPos, base.firstChunk.pos);
        //movementDir -= new Vector2(0f, 9.8f); // TODO: replace with whatever the global gravity thing is or whatever decides movement pully.

        // VERTEX POINT PLACEMENTS
        for (int i = 0; i < divs; i++)
        {
            for (int j = 0; j < divs; j++)
            {
                // TODO: dude the fact I even managed to get something relatively presentable is an accomplishment, still gotta remake all of this later tho lol.
                float pointCenterDistModifier = (Mathf.InverseLerp(0, divs, i) + Mathf.InverseLerp(0, divs, j));
                pointCenterDistModifier -= 1f; // Center point is 0,0
                pointCenterDistModifier /= 1; // Flip it to be 0,1
                pointCenterDistModifier *= looseness;

                // Set leather points to form a simple rectangle
                leatherPoints[i, j, 1] = leatherPoints[i, j, 0];
                leatherPoints[i, j, 0] = new Vector2
                (
                    firstChunk.pos.x + (i - divs / 2) * leatherScale // Centered around the first chunk and scaled.
                    + firstChunk.vel.x * pointCenterDistModifier, // Further from center, more movement.

                    firstChunk.pos.y + (j - divs / 2) * leatherScale
                    + firstChunk.vel.y * pointCenterDistModifier
                );
                leatherPoints[i, j, 0] += movementDir * movement;
            }
        }
    }
    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[TotalSprites];
        // Get random 1 of 3 different variants for the leather texture.
        var textureVariant = UnityEngine.Random.Range(0, 3);
        sLeaser.sprites[leatherSprite] = TriangleMesh.MakeGridMesh("lizardLeather" + textureVariant.ToString(), divs - 1);

        var triMesh = sLeaser.sprites[leatherSprite] as TriangleMesh;
        for (int i = 0; i < triMesh.verticeColors.Length; i++)
        {
            triMesh.verticeColors[i] = leatherColor;
        }

        AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        var triMesh = sLeaser.sprites[leatherSprite] as TriangleMesh;

        for (int i = 0; i < divs; i++)
        {
            for (int j = 0; j < divs; j++)
            {
                triMesh.MoveVertice(i * divs + j, Vector2.Lerp(leatherPoints[i, j, 1], leatherPoints[i, j, 0], timeStacker) - camPos);
            }
        }

        // Loop through and update the sprites.
        for (int i = 0; i < TotalSprites; i++)
        {
            sLeaser.sprites[i].rotation = 0;
        }

            if (base.slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        leatherColor = palette.blackColor;
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

    private void ResetVertices()
    {
        for (int i = 0; i < divs; i++)
        {
            for (int j = 0; j < divs; j++)
            {
                leatherPoints[i, j, 1] = base.bodyChunks[0].pos;
                leatherPoints[i, j, 0] = base.bodyChunks[0].pos;
            }
        }
    }
}
