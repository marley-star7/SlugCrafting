using SlugCrafting.Core;

namespace SlugCrafting.Items;

/// <summary>
/// A method of storing stacked items,
/// </summary>
public class ItemBundle
{
    public delegate bool Validation();

    public Validation addableToBundleValidation = () => false;

    public PlayerCarryableItem firstItem
    {
        get { return items.First(); }
    }

    private List<PlayerCarryableItem> items;

    public int count { get { return items.Count; } }

    /// <summary>
    /// Every bundle has a controlling bundle item, being the first item in the bundles list.
    /// This item is the item who's rotation and position will be followed, as well as the only one valid for grabbing.
    /// </summary>
    /// <param name="firstItem"></param>
    /// <param name="type"></param>
    public ItemBundle(PlayerCarryableItem firstItem, AbstractPhysicalObject.AbstractObjectType type)
    {
        var maxBundleSize = SlugCrafting.Core.Content.ItemsBundleProperties[type].maxBundleSize;

        items = new List<PlayerCarryableItem>();
        // Cap the capacity because don't need to hold more.
        items.Capacity = maxBundleSize;
        items.Add(firstItem);
    }

    public void AddItem(PlayerCarryableItem item)
    {
        if (items.Contains(item) || item.abstractPhysicalObject.type != firstItem.abstractPhysicalObject.type)
            return;

        items.Add(item);
        item.GetPlayerCarryableItemCraftingData().bundle = this; // Make sure the item's bundle is same as this one.

        item.AllGraspsLetGoOfThisObject(true);
    }

    public void RemoveItem(PlayerCarryableItem item)
    {
        if (!items.Contains(item))
            return;

        items.Remove(item);
        item.GetPlayerCarryableItemCraftingData().bundle = null; // Item no longer belongs to this bundle, so set it to null.

        item.AllGraspsLetGoOfThisObject(true);
    }

    public PlayerCarryableItem? PopItem()
    {
        if (items.Count > 0)
        {
            var item = items[items.Count - 1];
            RemoveItem(item);
            return item;
        }
        else
            return null;
    }
}
