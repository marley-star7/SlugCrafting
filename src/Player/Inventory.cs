namespace SlugCrafting;

public class Inventory
{
    private Dictionary<ObjectDefinition, List<AbstractPhysicalObject>> _items = new();
    /// <summary>
    /// Dictionary of items in the inventory, categorized by their object definitions, 
    /// Contains a list of each object of that type in the inventory.
    /// </summary>
    public Dictionary<ObjectDefinition, List<AbstractPhysicalObject>> Items => _items;

    /// <summary>
    /// Add an item to the inventory.
    /// </summary>
    /// <param name="abstractPhysicalObject"></param>
    public void AddItem(AbstractPhysicalObject abstractPhysicalObject)
    {
        ObjectDefinition itemDef = new(abstractPhysicalObject);

        if (!_items.ContainsKey(itemDef))
            _items[itemDef] = new List<AbstractPhysicalObject>();

        _items[itemDef].Add(abstractPhysicalObject);
    }

    /// <summary>
    /// Remove an item from the inventory.
    /// </summary>
    /// <param name="abstractPhysicalObject"></param>
    public void RemoveItem(AbstractPhysicalObject abstractPhysicalObject)
    {
        ObjectDefinition itemDef = new(abstractPhysicalObject);

        if (!_items.ContainsKey(itemDef))
            return;

        _items[itemDef].Remove(abstractPhysicalObject);
    }

    public bool CanCraftRecipe(in CraftRecipe recipe)
    {
        return HasSufficientIngredients(recipe);
    }

    public bool HasSufficientIngredients(in CraftRecipe recipe)
    {
        for (int i = 0; i < recipe.ingredients.Length; i++)
        {
            var ingredient = recipe.ingredients[i];
            var ingredientMaterialObjectDef = ingredient.material.objectDefinition;

#if DEBUG
            Plugin.LogDebug($"Looking for: {ingredientMaterialObjectDef.objectType} (Hash: {ingredientMaterialObjectDef.GetHashCode()})");
            Plugin.LogDebug($"Dictionary contains key: {_items.ContainsKey(ingredientMaterialObjectDef)}");
#endif

            if (_items.TryGetValue(ingredientMaterialObjectDef, out List<AbstractPhysicalObject> objects))
            {
#if DEBUG
                Plugin.LogDebug($"Found {objects.Count} objects, need {ingredient.quantityRequired}");
#endif
                if (ingredient.quantityRequired > objects.Count)
                {
                    return false;
                };
            }
            else
            {
#if DEBUG
                Plugin.LogDebug($"Key not found in dictionary");
                Plugin.LogDebug($"All keys in dictionary:");
                foreach (var key in _items.Keys)
                {
                    Plugin.LogDebug($"  - {key} (Hash: {key.GetHashCode()})");
                }
#endif
                return false;
            }
        }
#if DEBUG
        Plugin.LogDebug($"Has ingredients for recipe of result {recipe.resultedObjects[0].objectType}");
#endif
        return true;
    }
}
