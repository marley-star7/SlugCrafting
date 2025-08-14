namespace SlugCrafting.Core;

public class ItemContainer
{
    public class InsideItemContainerStick : AbstractPhysicalObject.AbstractObjectStick
    {
        public InsideItemContainerStick(AbstractPhysicalObject container, AbstractPhysicalObject objectInside) : base(container, objectInside)
        {
        }
    }

    public struct Slot
    {
        public AbstractPhysicalObject? items;
    }

    public Slot[] slots;

    public ItemContainer(AbstractPhysicalObject owner, Slot[] slots)
    {
        this.owner = owner;
        this.slots = slots;
    }

    public ItemContainer(AbstractPhysicalObject owner, int numOfSlots)
    {
        this.owner = owner;
        this.slots = new Slot[numOfSlots];
    }

    public AbstractPhysicalObject owner;

    public bool HasItemInSlot(int slotNum)
    {
        if (slotNum > slots.Length || slotNum < 0)
            return false;

        if (slots[slotNum].items == null)
            return false;

        return true;
    }

    // TODO: slots hold a single abstractphysical item, but use bundling data to stick items to that abstractphysical item inside (putting) them together.
    // Have it be checks for if items are stuck to the abstractphysical object, and of what kind, individual case scenarios for the sticks wether they should make it shareable with a bundle or not.
    // TODO: maybe pressing grab on a backpack pulls of them out, so bundling functionality still works within it, pulling and taking from the items stack.
    // 1. change containers to just hold a single abstractPhysicalObject in each slot, set up code that way.
    // 2. manage to be able to succesfully put and take an item out of a slot.
    // 3. make the item show when in the slot, and when held in your hand.
    // 4. make it so you can cycle between slots.
    public void PutItemInSlot(AbstractPhysicalObject item, int slotNum)
    {
        if (item.realizedObject != null)
        {
            item.realizedObject.AllGraspsLetGoOfThisObject(true);
            new InsideItemContainerStick(owner, item);
        }

        if (slots[slotNum].items == null)
            slots[slotNum].items = item;

        slots[slotNum].items = item;
    }

    public void PutItemInSlotAndAbstractize(AbstractPhysicalObject item, int slotNum)
    {
        PutItemInSlot(item, slotNum);

        if (item.realizedObject != null)
            item.realizedObject.Destroy();
    }

    public AbstractPhysicalObject? GetItemInSlot(int slotNum)
    {
        if (slotNum >= slots.Length || slotNum < 0)
            return null;

        return slots[slotNum].items;
    }

    public AbstractPhysicalObject? PopItemFromSlot(int slotNum)
    {
        var slotsItem = GetItemInSlot(slotNum);

        if (slotsItem == null)
            return null;

        for (int i = 0; i < owner.stuckObjects.Count(); i++)
        {
            if (owner.stuckObjects[i].B == slotsItem)
            {
                owner.stuckObjects[i].Deactivate();
                break;
            }
        }

        return slots[slotNum].items;
    }
}
