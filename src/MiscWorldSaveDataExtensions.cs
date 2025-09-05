namespace SlugCrafting;

public class MiscWorldCraftingSaveData
{
    private Dictionary<string, List<ShelterCraftResultData>> _roomShelterCraftsToDoOnWakeup = new();

    /// <summary>
    /// string is the regionName to do the craft in.
    /// </summary>
    public Dictionary<string, List<ShelterCraftResultData>> RoomShelterCraftsToDoOnWakeup
    {
        get => _roomShelterCraftsToDoOnWakeup;
    }


    public WeakReference<MiscWorldSaveData> miscWorldSaveDataRef;

    public MiscWorldCraftingSaveData(MiscWorldSaveData miscWorldSaveData)
    {
        miscWorldSaveDataRef = new WeakReference<MiscWorldSaveData>(miscWorldSaveData);
    }

    public void AddRoomShelterCraftToDoOnWakeup(string regionName, ShelterCraftResultData shelterCraftResultData)
    {
        if (!_roomShelterCraftsToDoOnWakeup.ContainsKey(regionName))
        {
            _roomShelterCraftsToDoOnWakeup.Add(regionName, new());
        }

        _roomShelterCraftsToDoOnWakeup[regionName].Add(shelterCraftResultData);
    }
}

public static class MiscWorldSaveDataExtensions
{
    private static readonly ConditionalWeakTable<MiscWorldSaveData, MiscWorldCraftingSaveData> _craftingDataTable = new();

    public static MiscWorldCraftingSaveData GetMiscWorldCraftingSaveData(this MiscWorldSaveData miscWorldSaveData) =>
        _craftingDataTable.GetValue(miscWorldSaveData, s => new MiscWorldCraftingSaveData(s));
}