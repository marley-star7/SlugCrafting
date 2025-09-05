using SlugCrafting.Crafts;
using static SlugCrafting.Crafts.ShelterCraft;
using static SlugCrafting.Menus.ShelterCraftScreen;

namespace SlugCrafting.Menus;

public class ShelterCraftSelector : CraftRecipesSelector
{
    protected ShelterCraftScreenDataPackage dataPackage;

    public ShelterCraftSelector(Menu.Menu menu, MenuObject owner, Vector2 pos, Vector2 size) : base(menu, owner, pos, size)
    {
    }

    protected void PerformShelterCraft(ShelterCraft shelterCraft)
    {
        Plugin.LogDebug($"Performing shelter craft in craft menu!");

        var ingredients = shelterCraft.recipe.ingredients;
        var objectsToUseAsMaterials = new ShelterCraftResultData.MaterialResultData[shelterCraft.recipe.GetTotalIngredientsQuantityRequired()];
        var materialUsedIndex = 0;

        for (int i = 0; i < ingredients.Length; i++)
        {
            for (int materialNum = 0; materialNum < ingredients[i].quantityRequired; materialNum++)
            {
                var materialEntityTypeDef = ingredients[i].material.entityTypeDefinition;
                var material = PlayerInventory.Items[materialEntityTypeDef][materialNum];

                // Add this material to the ones to be used, and increase the index to ensure the next material found is the one set.
                objectsToUseAsMaterials[materialUsedIndex] = new ShelterCraftResultData.MaterialResultData(material.entityID, ingredients[i].consumed);
                materialUsedIndex++;

                PlayerInventory.RemoveItem(material);
            }
        }

        var currentSaveState = menu.manager.rainWorld.progression.currentSaveState;
        var sleepAndDeathScreenDataPackage = dataPackage.sleepAndDeathScreenDataPackage;
        var playerRegionIndex = currentSaveState.GetRegionStateIndexByRegionName(sleepAndDeathScreenDataPackage.sessionRecord.wentToSleepInRegion);
        var playerRegion = currentSaveState.GetRegionStateByIndex(playerRegionIndex);

        ShelterCraftResultData shelterCraftResultData = new ShelterCraftResultData(
            objectsToUseAsMaterials, 
            shelterCraft.recipe.craftID, 
            new WorldCoordinate(sleepAndDeathScreenDataPackage.playerRoom, (int)sleepAndDeathScreenDataPackage.playerPos.x, (int)sleepAndDeathScreenDataPackage.playerPos.y, -1)
        );

        playerRegion.GetRegionStateCraftingData().AddRoomShelterCraftToDoOnWakeup(shelterCraftResultData);

        OnInventoryItemsChanged();
    }

    public void SetShelterCraftScreenDataPackage(ShelterCraftScreen.ShelterCraftScreenDataPackage dataPackage)
    {
        this.dataPackage = dataPackage;
        SetPlayerInventory(dataPackage.playerShelterInventory);
    }

    public override void Singal(MenuObject sender, string message)
    {
        if (message == "CRAFTRESULTCLICKED")
        {
            var craftRecipeItem = ((CraftRecipeItem.Button)sender).owner;
            if (Content.ShelterCrafts.TryGetValue(craftRecipeItem.recipe.craftID, out var shelterCraft))
            {
                ShowShelterCraftInformation(shelterCraft);
                return;
            }
        }
        if (message == "CRAFT")
        {
            var recipeToCraft = ((CraftRecipeElaborationDisplay)(sender.owner)).recipe;
            if (Content.ShelterCrafts.TryGetValue(recipeToCraft.craftID, out var shelterCraft))
            {
                PerformShelterCraft(shelterCraft);
            }
            return;
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
        craftRecipeElaborationDisplay = new CraftableCraftRecipeElaborationDisplay(shelterCraft.recipe, menu, this, recipeElaborationDisplayPlacementPosition, new Vector2(600, 900));
        this.subObjects.Add(craftRecipeElaborationDisplay);
    }
}
