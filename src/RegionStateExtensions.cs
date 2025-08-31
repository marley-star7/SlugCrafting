namespace SlugCrafting;

public class RegionStateCraftingData
{
    /// <summary>
    /// Int is the player room index.
    /// </summary>
    public Dictionary<int, Inventory> shelterInventories = new();

    /// <summary>
    /// Int is the player room index.
    /// </summary>
    public Dictionary<int, Queue<(ShelterCraft, ShelterCraft.ShelterCraftResultDataPackage)>> roomShelterCraftsToDoOnWakeup = new();

    public WeakReference<RegionState> RegionStateRef;

    public RegionStateCraftingData(RegionState RegionState)
    {
        RegionStateRef = new WeakReference<RegionState>(RegionState);
    }
}

public static class RegionStateExtensions
{
    private static readonly ConditionalWeakTable<RegionState, RegionStateCraftingData> _craftingDataTable = new();

    public static RegionStateCraftingData GetRegionStateCraftingData(this RegionState RegionState) =>
        _craftingDataTable.GetValue(RegionState, s => new RegionStateCraftingData(s));
}