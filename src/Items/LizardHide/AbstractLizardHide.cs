using Fisobs.Core;
using UnityEngine;

namespace SlugCrafting.Items;

// TODO: swap the Abstract around?
sealed class AbstractLizardHide : AbstractPhysicalObject
{
    public AbstractLizardHide(World world, WorldCoordinate pos, EntityID ID) 
        : base(world, LizardHideFisob.abstractObjectType, null, pos, ID)
    {

    }

    public override void Realize()
    {
        base.Realize();
        if (realizedObject == null)
            realizedObject = new LizardHide(this, Room.realizedRoom.MiddleOfTile(pos.Tile));
    }

    //public override string ToString()
    //{
    //    return this.SaveToString();
    //}
}