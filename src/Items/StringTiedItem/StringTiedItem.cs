using RWCustom;
using UnityEngine;

using MRCustom;

namespace SlugCrafting.Items;

public class StringTiedItem : PlayerCarryableItem, IDrawable
{
    public MRCord mRCord;
    public AbstractStringTiedItem abstractStringTiedItem;

    public float TotalStringLength = 200f;

    public AbstractPhysicalObject? AbstractTiedObject
    {
        get => abstractStringTiedItem.abstractTiedObject;
    }

    public PhysicalObject? TiedObject
    {
        get => abstractStringTiedItem.abstractTiedObject?.realizedObject;
    }

    public BodyChunk? BodyChunkTied
    {
        get
        {
            if (BodyChunkTiedIndex == -1)
                return null;
            else
                return TiedObject.bodyChunks[BodyChunkTiedIndex];
        }
        set
        {
            if (value == null)
            {
                BodyChunkTiedIndex = -1;
            }
            else
            {
                BodyChunkTiedIndex = value.index;
            }
        }
    }

    /// <summary>
    /// If set to -1 to indicate that the item index is not attached to anything.
    /// </summary>
    public int BodyChunkTiedIndex
    {
        get => abstractStringTiedItem.bodyChunkTied;
        set => abstractStringTiedItem.bodyChunkTied = value;
    }

    public int StringSprite => 0;
    public const int TotalSprites = 1;
    public const int StringMidPart = 10;
    /// <summary>
    /// The total amount of stalks used in the calculation of the string's length and appearance.
    /// More of these makes for a bendier more realistic string, but costs more to compute.
    /// </summary>
    public const int TotalStringParts = 20;
    /// <summary>
    /// The visual length of a stalk, used to make the stalk longer without a performance hit.
    /// </summary>
    public const float stringPartLength = 4f;

    /// <summary>
    /// How fast the stalk will settle and surcumb to gravity.
    /// </summary>
    public const float stringRestSpeed = 0.5f;

    public const float stringThickness = 0.6f;

    public float swallowed;

    public Color stringColor;

    public StringTiedItem(AbstractStringTiedItem abstractStringTiedItem) : base(abstractStringTiedItem)
    {
        this.abstractStringTiedItem = abstractStringTiedItem;
        var pos = abstractStringTiedItem.Room.realizedRoom.MiddleOfTile(abstractStringTiedItem.pos.Tile);

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

        mRCord = new MRCord(this, firstChunk, TotalStringParts)
        {
            partLength = stringPartLength,
            restSpeed = stringRestSpeed,
            midPart = 0 // The point at which you grab is the first part of the stalk.
        };
        mRCord.SetPartsRadius(stringThickness);
        mRCord.forceSetPartPositions = new Dictionary<int, Vector2>
        {
            { 0, Vector2.zero},
            { TotalStringParts - 1, Vector2.zero }
        };
    }

    public void AttachToObject(AbstractPhysicalObject abstractTiedObject, int bodyChunkToTieToIndex)
    {
        if (abstractTiedObject != null)
            DetachObject();

        abstractStringTiedItem.abstractTiedObject = abstractTiedObject;
        BodyChunkTiedIndex = bodyChunkToTieToIndex;

        var tiedObject = abstractTiedObject.realizedObject;

        bodyChunkConnections = new BodyChunkConnection[]
        {
            new BodyChunkConnection(this.firstChunk, BodyChunkTied, TotalStringLength, type: BodyChunkConnection.Type.Pull, 0.9f, 0.5f)
        };
    }

    public void DetachObject()
    {
        bodyChunkConnections = new BodyChunkConnection[0];

        abstractStringTiedItem.abstractTiedObject = null;
        BodyChunkTied = null;
    }

    public override void PlaceInRoom(Room placeRoom)
    {
        base.PlaceInRoom(placeRoom);
        mRCord.ResetParts();
    }

    public override void NewRoom(Room newRoom)
    {
        base.NewRoom(newRoom);
        mRCord.ResetParts();
    }

    public override void Update(bool eu)
    {
        base.Update(eu);

        mRCord.forceSetPartPositions[0] = firstChunk.pos;
        if (BodyChunkTiedIndex != -1)
        {
            mRCord.forceSetPartPositions[TotalStringParts - 1] = BodyChunkTied.pos;
        }
        mRCord.Update();
    }

    //
    // IDRAWABLE INTERFACE
    //

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[TotalSprites]
        {
            TriangleMesh.MakeLongMesh(mRCord.parts.Length, pointyTip: false, customColor: true)
        };
        AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        var triMesh = sLeaser.sprites[StringSprite] as TriangleMesh;

        Vector2 startingStalksChangeInPos = Vector2.Lerp(mRCord.parts[0].lastPos, mRCord.parts[0].pos, timeStacker);
        startingStalksChangeInPos += Custom.DirVec(Vector2.Lerp(mRCord.parts[1].lastPos, mRCord.parts[1].pos, timeStacker), startingStalksChangeInPos) * stringPartLength;
        for (int i = 0; i < mRCord.parts.Length; i++)
        {
            Vector2 currentStalkPos = Vector2.Lerp(mRCord.parts[i].lastPos, mRCord.parts[i].pos, timeStacker);
            Vector2 normalized = (currentStalkPos - startingStalksChangeInPos).normalized;
            Vector2 currentStalkPerpindicularAngle = Custom.PerpendicularVector(normalized);
            float distanceFromFirstStalk = Vector2.Distance(currentStalkPos, startingStalksChangeInPos) / 5f;
            if (i == 0)
            {
                triMesh.MoveVertice(i * 4, startingStalksChangeInPos - currentStalkPerpindicularAngle * stringThickness - camPos);
                triMesh.MoveVertice(i * 4 + 1, startingStalksChangeInPos + currentStalkPerpindicularAngle * stringThickness - camPos);
            }
            else
            {
                triMesh.MoveVertice(i * 4, startingStalksChangeInPos - currentStalkPerpindicularAngle * stringThickness + normalized * distanceFromFirstStalk - camPos);
                triMesh.MoveVertice(i * 4 + 1, startingStalksChangeInPos + currentStalkPerpindicularAngle * stringThickness + normalized * distanceFromFirstStalk - camPos);
            }
            triMesh.MoveVertice(i * 4 + 2, currentStalkPos - currentStalkPerpindicularAngle * stringThickness - normalized * distanceFromFirstStalk - camPos);
            triMesh.MoveVertice(i * 4 + 3, currentStalkPos + currentStalkPerpindicularAngle * stringThickness - normalized * distanceFromFirstStalk - camPos);
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
        else if (sLeaser.sprites[StringSprite].color == base.blinkColor)
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
            newColor = stringColor;

        for (int j = 0; j < ((TriangleMesh)sLeaser.sprites[StringSprite]).verticeColors.Length; j++)
        {
            ((TriangleMesh)sLeaser.sprites[StringSprite]).verticeColors[j] = newColor;
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        stringColor = palette.blackColor;
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
