using Fisobs.Core;
using UnityEngine;

namespace SlugCrafting.Items;

// TODO: swap the Abstract around?
sealed class AbstractLizardLeather : AbstractPhysicalObject
{
    public AbstractLizardLeather(World world, WorldCoordinate pos, EntityID ID) 
        : base(world, LizardLeatherFisob.abstractObjectType, null, pos, ID)
    {

    }

    public override void Realize()
    {
        base.Realize();
        if (realizedObject == null)
            realizedObject = new LizardLeather(this, Room.realizedRoom.MiddleOfTile(pos.Tile));
    }

    //public override string ToString()
    //{
    //    return this.SaveToString();
    //}
}