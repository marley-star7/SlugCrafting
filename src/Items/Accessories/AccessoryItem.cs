using UnityEngine;
using RWCustom;

using CompartmentalizedCreatureGraphics;
using SlugCrafting.Accessories;

namespace SlugCrafting.Items;

public class AccessoryItem : PlayerCarryableItem, IDrawable
{
    public Player wearer;

    public Accessory accessory;

    public AccessoryItem(AbstractPhysicalObject abstractPhysicalObject)
        : base(abstractPhysicalObject)
    {
    }

    public virtual void Equip(Player wearer)
    {
        this.wearer = wearer;
        wearer.EquipAccessory(accessory);

        this.CollideWithObjects = false;
        this.CollideWithTerrain = false;
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
