namespace SlugCrafting.Crafts;

/// <summary>
/// -- Ms7: This is a craft recipe, used to define what ingredients are needed to craft an item or creature, as well as the information of resulting items from the craft.
/// Functions purely for checks of valid crafts during, and visual representation of the craft recipe in the crafting menu.
/// See "Craft" or "ShelterCraft" for the delegates that provide the actual programmed result of a craft, as well as animation information.
/// Implemented by "Craft and "ShelterCraft".
/// </summary>
public readonly struct CraftRecipe
{
    /// <summary>
    /// Represents a material used in crafting, including its object definition, type, and associated properties.
    /// </summary>
    /// <remarks>The <see cref="Material"/> struct encapsulates information about a crafting material, such as
    /// its object type, optional creature type, and the required body chunk index for crafting. It is immutable and
    /// provides access to its properties through read-only fields and computed values.</remarks>
    public readonly struct Material
    {
        /// <summary>
        /// Represents the definition of an object, including its properties and metadata.
        /// </summary>
        /// <remarks>This field is read-only and provides access to the associated <see
        /// cref="EntityTypeDefinition"/> instance. It is typically used to retrieve information about the structure or
        /// configuration of the object.</remarks>
        public readonly EntityTypeDefinition entityTypeDefinition;
        /// <summary>
        /// The AbstractObjectType of the ingredient.
        /// Set as AbstractObjectType.Creature if you wish to make the ingredient for a creature.
        /// </summary>
        public readonly AbstractPhysicalObject.AbstractObjectType? objectType => entityTypeDefinition.objectType;
        /// <summary>
        /// If this craft ingredient is a creature, the type of that creature.
        /// </summary>
        public readonly CreatureTemplate.Type? creatureType => entityTypeDefinition.creatureType;
        /// <summary>
        /// The required body chunk to hold for the craft, useful for specifying in creatures.
        /// </summary>
        public readonly int bodyChunkIndex;

        public Material(AbstractPhysicalObject.AbstractObjectType objectType, int bodyChunkIndex = 0, CreatureTemplate.Type? creatureType = null)
        {
            entityTypeDefinition = new EntityTypeDefinition(objectType, creatureType);
            this.bodyChunkIndex = bodyChunkIndex;
        }

        public override bool Equals(object obj) => obj is Material other && Equals(other);

        public bool Equals(Material other)
        {
            return bodyChunkIndex == other.bodyChunkIndex &&
                   entityTypeDefinition.Equals(other.entityTypeDefinition);
        }

        public override int GetHashCode() => HashCodeHelper.Combine(bodyChunkIndex, entityTypeDefinition);

        public static bool operator ==(Material left, Material right) => left.Equals(right);
        public static bool operator !=(Material left, Material right) => !left.Equals(right);
    }

    /// <summary>
    /// Represents an ingredient used in crafting, defined by its material and whether it is consumed during the
    /// crafting process.
    /// </summary>
    /// <remarks>An ingredient is characterized by its material, which specifies the object type, body chunk
    /// index, and optionally a creature type. It also indicates whether the material is consumed during the crafting
    /// process.</remarks>
    public readonly struct Ingredient
    {
        /// <summary>
        /// The material of the ingredient, which contains the object type and body chunk index.
        /// </summary>
        public readonly Material material;

        /// <summary>
        /// How many of the ingredients materials are required for the craft.
        /// </summary>
        public readonly int quantityRequired = 1;

        /// <summary>
        /// Wether or not this ingredient's material is consumed in the craft.
        /// </summary>
        public readonly bool consumed = true;

        public Ingredient(Material material, bool consumed = false, int amountReguired = 1)
        {
            this.material = material;
            this.consumed = consumed;
            this.quantityRequired = amountReguired;
        }

        public Ingredient(AbstractPhysicalObject.AbstractObjectType objectType, bool consumed = false, int bodyChunkIndex = 0, CreatureTemplate.Type? creatureType = null, int amountReguired = 1)
        {
            this.material = new Material(objectType, bodyChunkIndex, creatureType);
            this.consumed = consumed;
            this.quantityRequired = amountReguired;
        }

        public override bool Equals(object obj) => obj is Ingredient other && Equals(other);

        public bool Equals(Ingredient other)
        {
            return quantityRequired == other.quantityRequired &&
                   consumed == other.consumed &&
                   material.Equals(other.material);
        }

        public override int GetHashCode() => HashCodeHelper.Combine(quantityRequired, consumed, material);

        public static bool operator ==(Ingredient left, Ingredient right) => left.Equals(right);
        public static bool operator !=(Ingredient left, Ingredient right) => !left.Equals(right);
    }

    /// <summary>
    /// The ingredients that this craft recipe requires.
    /// </summary>
    public readonly Ingredient[] ingredients;
    /// <summary>
    /// The objects that will result from this craft recipe.
    /// </summary>
    public readonly EntityTypeDefinition[] resultedObjects;

    /// <summary>
    /// The unique ID to relate this craft recipe back to it's respective craft.
    /// </summary>
    public readonly ushort craftID;

    //-- Ms7: These are stored as a byte to really crunch this structs size, the super micro-optimization lol.

    /// <summary>
    /// How much food points this craft recipe costs in quarter food points.
    /// </summary>
    public readonly byte quarterFoodPointsCost = 0;

    /// <summary>
    /// The required body mode of the character to perform the craft during a hand craft.
    /// </summary>
    public enum BodyModeRequirement : byte
    {
        Any,
        Stand,
        Sneak,
    }

    /// <summary>
    /// The required body mode of this craft.
    /// </summary>
    public readonly BodyModeRequirement bodyModeRequirement;

    /// <param name="craftID"></param>
    /// <param name="ingredients"></param>
    /// <param name="resultedObjects"></param>
    /// <param name="bodyModeRequirement"></param>
    /// <param name="quarterFoodPointsCost"></param>
    public CraftRecipe(ushort craftID, Ingredient[] ingredients, EntityTypeDefinition[] resultedObjects, BodyModeRequirement bodyModeRequirement = BodyModeRequirement.Any, byte quarterFoodPointsCost = 0)
    {
        this.craftID = craftID;
        this.ingredients = ingredients;
        this.resultedObjects = resultedObjects;
        this.bodyModeRequirement = bodyModeRequirement;
        this.quarterFoodPointsCost = quarterFoodPointsCost;
    }

    /// <summary>
    /// Create the setup for an in-game craft recipe, with two ingredients and one resulted object.
    /// </summary>
    /// <param name="craftID"></param>
    /// <param name="primaryIngredient"></param>
    /// <param name="secondaryIngredient"></param>
    /// <param name="resultedObject"></param>
    /// <param name="bodyModeRequirement"></param>
    /// <param name="quarterFoodPointsCost"></param>
    public CraftRecipe(ushort craftID, Ingredient primaryIngredient, Ingredient secondaryIngredient, EntityTypeDefinition resultedObject, BodyModeRequirement bodyModeRequirement = BodyModeRequirement.Any, byte quarterFoodPointsCost = 0)
    {
        this.craftID = craftID;
        this.ingredients = new[] { primaryIngredient, secondaryIngredient };
        this.resultedObjects = new[] { resultedObject };
        this.bodyModeRequirement = bodyModeRequirement;
        this.quarterFoodPointsCost = quarterFoodPointsCost;
    }

    public override bool Equals(object obj) => obj is CraftRecipe other && Equals(other);

    public bool Equals(CraftRecipe other)
    {
        return craftID == other.craftID
            && ArrayEquals(ingredients, other.ingredients)
            && ArrayEquals(resultedObjects, other.resultedObjects)
            && bodyModeRequirement == other.bodyModeRequirement
            && quarterFoodPointsCost == other.quarterFoodPointsCost;
    }

    public override int GetHashCode()
    {
        int hash = HashCodeHelper.Combine(craftID);
        hash = HashCodeHelper.Combine(hash, HashCodeHelper.GetArrayHashCode(ingredients));
        hash = HashCodeHelper.Combine(hash, HashCodeHelper.GetArrayHashCode(resultedObjects));
        hash = HashCodeHelper.Combine(hash, bodyModeRequirement);
        hash = HashCodeHelper.Combine(hash, quarterFoodPointsCost);
        return hash;
    }

    public static bool operator ==(CraftRecipe left, CraftRecipe right) => left.Equals(right);
    public static bool operator !=(CraftRecipe left, CraftRecipe right) => !left.Equals(right);

    // Helper methods for array comparison
    private static bool ArrayEquals<T>(in T[] left, in T[] right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        if (left.Length != right.Length) return false;

        for (int i = 0; i < left.Length; i++)
        {
            if (!left[i]?.Equals(right[i]) ?? right[i] is not null)
                return false;
        }
        return true;
    }
}

public static class CraftRecipeExtension
{
    public static bool IsShelterCraft(this CraftRecipe craftRecipe)
    {
        if (craftRecipe == null) return false;

        if (Content.ShelterCrafts.ContainsKey(craftRecipe.craftID))
            return true;

        return false;
    }

    public static int GetTotalIngredientsQuantityRequired(this CraftRecipe craftRecipe)
    {
        var total = 0;

        for (int i = 0; i < craftRecipe.ingredients.Length; i++)
        {
            total += craftRecipe.ingredients[i].quantityRequired;
        }

        return total;
    }
}