using RWCustom;
using UnityEngine;

using SlugCrafting.Items;
using MRCustom.Animations;
using MRCustom.Math;

namespace SlugCrafting;

public static partial class Hooks
{
    private static void ApplyPlayerCarryableItemHooks()
    {
        On.PlayerCarryableItem.Update += PlayerCarryableItem_Update;
    }

    private static void RemovePlayerCarryableItemHooks()
    {
        On.PlayerCarryableItem.Update -= PlayerCarryableItem_Update;
    }

    private static void PlayerCarryableItem_Update(On.PlayerCarryableItem.orig_Update orig, PlayerCarryableItem selfItem, bool eu)
    {
        orig(selfItem, eu);

        //
        // BUNDLE UPDATES
        //

        var selfItemCraftingData = selfItem.GetPlayerCarryableItemCraftingData();

        //-- Other bundled items refrence the firsts position.
        if (selfItemCraftingData.bundle == null || selfItemCraftingData.bundle.firstItem == selfItem)
            return;

        //-- If the item is not the first item in the bundle, then update its position to match the first item.
        var firstItem = selfItemCraftingData.bundle.firstItem;
        selfItem.firstChunk.pos = firstItem.firstChunk.pos; // TODO: need to add bundle offset.
        selfItem.firstChunk.vel = firstItem.firstChunk.vel;
    }

}

