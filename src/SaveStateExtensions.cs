namespace SlugCrafting;

public class SaveStateCraftingData
{
    public Inventory playerShelterInventory;

    public WeakReference<SaveState> saveStateRef;

    public SaveStateCraftingData(SaveState saveState)
    {
        saveStateRef = new WeakReference<SaveState>(saveState);
    }
}

public static class SaveStateExtensions
{
    private static readonly ConditionalWeakTable<SaveState, SaveStateCraftingData> _craftingDataTable = new();

    public static SaveStateCraftingData GetSaveStateCraftingData(this SaveState saveState) =>
        _craftingDataTable.GetValue(saveState, s => new SaveStateCraftingData(s));
}