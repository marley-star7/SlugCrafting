namespace SlugCrafting.Items;

public class PhysicalObjectCraftingData
{
    // Obligitory weak reference to the physical object, so that it can be garbage collected when no longer needed.
    public WeakReference<PhysicalObject> physicalObjectRef;

    public AbstractCord? tiedCord;
    public ItemBundle? bundle;

    public PhysicalObjectCraftingData(PhysicalObject physicalObject)
    {
        physicalObjectRef = new WeakReference<PhysicalObject>(physicalObject);
        //bundle = new ItemBundle(physicalObject.abstractPhysicalObject);
    }
}

public static class PhysicalObjectCraftingExtensions
{
    public static readonly ConditionalWeakTable<PhysicalObject, PhysicalObjectCraftingData> craftingDataConditionalWeakTable = new();

    public static PhysicalObjectCraftingData GetPhysicalObjectCraftingData(this PhysicalObject physicalObject) => craftingDataConditionalWeakTable.GetValue(physicalObject, _ => new PhysicalObjectCraftingData(physicalObject));

    /*
    public static bool IsInContainer(this PhysicalObject self)
    {
        return false;
    }
    */

    public static void BundledStickUpdate(this PhysicalObject self, bool isEvenUpdate)
    {
        // MS7: To make sure recursion doesn't occur, this only runs on even updates.
        // DON'T REMOVE THIS OR YOU WILL GET ERRORLESS BLACK SCREEN.
        //if (!isEvenUpdate)
        //    return;

        for (int i = 0; i < self.abstractPhysicalObject.stuckObjects.Count; i++)
        {
            if (self.abstractPhysicalObject.stuckObjects[i] is BundledItemStick bundledItemStick)
            {
                var realizedStuckObject = bundledItemStick.B.realizedObject;

                if (realizedStuckObject != null && realizedStuckObject.room == self.room)
                {
                    realizedStuckObject.firstChunk.MoveFromOutsideMyUpdate(isEvenUpdate, self.firstChunk.pos);
                    realizedStuckObject.firstChunk.vel *= 0f;
                }
            }
        }
    }

    public static void UpdateSetRotationForImpaledSpearStick(this PhysicalObject self, ref Vector2? setRotation)
    {
        for (int i = 0; i < self.abstractPhysicalObject.stuckObjects.Count; i++)
        {
            if (self.abstractPhysicalObject.stuckObjects[i] is not AbstractPhysicalObject.ImpaledOnSpearStick impaledOnSpearStick)
                continue;

            setRotation = (impaledOnSpearStick.A.realizedObject as Spear).rotation;
            break;
        }
    }

    //
    //-- CONVENIENCE FUNCTIONS FOR BUNDLE INTERACTION
    //

    /*
    public static bool CanBundleWith(this PhysicalObject selfPhysicalObject, AbstractPhysicalObject playerCarryableItemToBundle)
    {
        return selfPhysicalObject.GetPhysicalObjectCraftingData().bundle.CanBundleWith(playerCarryableItemToBundle);
    }

    public static void AddItemToBundle(this PhysicalObject selfPhysicalObject, PhysicalObject playerCarryableItemToAdd)
    {
        var selfCraftingData = selfPhysicalObject.GetPhysicalObjectCraftingData();
        if (selfCraftingData.bundle == null)
        {
            selfCraftingData.bundle = new ItemBundle(selfPhysicalObject, playerCarryableItemToAdd.abstractPhysicalObject.type);
        }

        selfCraftingData.bundle.AddItem(playerCarryableItemToAdd);
    }

    public static void RemoveItemFromBundle(this PhysicalObject selfPhysicalObject, PhysicalObject playerCarryableItemToRemove)
    {
        var selfCraftingData = selfPhysicalObject.GetPhysicalObjectCraftingData();
        if (selfCraftingData.bundle == null)
        {
            return;
        }

        selfCraftingData.bundle.RemoveItem(playerCarryableItemToRemove.abstractPhysicalObject);
    }

    public static PhysicalObject PopItemFromBundle(this PhysicalObject selfPhysicalObject)
    {
        var selfCraftingData = selfPhysicalObject.GetPhysicalObjectCraftingData();
        if (selfCraftingData.bundle == null)
        {
            return null;
        }

        return selfCraftingData.bundle.PopItem().realizedObject;
    }
    */

    public static ItemBundle GetBundle(this PhysicalObject playerCarryableItem)
    {
        return playerCarryableItem.GetPhysicalObjectCraftingData().bundle;
    }
}
