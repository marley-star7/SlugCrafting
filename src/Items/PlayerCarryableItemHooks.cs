namespace SlugCrafting;

internal static class PlayerCarryableHooks
{
    internal static void PlayerCarryableItem_Update(On.PlayerCarryableItem.orig_Update orig, PlayerCarryableItem selfItem, bool eu)
    {
        orig(selfItem, eu);

        //
        // BUNDLE UPDATES
        //

        var selfItemCraftingData = selfItem.GetPlayerCarryableItemCraftingData();

        //-- Other bundled items refrence the firsts position.
        if (selfItemCraftingData.bundle == null || selfItemCraftingData.bundle.firstItem == selfItem)
            return;

        //
        // ITEM POSITIONING FOR OTHER THAN FIRST
        //

        // Should only be able to grab the first item in a bundle.
        selfItem.forbiddenToPlayer = 1;
        // If the item is not the first item in the bundle, then update its position to match the first item.
        var firstItem = selfItemCraftingData.bundle.firstItem;
        selfItem.firstChunk.pos = firstItem.firstChunk.pos; // TODO: need to add bundle offset.
        selfItem.firstChunk.vel = firstItem.firstChunk.vel;
    }

}

