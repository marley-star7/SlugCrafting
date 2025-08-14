namespace SlugCrafting.Core;

public class VisibleItemContainerCycler
{
    private ItemContainer _itemContainer;

    public bool targetedItemVisible;

    public VisibleItemContainerCycler(ItemContainer itemContainer)
    {
        this._itemContainer = itemContainer;
    }

    public int currentlyTargetedSlot;

    public void MoveNext()
    {
        currentlyTargetedSlot++;

        if (currentlyTargetedSlot == _itemContainer.slots.Length)
            currentlyTargetedSlot = 0;
    }

    public void MovePrevious()
    {
        currentlyTargetedSlot--;

        if (currentlyTargetedSlot == -1)
            currentlyTargetedSlot = _itemContainer.slots.Length - 1;
    }

    public AbstractPhysicalObject? PopItemFromTargetedSlot()
    {
        return _itemContainer.PopItemFromSlot(currentlyTargetedSlot);
    }

    public bool HasItemInTargetedSlot()
    {
        return _itemContainer.HasItemInSlot(currentlyTargetedSlot);
    }

    public void PutItemInTargetedSlot(AbstractPhysicalObject abstractPhysicalObject)
    {
        if (!HasItemInTargetedSlot())
        {
            if (targetedItemVisible)
                _itemContainer.PutItemInSlot(abstractPhysicalObject, currentlyTargetedSlot);
            else
                _itemContainer.PutItemInSlotAndAbstractize(abstractPhysicalObject, currentlyTargetedSlot);
        }
    }

    public void UpdateShowTargetedSlotItem(Vector2 pos, Vector2 rotation)
    {
        var currentlyTargtedItem = _itemContainer.GetItemInSlot(currentlyTargetedSlot);

        if (currentlyTargtedItem == null)
            return;

        if (currentlyTargtedItem.realizedObject == null)
            currentlyTargtedItem.RealizeInRoom();

        currentlyTargtedItem.realizedObject.firstChunk.pos = pos;
    }
}