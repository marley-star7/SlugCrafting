using Fisobs.Core;
using UnityEngine;

namespace SlugCrafting.Items;

// TODO: swap the Abstract around?
public sealed class AbstractString : AbstractPhysicalObject
{
    public AbstractString(World world, WorldCoordinate pos, EntityID ID)
        : base(world, StringFisob.abstractObjectType, null, pos, ID)
    {

    }

    public override void Realize()
    {
        base.Realize();
        if (realizedObject == null)
            realizedObject = new String(this);
    }

    //public override string ToString()
    //{
    //    return this.SaveToString();
    //}
}