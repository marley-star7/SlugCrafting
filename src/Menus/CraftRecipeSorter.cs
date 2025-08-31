namespace SlugCrafting.Menus;

// TODO: somehow need to have system to sort recipes by various criteria, and then also re-use other sorters sorting methods for any non-applicable recipes to a sort type.
// e.g. sort by ingredient, but also have recipes without ingredients still be sorted in a consistent manner.

// maybe do this by having functions that send all the recipes that need to be sorted, and return a list of it sorted in that manner.
// Well actually, do not really need to do this since can just hide everything that does not apply to the current sort type.
// Still crafts that are avaliable should be sorted higher.

// SORT METHODS:
// By Ingredient (e.g. all recipes that use twigs, then all recipes that use rocks, etc.), the ones that don't use any do not show up.
// By Result, all recipes that make a certain item.
// By availability, all recipes that can be made with current inventory items.

// TODO: can also maybe have the inventory items that appear by selectable buttons that when clicked, sort the recipe list by ingredient of that item.

/*
public class CraftRecipeSorter
{
    public abstract List<CraftRecipe> SortRecipes(List<CraftRecipe> recipes);
}

public class CraftRecipeByIngredientSorter
{
    
}
*/

// When you come back the first thing you will do is make avaliability sorter, do this in craft recipe selector class.
// you will check all craft recipes stored by if they contain any ingredient in inventory.