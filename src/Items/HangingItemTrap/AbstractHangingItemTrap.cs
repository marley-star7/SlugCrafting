using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugCrafting.Items;

public class AbstractHangingItemTrap : AbstractPhysicalObject
{
    public AbstractHangingItemTrap(World world, AbstractObjectType type, PhysicalObject realizedObject, WorldCoordinate pos, EntityID ID) : base(world, type, realizedObject, pos, ID)
    {
    }
}
