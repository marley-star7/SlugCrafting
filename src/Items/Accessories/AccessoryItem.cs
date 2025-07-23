using UnityEngine;
using RWCustom;

using CompartmentalizedCreatureGraphics;

namespace SlugCrafting.Items.Accessories;

public class AccessoryItem : PlayerCarryableItem, IDrawable
{
    public Player? wearer => accessory.wearer;

    public Accessory accessory;

    public AccessoryItem(AbstractPhysicalObject abstractPhysicalObject)
        : base(abstractPhysicalObject)
    {
    }

    public virtual void Equip(Player wearer)
    {
        wearer.EquipAccessory(accessory);

        CollideWithObjects = false;
        CollideWithTerrain = false;
    }

    //
    // IDRAWABLE
    //

    public virtual void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
    {
        newContainer ??= rCam.ReturnFContainer("Items");

        foreach (FSprite fsprite in sLeaser.sprites)
        {
            fsprite.RemoveFromContainer();
            newContainer.AddChild(fsprite);
        }
    }

    public virtual void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {

    }

    public virtual void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {

    }

    public virtual void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {

    }
}
