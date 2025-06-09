using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SlugCrafting.Scavenges;

namespace SlugCrafting;

public static class CreatureCraftingExtension
{
    private static readonly ConditionalWeakTable<Creature, CreatureScavengeData> conditionalWeakTable = new();

    public static CreatureScavengeData GetScavengeData(this Creature creature)
    {
        //-- Check if the creature already has scavenge data and return that,
        // If it doesn't, make some and return that.
        return conditionalWeakTable.GetValue(creature, _ => SlugCrafting.Core.Content.CreateScavengeData(creature, creature.Template.type));
    }
}
