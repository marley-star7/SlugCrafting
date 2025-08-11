using MRCustom;
using RWCustom;
using System;
using UnityEngine;

namespace SlugCrafting.Items;

public class CordItem : PlayerCarryableItem, IDrawable
{
    public CordProperties properties;

    public AbstractCord abstractCord;

    public MRLinePhysics mRLinePhysics;
    public Rope rope;

    public enum Mode
    {
        Free,
        Grabbed,
        FirstEndTied,
        FirstEndTiedAndSecondEndGrabbed,
        BothEndsTied,
    }

    private Mode _mode = Mode.Free;
    public Mode mode { get => _mode; }

    /// <summary>
    /// The total length of the string, how far of a distance an object can have from the string.
    /// </summary>
    public float totalCordLength = 200f;

    public int cordSprite => 0;
    public const int totalSprites = 1;
    /// <summary>
    /// The total amount of stalks used in the calculation of the string's length and appearance.
    /// More of these makes for a bendier more realistic string, but costs more to compute.
    /// </summary>
    public const int totalCordGraphicsParts = 20;
    /// <summary>
    /// The visual length of a stalk, used to make the stalk longer without a performance hit.
    /// </summary>
    public const float cordGraphicsPartLength = 4f;

    public int[] cordEndsMRLinePhysicsPartIndexes = new int[] {totalCordGraphicsParts - 2, 1};
    public int midPart => mRLinePhysics.midPart;

    /// <summary>
    /// How fast the stalk will settle and surcumb to gravity.
    /// </summary>
    public const float cordGraphicsRestSpeed = 0.5f;

    public float swallowed;

    public Color cordColor;

    public CordItem(AbstractCord abstractCord, CordProperties properties) : base(abstractCord)
    {
        this.abstractCord = abstractCord;
        var pos = abstractPhysicalObject.Room.realizedRoom.MiddleOfTile(abstractPhysicalObject.pos.Tile);

        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, pos, 5f, 0.07f);
        base.bodyChunkConnections = new BodyChunkConnection[0];

        base.airFriction = 0.991f;
        base.gravity = 0.9f;
        base.bounce = 0f;
        base.surfaceFriction = 0.3f;
        base.collisionLayer = 2;
        base.waterFriction = 0.92f;
        base.buoyancy = 1.2f;

        mRLinePhysics = new MRLinePhysics(this, firstChunk, totalCordGraphicsParts)
        {
            partLength = cordGraphicsPartLength,
            restSpeed = cordGraphicsRestSpeed,
            midPart = totalCordGraphicsParts / 2,
        };
        mRLinePhysics.SetPartsRadius(properties.thickness);
        this.properties = properties;
    }

    public override void PlaceInRoom(Room placeRoom)
    {
        base.PlaceInRoom(placeRoom);
        mRLinePhysics.ResetParts();
    }

    public override void NewRoom(Room newRoom)
    {
        base.NewRoom(newRoom);
        mRLinePhysics.ResetParts();
    }

    public void ChangeMode(Mode newMode)
    {
        if (_mode == newMode)
            return;

        if (newMode != Mode.BothEndsTied)
        {
            firstChunk.collideWithTerrain = false;
            firstChunk.collideWithObjects = false;
        }
        else
        {
            firstChunk.collideWithTerrain = true;
            firstChunk.collideWithObjects = true;
        }

        if (newMode != Mode.FirstEndTiedAndSecondEndGrabbed && newMode != Mode.BothEndsTied)
        {
            mRLinePhysics.forceSetPartPositions.Remove(cordEndsMRLinePhysicsPartIndexes[1]);
        }

        _mode = newMode;
    }

    public void TieObject(AbstractPhysicalObject objectToTie, int tiedChunkIndex, int tiePosition)
    {
        if (objectToTie == null)
            return;

        UntieAllObjects();

        abstractCord.tiedObjects[tiePosition] = objectToTie;
        abstractCord.tiedObjectBodyChunkIndexes[tiePosition] = tiedChunkIndex;

        if (objectToTie.realizedObject != null)
        {
            Array.Resize(ref bodyChunkConnections, bodyChunkConnections.Length + 1);
            // MS7: Looking at source code, using -1 weight symmetry has the calculations go based off the chunks mass, so cool!
            bodyChunkConnections[bodyChunkConnections.Length - 1] = (new BodyChunkConnection(this.firstChunk, objectToTie.realizedObject.bodyChunks[tiedChunkIndex], totalCordLength, type: BodyChunkConnection.Type.Pull, 0.7f, -1));
        }

        objectToTie.GetAbstractPhysicalObjectCraftingData().tiedCord = abstractCord;

        mRLinePhysics.midPart = 0;

        if (tiePosition == 0)
            ChangeMode(Mode.FirstEndTied);
        else
            ChangeMode(Mode.BothEndsTied);
    }

    public void UntieObject(int tiePosition)
    {
        var tiedObject = abstractCord.tiedObjects[tiePosition];

        if (tiedObject == null)
            return;

        tiedObject.GetAbstractPhysicalObjectCraftingData().tiedCord = null;

        if (tiedObject.realizedObject != null)
        {
            // Remove from the array the tiePos
            bodyChunkConnections = bodyChunkConnections.Where((source, index) => index != tiePosition).ToArray();
        }

        abstractCord.tiedObjects[tiePosition] = null;
        abstractCord.tiedObjectBodyChunkIndexes[tiePosition] = -1;
        mRLinePhysics.forceSetPartPositions.Remove(tiePosition);

        if (tiePosition == 1)
            ChangeMode(Mode.Free);
    }

    public void UntieAllObjects()
    {
        bodyChunkConnections = new BodyChunkConnection[0];

        for (int i = 0; i < abstractCord.tiedObjects.Length; i++)
            UntieObject(i);

        // Reset midpart to center of string.
        mRLinePhysics.midPart = totalCordGraphicsParts / 2;
    }

    private void TugOnGrabber()
    {
        if (grabbedBy.Count > 0 && grabbedBy[0].grabber != null)
        {
            bodyChunkConnections[0].TugOnChunk(grabbedBy[0].grabber.firstChunk);
        }
    }

    private void DislodgeTiedSpearFromWall(Spear tiedSpear)
    {
        if (tiedSpear.stuckInWall.HasValue)
        {
            tiedSpear.room.PlaySound(SoundID.Spear_Stick_In_Ground, tiedSpear.firstChunk.pos, 1.8f, Random.Range(1.1f, 1.5f));
            tiedSpear.ChangeMode(Spear.Mode.Free);
        }
    }

    public override void Update(bool eu)
    {
        base.Update(eu);

        if (abstractCord.tiedObjects[0] == null || abstractCord.tiedObjects[0].realizedObject == null)
        {
            ChangeMode(Mode.Free);
        }
        else
        {
            if (grabbedBy.Count > 0)
            {
                ChangeMode(Mode.FirstEndTiedAndSecondEndGrabbed);

                mRLinePhysics.forceSetPartPositions[cordEndsMRLinePhysicsPartIndexes[1]] = firstChunk.pos;

                if (bodyChunkConnections.Length > 0)
                {
                    TugOnGrabber();
                }

                if (abstractCord.tiedObjects[0].realizedObject is Spear tiedSpear)
                {
                    float distBetweenTiedChunks = Vector2.Distance(bodyChunkConnections[0].chunk1.pos, bodyChunkConnections[0].chunk2.pos);
                    if (tiedSpear.stuckInWall != null)
                    {
                        var tiedSpearThrowDirVec2 = Custom.IntVector2ToVector2(tiedSpear.throwDir);

                        if (distBetweenTiedChunks > bodyChunkConnections[0].distance)
                        {
                            if (tiedSpear.stuckInWall.HasValue)
                            {
                                // TODO: add creaking sound loop.

                                var extraDistancePastConnectionDistance = distBetweenTiedChunks - bodyChunkConnections[0].distance;
                                tiedSpear.vibrate = (int)(extraDistancePastConnectionDistance * 0.1f);

                                var distancePastCordConnectionDistanceForDislodge = tiedSpear.GetSpearCraftingData().distancePastCordConnectionDistanceForDislodge;

                                // Ms7: Visual rotation pulling of string to show it's gon SNAP
                                var dirVecStringPulling = Custom.DirVec(tiedSpear.firstChunk.pos, firstChunk.pos);
                                var stringPullingRotationInfluence = Mathf.InverseLerp(0, distancePastCordConnectionDistanceForDislodge, extraDistancePastConnectionDistance) * 0.3f;

                                tiedSpear.rotation = tiedSpearThrowDirVec2 - dirVecStringPulling * stringPullingRotationInfluence;

                                if (extraDistancePastConnectionDistance > distancePastCordConnectionDistanceForDislodge)
                                {
                                    DislodgeTiedSpearFromWall(tiedSpear);
                                }
                                else
                                {
                                    tiedSpear.firstChunk.pos = tiedSpear.stuckInWall.Value;
                                }
                            }
                        }
                        else
                        {
                            tiedSpear.rotation = tiedSpearThrowDirVec2;
                        }
                    }
                    else if (tiedSpear.mode == Spear.Mode.Thrown
                        || (tiedSpear.mode == Spear.Mode.Free && tiedSpear.firstChunk.ContactPoint == IntVector2Extensions.zero))
                    {
                        // MS7: Only limit rotation if above the connection distance, so the communication of a spear hitting max range spinning out still exists.
                        if (distBetweenTiedChunks > bodyChunkConnections[0].distance)
                        {
                            tiedSpear.rotationSpeed = 0;
                            tiedSpear.rotation = Custom.DirVec(firstChunk.pos, tiedSpear.firstChunk.pos);
                        }
                    }
                }
            }
            else
            {
                ChangeMode(Mode.FirstEndTied);
            }
        }

        for (int i = 0; i < abstractCord.tiedObjects.Length; i++)
        {
            var currentTiedObject = abstractCord.tiedObjects[i];

            if (currentTiedObject == null || currentTiedObject.realizedObject == null)
                continue;

            if (currentTiedObject.realizedObject.slatedForDeletetion)
                UntieObject(i);

            mRLinePhysics.forceSetPartPositions[cordEndsMRLinePhysicsPartIndexes[i]] = currentTiedObject.realizedObject.bodyChunks[abstractCord.tiedObjectBodyChunkIndexes[i]].pos;
        }

        mRLinePhysics.Update();
    }

    //
    // IDRAWABLE INTERFACE
    //

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[totalSprites]
        {
            TriangleMesh.MakeLongMesh(mRLinePhysics.parts.Length, pointyTip: false, customColor: true)
        };
        AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        var triMesh = sLeaser.sprites[cordSprite] as TriangleMesh;

        Vector2 startingStalksChangeInPos = Vector2.Lerp(mRLinePhysics.parts[0].lastPos, mRLinePhysics.parts[0].pos, timeStacker);
        startingStalksChangeInPos += Custom.DirVec(Vector2.Lerp(mRLinePhysics.parts[1].lastPos, mRLinePhysics.parts[1].pos, timeStacker), startingStalksChangeInPos) * cordGraphicsPartLength;
        for (int i = 0; i < mRLinePhysics.parts.Length; i++)
        {
            Vector2 currentStalkPos = Vector2.Lerp(mRLinePhysics.parts[i].lastPos, mRLinePhysics.parts[i].pos, timeStacker);
            Vector2 normalized = (currentStalkPos - startingStalksChangeInPos).normalized;
            Vector2 currentStalkPerpindicularAngle = Custom.PerpendicularVector(normalized);
            float distanceFromFirstStalk = Vector2.Distance(currentStalkPos, startingStalksChangeInPos) / 5f;
            if (i == 0)
            {
                triMesh.MoveVertice(i * 4, startingStalksChangeInPos - currentStalkPerpindicularAngle * properties.thickness - camPos);
                triMesh.MoveVertice(i * 4 + 1, startingStalksChangeInPos + currentStalkPerpindicularAngle * properties.thickness - camPos);
            }
            else
            {
                triMesh.MoveVertice(i * 4, startingStalksChangeInPos - currentStalkPerpindicularAngle * properties.thickness + normalized * distanceFromFirstStalk - camPos);
                triMesh.MoveVertice(i * 4 + 1, startingStalksChangeInPos + currentStalkPerpindicularAngle * properties.thickness + normalized * distanceFromFirstStalk - camPos);
            }
            triMesh.MoveVertice(i * 4 + 2, currentStalkPos - currentStalkPerpindicularAngle * properties.thickness - normalized * distanceFromFirstStalk - camPos);
            triMesh.MoveVertice(i * 4 + 3, currentStalkPos + currentStalkPerpindicularAngle * properties.thickness - normalized * distanceFromFirstStalk - camPos);
            startingStalksChangeInPos = currentStalkPos;
        }

        if (base.slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }

        // TODO: get blink working, it's not for some reason.
        if (blink > 0)
        {
            UpdateColor(sLeaser, blink > 4 && UnityEngine.Random.value < 0.5f);
        }
        else if (sLeaser.sprites[cordSprite].color == base.blinkColor)
        {
            UpdateColor(sLeaser, blink: false);
        }
    }

    public void UpdateColor(RoomCamera.SpriteLeaser sLeaser, bool blink)
    {
        Color newColor;
        if (blink)
            newColor = base.blinkColor;
        else
            newColor = cordColor;

        for (int j = 0; j < ((TriangleMesh)sLeaser.sprites[cordSprite]).verticeColors.Length; j++)
        {
            ((TriangleMesh)sLeaser.sprites[cordSprite]).verticeColors[j] = newColor;
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        cordColor = palette.blackColor;
        UpdateColor(sLeaser, false);
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        if (newContatiner == null)
        {
            newContatiner = rCam.ReturnFContainer("Items");
        }
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].RemoveFromContainer();
            newContatiner.AddChild(sLeaser.sprites[i]);
        }
    }
}
