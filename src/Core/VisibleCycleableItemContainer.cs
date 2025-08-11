namespace SlugCrafting.Core;

public class VisibleItemContainerCycler
{
    public ItemContainer itemContainer;

    public VisibleItemContainerCycler(ItemContainer itemContainer)
    {
        this.itemContainer = itemContainer;
    }

    public int currentlyTargetedSlot;

    public void MoveNext()
    {
        currentlyTargetedSlot++;

        if (currentlyTargetedSlot == itemContainer.slots.Length)
            currentlyTargetedSlot = 0;
    }

    public void MovePrevious()
    {
        currentlyTargetedSlot--;

        if (currentlyTargetedSlot == -1)
            currentlyTargetedSlot = itemContainer.slots.Length - 1;
    }

    public AbstractPhysicalObject? PopItemFromTargetedSlot()
    {
        return itemContainer.PopItemFromSlot(currentlyTargetedSlot);
    }

    public void GetItemBundleInTargetedSlot()
    {
        itemContainer.GetItemBundleInSlot(currentlyTargetedSlot);
    }

    public void PutItemInTargetedSlot(AbstractPhysicalObject abstractPhysicalObject)
    {
        itemContainer.PutItemInSlot(abstractPhysicalObject, currentlyTargetedSlot);
    }

    public void UpdateShowTargetedSlotItemBundle(Vector2 pos)
    {
        var currentlyTargtedBundle = itemContainer.GetItemBundleInSlot(currentlyTargetedSlot);

        if (currentlyTargtedBundle == null)
            return;

        if (!currentlyTargtedBundle.isRealized)
            currentlyTargtedBundle.RealizeInRoom();
    }
}