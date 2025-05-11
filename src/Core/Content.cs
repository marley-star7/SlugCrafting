using RWCustom;
using UnityEngine;

using System.Runtime.CompilerServices;
using SlugCrafting.Scavenges;

namespace SlugCrafting.Core;

public static class Content
{
    public static readonly Dictionary<CreatureTemplate.Type, Type> CreatureScavengeTypes = new Dictionary<CreatureTemplate.Type, Type>();

    /// <summary>
    /// Register a creature type with its corresponding scavenge data type
    /// </summary>
    /// <param name="creatureType"></param>
    /// <param name="scavengeDataType"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void RegisterScavengeData(CreatureTemplate.Type creatureType, Type scavengeDataType)
    {
        if (!typeof(CreatureScavengeData).IsAssignableFrom(scavengeDataType))
        {
            throw new ArgumentException($"Type must inherit from CreatureScavengeData: {scavengeDataType}");
        }

        CreatureScavengeTypes[creatureType] = scavengeDataType;
    }

    /// <summary>
    /// // Create scavenge data for a given creature type
    /// </summary>
    /// <param name="creatureType"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static CreatureScavengeData CreateScavengeData(Creature creature, CreatureTemplate.Type creatureType)
    {
        if (CreatureScavengeTypes.TryGetValue(creatureType, out Type scavengeDataType))
        {
            var instance = (CreatureScavengeData) Activator.CreateInstance(scavengeDataType, creature);
            return instance;
        }

        throw new KeyNotFoundException($"No scavenge data registered for creature type: {creatureType}");
    }
}