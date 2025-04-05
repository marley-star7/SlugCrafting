using Fisobs.Core;
using UnityEngine;

namespace SlugCrafting.Items;

sealed class BoneKnifeAbstract : AbstractPhysicalObject
{
    // TODO: need to rework this whole class.
    public BoneKnifeAbstract(World world, WorldCoordinate pos, EntityID ID) : base(world, BoneKnifeFisob.AbstractBoneKnife, null, pos, ID)
    {

    }

    public override void Realize()
    {
        base.Realize();
        if (realizedObject == null)
            realizedObject = new BoneKnife(this, Room.realizedRoom.MiddleOfTile(pos.Tile), Vector2.zero);
    }

    public override string ToString()
    {
        return this.SaveToString();
    }
}