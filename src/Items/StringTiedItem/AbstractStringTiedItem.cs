using RWCustom;
using UnityEngine;

namespace SlugCrafting.Items;

public class AbstractStringTiedItem : AbstractPhysicalObject
{
    public AbstractPhysicalObject? abstractTiedObject;
    public int bodyChunkTied = -1;

    public AbstractStringTiedItem(World world, WorldCoordinate pos, EntityID ID) : base(world, StringTiedItemFisob.abstractObjectType, null, pos, ID)
    {
    }

    public override void Realize()
    {
        base.Realize();
        if (realizedObject == null)
            realizedObject = new StringTiedItem(this);
    }
}
