using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugCrafting.Core;

public struct ItemBundleProperties
{
    public int maxBundleSize;

    public ItemBundleProperties(int maxBundleSize)
    {
        this.maxBundleSize = maxBundleSize;
    }
}
