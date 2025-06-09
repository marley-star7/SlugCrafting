using RWCustom;
using UnityEngine;

using MRCustom;

namespace SlugCrafting.Items;

class LizardHide : PlayerCarryableItem, IDrawable
{
    public float rotation;
    public float lastRotation;

    public int TotalSprites;
    public int hideSprite;

    public MRCord mRCord;

    public bool lastGrabbed;

    private Vector2[,] hidePointsPos;
    private Vector2[,] hidePointsLastPos;

    private Color hideColor;

    private int divs;
    private float hideScale;

    public override float ThrowPowerFactor => .7f;
    public LizardHide(AbstractLizardHide abstractLizardLeather, Vector2 pos) 
        : base(abstractLizardLeather)
    {
        base.bodyChunks = new[] {
            new BodyChunk(this, 0, pos, 3f, 0.11f),
        };
        bodyChunkConnections = new BodyChunkConnection[0];

        divs = 7;
        hideScale = 8;

        TotalSprites = 1;
        hideSprite = 0;

        mRCord = new MRCord(this, firstChunk, divs)
        {
            partLength = hideScale,
            restSpeed = 0.3f,
            midPart = divs / 2,
        };
        mRCord.SetPartsRadius(divs);

        hidePointsPos = new Vector2[divs, divs];
        hidePointsLastPos = new Vector2[divs, divs];
        Reset();

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

        //movementDir -= new Vector2(0f, 9.8f); // TODO: replace with whatever the global gravity thing is or whatever decides movement pully.

        mRCord.Update();
        // VERTEX POINT PLACEMENTS
        for (int i = 0; i < divs; i++)
        {
            for (int j = 0; j < divs; j++)
            {
                //- MR7: Set hide points to form a simple rectangle, and then use 
                hidePointsPos[i, j] = new Vector2
                (
                    //-- // Centered around the first chunk and scaled.
                    (i - divs / 2) * hideScale
                    + mRCord.parts[i].pos.x,

                    (j - divs / 2) * hideScale
                    + mRCord.parts[j].pos.y
                );
                hidePointsLastPos[i, j] = hidePointsPos[i, j];
            }
        }
    }

    //
    // GRAPHICS STUFF
    //

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[TotalSprites];
        // Get random 1 of 3 different variants for the hide texture.
        var textureVariant = UnityEngine.Random.Range(0, 3);
        sLeaser.sprites[hideSprite] = TriangleMesh.MakeGridMesh("lizardLeather" + textureVariant.ToString(), divs - 1);

        var triMesh = sLeaser.sprites[hideSprite] as TriangleMesh;
        for (int i = 0; i < triMesh.verticeColors.Length; i++)
        {
            triMesh.verticeColors[i] = hideColor;
        }

        AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        var triMesh = sLeaser.sprites[hideSprite] as TriangleMesh;

        Vector2 startingStalksChangeInPos = Vector2.Lerp(mRCord.parts[0].lastPos, mRCord.parts[0].pos, timeStacker);
        startingStalksChangeInPos += Custom.DirVec(Vector2.Lerp(mRCord.parts[1].lastPos, mRCord.parts[1].pos, timeStacker), startingStalksChangeInPos) * mRCord.partLength;

        for (int i = 0; i < mRCord.parts.Length; i++)
        {
            for (int j = 0; j < divs; j++)
            {
                Vector2 currentStalkPos = Vector2.Lerp(hidePointsLastPos[i, j], hidePointsPos[i, j], timeStacker);
                Vector2 normalized = (currentStalkPos - startingStalksChangeInPos).normalized;
                Vector2 currentStalkPerpindicularAngle = Custom.PerpendicularVector(normalized);
                float distanceFromFirstStalk = Vector2.Distance(currentStalkPos, startingStalksChangeInPos) / 5f;

                if (i * divs + j < triMesh.vertices.Length - 1)
                {
                    if (i == 0)
                    {
                        triMesh.MoveVertice(i * divs + j, startingStalksChangeInPos - currentStalkPerpindicularAngle * hideScale - camPos);
                        triMesh.MoveVertice(i * divs + j + 1, startingStalksChangeInPos + currentStalkPerpindicularAngle * hideScale - camPos);
                    }
                    else
                    {
                        triMesh.MoveVertice(i * divs + j, startingStalksChangeInPos - currentStalkPerpindicularAngle * hideScale + normalized * distanceFromFirstStalk - camPos);
                        triMesh.MoveVertice(i * divs + j + 1, startingStalksChangeInPos + currentStalkPerpindicularAngle * hideScale + normalized * distanceFromFirstStalk - camPos);
                    }
                    triMesh.MoveVertice(i * divs + j, currentStalkPos - currentStalkPerpindicularAngle * hideScale - normalized * distanceFromFirstStalk - camPos);
                    triMesh.MoveVertice(i * divs + j + 1, currentStalkPos + currentStalkPerpindicularAngle * hideScale - normalized * distanceFromFirstStalk - camPos);
                }
            }
        }

        /*
        for (int i = 0; i < divs; i++)
        {
            for (int j = 0; j < divs; j++)
            {
                var posThisDraw = Vector2.Lerp(hidePointsPos[i, j], hidePointsLastPos[i, j], timeStacker);
                triMesh.MoveVertice(i * divs + j, posThisDraw - camPos);
            }
        }

        // Loop through and update the sprites.
        for (int i = 0; i < TotalSprites; i++)
        {
            sLeaser.sprites[i].rotation = 0;
        }
        */

        if (base.slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        hideColor = palette.blackColor;
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

    private void Reset()
    {
        mRCord.ResetParts();
        for (int i = 0; i < divs; i++)
        {
            for (int j = 0; j < divs; j++)
            {
                hidePointsPos[i, j] = base.bodyChunks[0].pos;
                hidePointsPos[i, j] = base.bodyChunks[0].pos;
            }
        }
    }
}
