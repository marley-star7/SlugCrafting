namespace SlugCrafting.Menus;

public class CraftableCraftRecipeElaborationDisplay : CraftRecipeElaborationDisplay
{
    public readonly ShelterCraft shelterCraft;

    /// <summary>
    /// Creates a information with an active shelter craft button for respective craft.
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="owner"></param>
    /// <param name="shelterCraft"></param>
    /// <param name="pos"></param>
    /// <param name="size"></param>
    public CraftableCraftRecipeElaborationDisplay(ShelterCraft shelterCraft, Menu.Menu menu, CraftRecipesSelector owner, Vector2 pos, Vector2 size) : base(shelterCraft.recipe, menu, owner, pos, size)
    {
        this.shelterCraft = shelterCraft;

        AddCraftButton();
    }

    private void AddCraftButton()
    {
        var craftButtonPos = new Vector2(160, 90);
        var craftButtonSize = new Vector2(90, 32);
        craftButton = new BigSimpleButton(menu, owner, "Craft", "CRAFT", craftButtonPos, craftButtonSize, FLabelAlignment.Center, true);

        this.subObjects.Add(craftButton);
    }

}
