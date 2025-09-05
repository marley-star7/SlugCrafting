namespace SlugCrafting.Menus;

public class MenuInventory
{
    public event Action<EntityMenuData> PostItemAdded;

    public event Action<EntityMenuData> PostItemRemoved;

    private Dictionary<EntityTypeDefinition, List<EntityMenuData>> _items = new();
    /// <summary>
    /// Dictionary of items in the inventory, categorized by their object definitions, 
    /// Contains a list of each object of that type in the inventory.
    /// </summary>
    public Dictionary<EntityTypeDefinition, List<EntityMenuData>> Items
    {
        get { return _items; }
    }

    /// <summary>
    /// Add an item to the inventory.
    /// </summary>
    /// <param name="entityMenuData"></param>
    public void AddItem(EntityMenuData entityMenuData)
    {
        if (!_items.ContainsKey(entityMenuData.typeDefinition))
        {
            _items[entityMenuData.typeDefinition] = new List<EntityMenuData>();
        }

        _items[entityMenuData.typeDefinition].Add(entityMenuData);
        PostItemAdded?.Invoke(entityMenuData);
    }

    public void RemoveItem(EntityMenuData entityMenuData)
    {
        if (!_items.ContainsKey(entityMenuData.typeDefinition))
            return;

        _items[entityMenuData.typeDefinition].Remove(entityMenuData);
        if (_items[entityMenuData.typeDefinition].Count == 0)
        {
            _items.Remove(entityMenuData.typeDefinition);
        }

        PostItemRemoved?.Invoke(entityMenuData);
    }

    public bool CanCraftRecipe(in CraftRecipe recipe)
    {
        return HasSufficientIngredients(recipe);
    }

    public bool HasSufficientIngredients(in CraftRecipe recipe)
    {
        // -- Ms7: Disabled the debug prints, stuff seems to be working so don't need right now.

        for (int i = 0; i < recipe.ingredients.Length; i++)
        {
            var ingredient = recipe.ingredients[i];
            var ingredientMaterialObjectDef = ingredient.material.entityTypeDefinition;

/*
#if DEBUG
            Plugin.LogDebug($"Looking for: {ingredientMaterialObjectDef.objectType} (Hash: {ingredientMaterialObjectDef.GetHashCode()})");
            Plugin.LogDebug($"Dictionary contains key: {_items.ContainsKey(ingredientMaterialObjectDef)}");
#endif
*/

            if (_items.TryGetValue(ingredientMaterialObjectDef, out List<EntityMenuData> objects))
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
/*
#if DEBUG
                Plugin.LogDebug($"Key not found in dictionary");
                Plugin.LogDebug($"All keys in dictionary:");
                foreach (var key in _items.Keys)
                {
                    Plugin.LogDebug($"  - {key} (Hash: {key.GetHashCode()})");
                }
#endif
*/
                return false;
            }
        }
/*
#if DEBUG
        Plugin.LogDebug($"Has ingredients for recipe of result {recipe.resultedObjects[0].objectType}");
#endif
*/
        return true;
    }
}
