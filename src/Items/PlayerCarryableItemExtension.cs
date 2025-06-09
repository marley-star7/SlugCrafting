using RWCustom;
using UnityEngine;

using System.Runtime.CompilerServices;

namespace SlugCrafting.Items;

public class PlayerCarryableItemCraftingData
{
    // Obligitory weak reference to the physical object, so that it can be garbage collected when no longer needed.
    public WeakReference<PlayerCarryableItem> physicalObjectRef;

    public ItemBundle? bundle;

    public PlayerCarryableItemCraftingData(PlayerCarryableItem physicalObject)
    {
        physicalObjectRef = new WeakReference<PlayerCarryableItem>(physicalObject);
    }
}

public static class PlayerCarryableItemCraftingExtension
{
    public static readonly ConditionalWeakTable<PlayerCarryableItem, PlayerCarryableItemCraftingData> craftingDataConditionalWeakTable = new();

    public static PlayerCarryableItemCraftingData GetPlayerCarryableItemCraftingData(this PlayerCarryableItem physicalObject) => craftingDataConditionalWeakTable.GetValue(physicalObject, _ => new PlayerCarryableItemCraftingData(physicalObject));

    public static void AddItemToBundle(this PlayerCarryableItem selfPlayerCarryableItem, PlayerCarryableItem playerCarryableItemToAdd)
    {
        var selfCraftingData = selfPlayerCarryableItem.GetPlayerCarryableItemCraftingData();
        if (selfCraftingData.bundle == null)
        {
            selfCraftingData.bundle = new ItemBundle(playerCarryableItemToAdd.abstractPhysicalObject.type);
        }

        selfCraftingData.bundle.AddItem(playerCarryableItemToAdd);
    }

    public static void RemoveItemFromBundle(this PlayerCarryableItem selfPlayerCarryableItem, PlayerCarryableItem playerCarryableItemToAdd)
    {
        var selfCraftingData = selfPlayerCarryableItem.GetPlayerCarryableItemCraftingData();
        if (selfCraftingData.bundle == null)
        {
            return;
        }

        selfCraftingData.bundle.RemoveItem(playerCarryableItemToAdd);
    }

    public static ItemBundle? GetBundle(this PlayerCarryableItem playerCarryableItem)
    {
        return playerCarryableItem.GetPlayerCarryableItemCraftingData().bundle;
    }
}
