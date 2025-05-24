using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SlugCrafting.Items;

public class SpearData
{
    public Skewer skewer { get; set; }

    public WeakReference<Spear> spearRef;

    public SpearData(Spear spear)
    {
        spearRef = new WeakReference<Spear>(spear);
    }
}

public static class SpearExtension
{
    private static readonly ConditionalWeakTable<Spear, SpearData> craftingDataConditionalWeakTable = new();

    public static SpearData SlugCrafting(this Spear spear) => craftingDataConditionalWeakTable.GetValue(spear, _ => new SpearData(spear));
}
