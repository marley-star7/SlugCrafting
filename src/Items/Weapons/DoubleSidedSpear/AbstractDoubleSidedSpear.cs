/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugCrafting.Weapons;

public class AbstractDoubleSidedSpear : AbstractPhysicalObject
{
    public AbstractPhysicalObject abstractBackSpear;

    public AbstractDoubleSidedSpear(AbstractPhysicalObject abstractBackSpear, World world, DoubleSidedSpear realizedObject, WorldCoordinate pos, EntityID ID)
        : base(world, DoubleSidedSpearFisob.abstractObjectType, realizedObject, pos, ID)
    {
        this.abstractBackSpear = abstractBackSpear;
    }

    public override void Realize()
    {
        base.Realize();
        if (realizedObject == null)
            realizedObject = new DoubleSidedSpear(this, world);
    }
}
*/