using Fisobs.Core;
using UnityEngine;

namespace SlugCrafting.Items;

// TODO: swap the Abstract around?
sealed class AbstractKnife : AbstractPhysicalObject
{
    // TODO: need to rework this whole class.
    public AbstractKnife(World world, WorldCoordinate pos, EntityID ID) : base(world, KnifeFisob.abstractObjectType, null, pos, ID)
    {

    }

    public override void Realize()
    {
        base.Realize();
        if (realizedObject == null)
            realizedObject = new Knife(this, Room.realizedRoom.MiddleOfTile(pos.Tile), Vector2.zero);
    }

    //public override string ToString()
    //{
    //    return this.SaveToString();
    //}
}