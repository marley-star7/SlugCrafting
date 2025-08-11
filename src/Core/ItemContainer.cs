namespace SlugCrafting.Core;

public class ItemContainer
{
    public struct Slot
    {
        public ItemBundle? items;
    }

    public Slot[] slots;

    public ItemContainer(AbstractPhysicalObject owner, Slot[] slots)
    {
        this.slots = slots;
    }

    public ItemContainer(AbstractPhysicalObject owner, int numOfSlots)
    {
        this.slots = new Slot[numOfSlots];
    }

    public bool HasItemBundleInSlot(int slotNum)
    {
        if (slotNum > slots.Length || slotNum < 0)
            return false;

        if (slots[slotNum].items == null)
            return false;

        return true;
    }

    public void PutItemInSlot(AbstractPhysicalObject item, int slotNum)
    {
        if (slots[slotNum].items == null)
            slots[slotNum].items = new ItemBundle(item);

        slots[slotNum].items.AddItem(item);
    }

    public void PutItemBundleInSlot(ItemBundle bundle, int slotNum)
    {
        bundle.RemoveFromRoomAndAbstractize();
        slots[slotNum].items = bundle;
    }

    public ItemBundle? GetItemBundleInSlot(int slotNum)
    {
        return slots[slotNum].items;
    }

    public AbstractPhysicalObject? PopItemFromSlot(int slotNum)
    {
        if (slots[slotNum].items == null)
            return null;

        return slots[slotNum].items.PopItem();
    }
}
