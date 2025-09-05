namespace SlugCrafting;

public class RegionStateCraftingData
{
    /// <summary>
    /// Int is the player room index.
    /// </summary>
    public Dictionary<int, MenuInventory> shelterInventories = new();

    public WeakReference<RegionState> RegionStateRef;

    public RegionStateCraftingData(RegionState RegionState)
    {
        RegionStateRef = new WeakReference<RegionState>(RegionState);
    }

    public void AddRoomShelterCraftToDoOnWakeup(ShelterCraftResultData shelterCraftResultData)
    {
        if (!RegionStateRef.TryGetTarget(out var regionState))
            return;

        regionState.saveState.miscWorldSaveData.GetMiscWorldCraftingSaveData().AddRoomShelterCraftToDoOnWakeup(regionState.regionName, shelterCraftResultData);
    }

    internal void RemoveAllConsumableIngredientsForShelterCraft(AbstractRoom abstractRoom, ShelterCraftResultData shelterCraftResultData)
    {
        for (int i = 0; i < abstractRoom.entities.Count; i++)
        {
            for (int j = 0; j < shelterCraftResultData.materials.Length; j++)
            {
                if (abstractRoom.entities[i].ID != shelterCraftResultData.materials[j].entityID)
                    continue;

                // Matching loaded entity found, doing stuff to it.
                //PerformMaterialResultsOnEntity(abstractRoom.entities[i], materialMenuData.Value);
                break;
            }
        }
    }
}

public static class RegionStateExtensions
{
    private static readonly ConditionalWeakTable<RegionState, RegionStateCraftingData> _craftingDataTable = new();

    public static RegionStateCraftingData GetRegionStateCraftingData(this RegionState RegionState) =>
        _craftingDataTable.GetValue(RegionState, s => new RegionStateCraftingData(s));
}