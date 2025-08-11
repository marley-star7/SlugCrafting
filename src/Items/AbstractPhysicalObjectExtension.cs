namespace SlugCrafting.Items;

public class AbstractPhysicalObjectCraftingData
{
    public WeakReference<AbstractPhysicalObject> abstractPhysicalObjectRef;

    public AbstractCord? tiedCord;

    public AbstractPhysicalObjectCraftingData(AbstractPhysicalObject abstractPhysicalObject)
    {
        abstractPhysicalObjectRef = new WeakReference<AbstractPhysicalObject>(abstractPhysicalObject);
    }
}

public static class AbstractPhysicalObjectCraftingExtensions
{
    public static readonly ConditionalWeakTable<AbstractPhysicalObject, AbstractPhysicalObjectCraftingData> craftingDataConditionalWeakTable = new();

    public static AbstractPhysicalObjectCraftingData GetAbstractPhysicalObjectCraftingData(this AbstractPhysicalObject abstractPhysicalObject) => craftingDataConditionalWeakTable.GetValue(abstractPhysicalObject, _ => new AbstractPhysicalObjectCraftingData(abstractPhysicalObject));
}