namespace SlugCrafting.Items;

public class SporePlantCraftingData
{
    public Spear? stuckInSpear;

    public WeakReference<SporePlant> sporePlantRef;

    public SporePlantCraftingData(SporePlant sporePlant)
    {
        sporePlantRef = new WeakReference<SporePlant>(sporePlant);
    }
}

public static class SporePlantExtension
{
    public static readonly ConditionalWeakTable<PlayerCarryableItem, SporePlantCraftingData> craftingDataConditionalWeakTable = new();

    public static SporePlantCraftingData GetSporePlantCraftingData(this SporePlant physicalObject) => craftingDataConditionalWeakTable.GetValue(physicalObject, _ => new SporePlantCraftingData(physicalObject));
}
