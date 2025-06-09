using SlugCrafting.Core;

namespace SlugCrafting.Items;

public class ItemBundle
{
    public delegate bool Validation();

    public Validation addableToBundleValidation = () => false;

    public PlayerCarryableItem firstItem
    {
        get => items[0];
    }
    public readonly List<PlayerCarryableItem> items;

    public ItemBundle(AbstractPhysicalObject.AbstractObjectType type)
    {
        var maxBundleSize = Content.ItemsBundleProperties[type].maxBundleSize;

        items = new List<PlayerCarryableItem>();

        items.Capacity = maxBundleSize;
    }

    public void AddItem(PlayerCarryableItem item)
    {
        if (items.Contains(item))
            return;

        items.Add(item);
        //-- Make sure the item's bundle is same as this one.
        item.GetPlayerCarryableItemCraftingData().bundle = this;
    }

    public void RemoveItem(PlayerCarryableItem item)
    {
        if (!items.Contains(item))
            return;

        items.Remove(item);
        //-- Item no longer belongs to this bundle, so set it to null.
        item.GetPlayerCarryableItemCraftingData().bundle = null;
    }
}
