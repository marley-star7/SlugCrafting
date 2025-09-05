namespace SlugCrafting.Menus;

public class CraftableCraftRecipeElaborationDisplay : CraftRecipeElaborationDisplay
{
    /// <summary>
    /// Creates a information with an active shelter craft button for respective craft.
    /// </summary>
    /// <param name="recipe"></param>
    /// <param name="menu"></param>
    /// <param name="owner"></param>
    /// <param name="pos"></param>
    /// <param name="size"></param>
    public CraftableCraftRecipeElaborationDisplay(CraftRecipe recipe, Menu.Menu menu, CraftRecipesSelector owner, Vector2 pos, Vector2 size) : base(recipe, menu, owner, pos, size)
    {
        AddCraftButton();
    }
}
