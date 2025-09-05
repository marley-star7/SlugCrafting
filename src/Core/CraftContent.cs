using MRCustom.IdGenerators;

namespace SlugCrafting;

public static partial class Content
{
    // Ms7: Set up a craft recipe id system, this way only one list is needed for all craft recipes, which can be efficiently looped through.
    // A craft recipe stores the id of the craft with it, which you get set yourself in the constructor for no confusion as to each craft recipe having it's own unique id.
    // The id is used to refer to shelter crafts by dictionary, for efficient gettage in the thingy, all very performance friendly! (A little overkill tbh)

    private static UShortIdGenerator CraftIDGenerator = new();

    /// <summary>
    /// All the registered craft recipes in the crafting system.
    /// </summary>
    public static readonly List<CraftRecipe> CraftRecipes = new();

    /// <summary>
    /// All the registered craft recipes, with their assocaited ID as the key.
    /// </summary>
    public static readonly Dictionary<ushort, CraftRecipe> CraftRecipeIDs = new();

    /// <summary>
    /// The first element in the tuple is the primary dominant ingredient.
    /// Second is the non-dominant (or secondary) ingredient.
    /// A dictionary is used for optimized lookup, so can immediately see the existance of crafts using an item.
    /// </summary>
    public static readonly Dictionary<(CraftRecipe.Material?, CraftRecipe.Material?), HandCraft> HandCrafts = new();

    /// <summary>
    /// The static collection of all shelter crafts registered in the crafting system.
    /// Access a ShelterCraft by it's CraftRecipe Id.
    /// </summary>
    public static readonly Dictionary<ushort, ShelterCraft> ShelterCrafts = new();

    // -- Ms7: The purpose of the RegisterHandCraft optional register for shelter craft is just to help in creation, and clearly define it in code as a craft that shares id's with a ShelterCraft
    // This so that if showing both HandCrafts and ShelterCrafts there is not duplicate entries in the menu for the same resulting craft.

    /// <summary>
    /// Register a new craft to the crafting system.
    /// The primary ingredient is the one in your character's dominant hand, and is the item that always ends up "changed" after crafts.
    /// The secondary ingredient is the one in your character's non-dominant hand, it is where tools, or the item that is consumed is held.
    /// The craft will always end with the new item in your primary (dominant) hand.
    /// If you set an optional ShelterCraft.CraftResult, it will also register a ShelterCraft for this HandCraft, using the same recipe.
    /// </summary>
    /// <param name="newCraft"></param>
    /// <param name="optionalShelterCraftResult"></param>
    public static void RegisterHandCraft(HandCraft newCraft, ShelterCraft.CraftResult optionalShelterCraftResult = null)
    {
        var ingredientTuple = (newCraft.primaryIngredient.material, newCraft.secondaryIngredient.material);

        if (HandCrafts.ContainsKey(ingredientTuple))
            HandCrafts[ingredientTuple] = newCraft;
        else
            HandCrafts.Add(ingredientTuple, newCraft);

        if (optionalShelterCraftResult != null)
        {
            RegisterShelterCraft(new ShelterCraft(newCraft.recipe, optionalShelterCraftResult));
        }
        else
        {
            RegisterCraftRecipe(newCraft.recipe); // RegisterShelterCraft registers craft recipe, no need to do twice,
        }
    }

    /// <summary>
    /// Register a new craft to the crafting system.
    /// </summary>
    /// <param name="newShelterCraft"></param>
    /// <exception cref="ArgumentException"></exception>
    public static void RegisterShelterCraft(ShelterCraft newShelterCraft)
    {
        ShelterCrafts.Add(newShelterCraft.recipe.craftID, newShelterCraft);
        RegisterCraftRecipe(newShelterCraft.recipe);
    }

    /// <summary>
    /// Generate a unique craft ID to be assigned to a CraftRecipe, relating it back to it's respective crafts.
    /// </summary>
    /// <returns></returns>
    public static ushort GenerateUniqueCraftID()
    {
        return CraftIDGenerator.GenerateUniqueId();
    }

    private static void RegisterCraftRecipe(CraftRecipe newRecipe)
    {
        if (CraftRecipes.Contains(newRecipe))
        {
            Plugin.LogError($"Craft Recipe of ingredients: {newRecipe.ingredients} already exists! Cannot register! What are you actually doing? How did you do this?");
            return;
        }

        CraftRecipes.Add(newRecipe);
        CraftRecipeIDs.Add(newRecipe.craftID, newRecipe);
    }

    /*
    public static bool ShelterCraftContainsIngredient(in ShelterCraft craft, AbstractPhysicalObject.AbstractObjectType objectType)
    {
        for (int i = 0; i < craft.ingredients.Length; i++)
        {
            // Check if the ingredient type is in the object types.
            if (craft.ingredients[i].material == objectType)
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
        */

    /*
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
    */

    //
    // Ms7: Old functionality when scavenges were still functionally seperate from crafts.
    //

    /*
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
        // Return default Lizard Scavenge MagneticState if no specific lizard type is registered.
        if (creature.Template.IsLizard)
        {
            Plugin.LogDebug($"No specific scavenge data registered for lizard type: {creatureType}, using default LizardScavengeData.");
            var instance = (CreatureScavengeData)Activator.CreateInstance(typeof(LizardScavengeData), creature);
            return instance;
        }

        Plugin.LogDebug($"No scavenge data registered for creature type: {creatureType}");
        return null;
    }
    */

    //
    // HELPER FUNCTIONS
    //

    // -- Ms7: DISABLED! Do it manually, because abstractobject construction can sometimes change between types.
    /*
    public static Action<Creature, PhysicalObject, PhysicalObject> CreateConsumeBothItemsRealizeObjectTypeCraftResult(
    AbstractPhysicalObject.AbstractObjectType objectType)
    {
        return (crafter, primary, secondary) =>
        {
            crafter.RemoveGrabbedObject(0);
            crafter.RemoveGrabbedObject(1);

            var player = (crafter as Player);
            player.RealizeAndGrab(new AbstractPhysicalObject(
                crafter.room.world,
                objectType,
                null,
                crafter.coord,
                crafter.room.game.GetNewID()
            ));
        };
    }
    */
}