using SlugCrafting.Crafts;

namespace SlugCrafting.Menus;

public class ShelterCraftSelector : CraftRecipesSelector
{
    public ShelterCraftSelector(Menu.Menu menu, MenuObject owner, Vector2 pos, Vector2 size) : base(menu, owner, pos, size)
    {
    }

    public override void Singal(MenuObject sender, string message)
    {
        if (message == "CRAFTRESULTCLICKED")
        {
            var craftRecipeItem = sender as CraftRecipeItem;
            if (Content.ShelterCrafts.TryGetValue(craftRecipeItem.recipe.craftID, out var shelterCraft))
            {
                ShowShelterCraftInformation(shelterCraft);
                return;
            }
        }

        // Continue with rest of signal.
        base.Singal(sender, message);
    }

    public override bool IsRecipeCraftable(CraftRecipe recipe)
    {
        if (base.IsRecipeCraftable(recipe)
            && recipe.IsShelterCraft())
            return true;

        return false;
    }

    public void ShowShelterCraftInformation(ShelterCraft shelterCraft)
    {
        craftRecipeElaborationDisplay = new CraftableCraftRecipeElaborationDisplay(shelterCraft, menu, this, this.pos + new Vector2(200, 0), new Vector2(600, 900));
        this.subObjects.Add(craftRecipeElaborationDisplay);
    }
}
