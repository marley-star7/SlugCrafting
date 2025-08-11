namespace SlugCrafting.Core;

/// <summary>
/// A method of storing stacked items,
/// </summary>
public sealed class ItemBundle
{
    public AbstractPhysicalObject primaryObject => items.First();

    private List<AbstractPhysicalObject> items;

    public int count { get { return items.Count; } }

    public bool isRealized => firstItem.realizedObject != null;

    /// <summary>
    /// Every bundle has a controlling bundle item, being the first item in the bundles list.
    /// This item is the item who's rotation and position will be followed, as well as the only one valid for grabbing.
    /// </summary>
    /// <param name="firstItem"></param>
    /// <param name="type"></param>
    public ItemBundle(AbstractPhysicalObject firstItem)
    {
        var maxBundleSize = 1;
        if (Content.ItemsBundleProperties.ContainsKey(firstItem.type))
            maxBundleSize = Content.ItemsBundleProperties[firstItem.type].maxBundleSize;

        items = new List<AbstractPhysicalObject>();
        // Cap the capacity because don't need to hold more.
        items.Capacity = maxBundleSize;
        items.Add(firstItem);

        if (firstItem.realizedObject != null)
            firstItem.realizedObject.GetPhysicalObjectCraftingData().bundle = this;
    }

    public AbstractPhysicalObject firstItem
    {
        get { return items.First(); }
    }

    public bool CanBundleWith(AbstractPhysicalObject abstractPhysicalObject)
    {
        if (items.Contains(abstractPhysicalObject) || abstractPhysicalObject.type != firstItem.type)
            return false;

        return true;
    }

    public bool TryAddItem(AbstractPhysicalObject itemToAdd)
    {
        var can = CanBundleWith(itemToAdd);
        if (can)
            AddItem(itemToAdd);

        return can;
    }

    public void AddItem(AbstractPhysicalObject item)
    {
        items.Add(item);

        if (item.realizedObject != null)
        {
            item.realizedObject.AllGraspsLetGoOfThisObject(true);

            if (isRealized)
            {
                item.realizedObject.GetPhysicalObjectCraftingData().bundle = this; // Make sure the item's bundle is same as this one.
            }
            else
            {
                item.realizedObject.Destroy();
            }
        }
    }

    public void RemoveItem(AbstractPhysicalObject item)
    {
        if (!items.Contains(item))
            return;

        items.Remove(item);

        if (item.realizedObject != null)
        {
            item.realizedObject.GetPhysicalObjectCraftingData().bundle = new ItemBundle(item); // Item no longer belongs to this bundle, so set it to null.
            item.realizedObject.AllGraspsLetGoOfThisObject(true);
        }
    }

    public AbstractPhysicalObject? PopItem()
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

    public void RemoveFromRoomAndAbstractize()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].realizedObject.RemoveFromRoom();
            items[i].Abstractize(primaryObject.pos);
        }
    }

    public void RealizeInRoom()
    {
        for (int i = 0; i < items.Count; i++)
        {
            primaryObject.Room.AddEntity(items[i]);
            items[i].pos = primaryObject.pos;
            items[i].RealizeInRoom();
        }
    }
}
