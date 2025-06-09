using RWCustom;
using UnityEngine;

using SlugCrafting.Scavenges;
using SlugCrafting.Crafts;
using SlugCrafting.Items;

namespace SlugCrafting.Core;

public static partial class Content
{
    //
    // CRAFTING
    //

    /// <summary>
    /// The first element in the tuple is the primary dominant ingredient.
    /// Second is the non-dominant (or secondary) ingredient.
    /// A dictionary is used for optimized lookup
    /// </summary>
    public static readonly Dictionary<(AbstractPhysicalObject.AbstractObjectType, AbstractPhysicalObject.AbstractObjectType), Craft> Crafts = new();

    /// <summary>
    /// Register a new craft to the crafting system.
    /// The primary ingredient is the one in your character's dominant hand, and is the item that always ends up "changed" after crafts.
    /// The secondary ingredient is the one in your character's non-dominant hand, it is where tools, or the item that is consumed is held.
    /// The craft will always end with the new item in your primary (dominant) hand.
    /// </summary>
    /// <param name="primaryIngredient"></param>
    /// <param name="secondaryIngredient"></param>
    /// <param name="newCraft"></param>
    public static void RegisterCraft(Craft newCraft)
    {
        var ingredientTuple = (newCraft.primaryIngredient.type, newCraft.secondaryIngredient.type);

        if (Crafts.ContainsKey(ingredientTuple))
            Crafts[ingredientTuple] = newCraft;
        else
            Crafts.Add(ingredientTuple, newCraft);
    }

    /// <summary>
    /// The static collection of all crafts registered in the crafting system.
    /// </summary>
    public static readonly HashSet<ShelterCraft> ShelterCrafts = new();

    /// <summary>
    /// The dynamic dictionary of crafts using a specific object type,
    /// Important saved and updated to improve performance during runtime real time questionaires for crafting.
    /// </summary>
    public static readonly Dictionary<AbstractPhysicalObject.AbstractObjectType, HashSet<ShelterCraft>> ShelterCraftsUsingObjectType = new();

    /// <summary>
    /// Register a new craft to the crafting system.
    /// </summary>
    /// <param name="newCraft"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void RegisterShelterCraft(ShelterCraft newCraft)
    {
        if (ShelterCrafts.Contains(newCraft))
            throw new ArgumentException($"Craft already registered: {newCraft}");
        else
            ShelterCrafts.Add(newCraft);

        foreach (var ingredient in newCraft.ingredients)
        {
            // Search for the ingredient type in dictionary
            if (ShelterCraftsUsingObjectType.TryGetValue(ingredient.type, out HashSet<ShelterCraft> crafts))
            {
                if (!crafts.Contains(newCraft))
                    crafts.Add(newCraft);
            }
            else
            {
                // If the ingredient type is not found, create a new Hashet for that ingredient containing the craft, and add it to the dictionary.
                ShelterCraftsUsingObjectType[ingredient.type] = crafts = new HashSet<ShelterCraft>()
                {
                    newCraft
                };
            }
        }
    }

    public static bool ShelterCraftContainsIngredient(in ShelterCraft craft, AbstractPhysicalObject.AbstractObjectType objectType)
    {
        for (int i = 0; i < craft.ingredients.Length; i++)
        {
            // Check if the ingredient type is in the object types.
            if (craft.ingredients[i].type == objectType)
            {
                return true;
            }
        }
        return false;
    }

    public static bool HasAllIngredientsForShelterCraft(AbstractPhysicalObject.AbstractObjectType[] objectTypes, in ShelterCraft craft)
    {
        for (int i = 0; i < objectTypes.Length; i++)
        {
            // Check if the ingredient type is in the object types.
            if (!ShelterCraftContainsIngredient(craft, objectTypes[i]))
            {
                return false;
            }
        }
        return true;
    }

    public static HashSet<ShelterCraft> GetShelterCraftsForObjectType(AbstractPhysicalObject.AbstractObjectType objectType)
    {
        if (ShelterCraftsUsingObjectType.TryGetValue(objectType, out HashSet<ShelterCraft> crafts))
        {
            return crafts;
        }
        else
        {
            return null;
        }
    }

    public static HashSet<ShelterCraft> GetSheterCraftsForObjectTypes(AbstractPhysicalObject.AbstractObjectType[] objectTypes)
    {
        var craftsContainingAllObjectTypes = new HashSet<ShelterCraft>();

        if (ShelterCraftsUsingObjectType.TryGetValue(objectTypes[0], out HashSet<ShelterCraft> crafts)) ;
        else return craftsContainingAllObjectTypes;

        // Loop through all the crafts
        for (int i = 0; i < crafts.Count; i++)
        {
            // Then check all the ingredients in the chosen craft.
            var checkingCraft = crafts.ElementAt(i);
            var checkingCraftContainsAllObjectTypes = true;

            // Loop through all the checking crafts ingredient object types.
            for (int j = 0; j < checkingCraft.ingredients.Length; j++)
            {
                var checkingCraftContainsIngredient = false;

                // Loop through all the object types we want to compare too, and note if it has it.
                for (int k = 0; k < objectTypes.Length; k++)
                {
                    // Check if the ingredient type is in the object types.
                    if (checkingCraft.ingredients[i].type == objectTypes[j])
                    {
                        checkingCraftContainsIngredient = true;
                        continue;
                    }
                }
                if (!checkingCraftContainsIngredient)
                {
                    checkingCraftContainsAllObjectTypes = false;
                    break;
                }
            }
            // If the craft contains all the object types, add it to the list.
            if (checkingCraftContainsAllObjectTypes)
            {
                craftsContainingAllObjectTypes.Add(checkingCraft);
            }
        }

        return craftsContainingAllObjectTypes;
    }

    //
    // SCAVENGING
    //

    public static readonly Dictionary<CreatureTemplate.Type, Type> CreatureScavengeTypes = new Dictionary<CreatureTemplate.Type, Type>();

    /// <summary>
    /// Register a creature type with its corresponding scavenge data type.
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
    /// Create scavenge data for a given creature type.
    /// </summary>
    /// <param name="creatureType"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public static CreatureScavengeData CreateScavengeData(Creature creature, CreatureTemplate.Type creatureType)
    {
        if (CreatureScavengeTypes.TryGetValue(creatureType, out Type scavengeDataType))
        {
            var instance = (CreatureScavengeData)Activator.CreateInstance(scavengeDataType, creature);
            return instance;
        }
        // Return default Lizard Scavenge Data if no specific lizard type is registered.
        if (creature.Template.IsLizard)
        {
            Plugin.Logger.LogDebug($"No specific scavenge data registered for lizard type: {creatureType}, using default LizardScavengeData.");
            var instance = (CreatureScavengeData)Activator.CreateInstance(typeof(LizardScavengeData), creature);
            return instance;
        }

        Plugin.Logger.LogDebug($"No scavenge data registered for creature type: {creatureType}");
        return null;
    }

    //
    // ITEM BUNDLES
    //

    public static readonly Dictionary<AbstractPhysicalObject.AbstractObjectType, ItemBundleProperties> ItemsBundleProperties = new Dictionary<AbstractPhysicalObject.AbstractObjectType, ItemBundleProperties>();

    public static void RegisterItemBundleProperties(AbstractPhysicalObject.AbstractObjectType type, ItemBundleProperties properties)
    {
        ItemsBundleProperties[type] = properties;
    }
}