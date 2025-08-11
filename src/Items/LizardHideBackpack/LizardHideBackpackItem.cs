
using MRCustom.Physics;

namespace SlugCrafting.Items;

public class LizardHideBackpackItem : PlayerCarryableItem, IDrawable, IHaveVisibleItemContainerCycler, IHavePlayerAlternateUse
{
    public AbstractLizardHideBackpack abstractLizardHideBackpack;
    public LizardHideBackpack lizardHideBackpack;

    public Vector2 lastRotation;
    public Vector2 rotation;

    public Vector2 rotVel;

    public ItemContainer itemContainer => lizardHideBackpack.itemContainer;
    public VisibleItemContainerCycler visibleItemContainerCycler => lizardHideBackpack.itemContainerCycler;

    public LizardHideBackpackItem(AbstractLizardHideBackpack abstractPhysicalObject, LizardHideBackpack lizardHideBackpack) : base(abstractPhysicalObject)
    {
        this.abstractLizardHideBackpack = abstractPhysicalObject;
        this.lizardHideBackpack = lizardHideBackpack;

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
    }

    public void AlternateUse(Creature user)
    {
        throw new NotImplementedException();
    }

    public override void Update(bool eu)
    {
        StorePreviousStates();
        base.Update(eu);

        UpdateRotation();
    }

    private void StorePreviousStates()
    {
        lastRotation = rotation;
    }

    private void UpdateRotation()
    {
        rotation = Custom.DegToVec(Custom.VecToDeg(rotation) + rotVel.x);
        rotVel = Vector2.ClampMagnitude(rotVel, 50f);
        rotVel *= Custom.LerpMap(rotVel.magnitude, 5f, 50f, 1f, 0.8f);
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("Circle20", true);
        AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);

        Vector3 rot3 = Vector3.Slerp(lastRotation, rotation, timeStacker);
        float finalRotation = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), rot3);

        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].x = pos.x - camPos.x;
            sLeaser.sprites[i].y = pos.y - camPos.y;
            sLeaser.sprites[i].rotation = finalRotation;
            sLeaser.sprites[i].color = color;
        }

        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        color = palette.blackColor;
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContainer)
    {
        newContainer ??= rCam.ReturnFContainer("Items");

        foreach (FSprite fsprite in sLeaser.sprites)
        {
            fsprite.RemoveFromContainer();
            newContainer.AddChild(fsprite);
        }
    }
}
