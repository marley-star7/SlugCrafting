using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SlugCrafting.Items;

public class HangingItemTrap : PlayerCarryableItem, IDrawable
{
    public AbstractHangingItemTrap abstractHangingItemTrap;
    public AbstractPhysicalObject hangingItem;
    public HangingItemTrap(AbstractHangingItemTrap abstractHangingItemTrap) 
        : base(abstractHangingItemTrap)
    {
        var pos = abstractPhysicalObject.Room.realizedRoom.MiddleOfTile(abstractPhysicalObject.pos.Tile);
    }

    //
    // IDRAWABLE INTERFACE
    //

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        throw new NotImplementedException();
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        throw new NotImplementedException();
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        throw new NotImplementedException();
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        throw new NotImplementedException();
    }
}
