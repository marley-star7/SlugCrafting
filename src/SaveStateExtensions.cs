namespace SlugCrafting;

public class SaveStateCraftingData
{
    public MenuInventory playerShelterInventory;

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

    public static RegionState? GetRegionStateByRegionName(this SaveState saveState, string regionName)
    {
        var regionStateIndex = GetRegionStateIndexByRegionName(saveState, regionName);
        if (regionStateIndex == -1)
        {
            Plugin.LogWarning($"No region by name: {regionName} in save state progression, cannot get RegionState!");
            return null;
        }

        return saveState.regionStates[regionStateIndex];
    }

    public static RegionState? GetRegionStateByIndex(this SaveState saveState, int regionIndex)
    {
        return saveState.regionStates[regionIndex];
    }

    public static int GetRegionStateIndexByRegionName(this SaveState saveState, string regionName)
    {
        for (int i = 0; i < saveState.progression.regionNames.Length; i++)
        {
            if (saveState.progression.regionNames[i] == regionName)
            {
                return i;
            }
        }

        return -1;
    }
}