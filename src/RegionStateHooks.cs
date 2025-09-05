using SlugBase.SaveData;

namespace SlugCrafting;

internal static class RegionStateHooks
{
    internal static void ApplyHooks()
    {
        On.RegionState.AdaptWorldToRegionState += RegionStateHooks.RegionState_AdaptWorldToRegionState;
        On.RegionState.AdaptRegionStateToWorld += RegionStateHooks.RegionState_AdaptRegionStateToWorld;

        On.RegionState.SaveToString += RegionStateHooks.RegionState_SaveToString;
    }

    internal static void RemoveHooks()
    {
        On.RegionState.AdaptWorldToRegionState -= RegionStateHooks.RegionState_AdaptWorldToRegionState;
        On.RegionState.AdaptRegionStateToWorld -= RegionStateHooks.RegionState_AdaptRegionStateToWorld;

        On.RegionState.SaveToString -= RegionStateHooks.RegionState_SaveToString;
    }

    private const string RegionShelterCraftsSaveDataKey = "RegionShelterCrafts";

    private static void PerformMaterialResultOnEntity(AbstractWorldEntity entity, in ShelterCraftResultData.MaterialResultData entityMaterialResultData)
    {
        if (entityMaterialResultData.consumed)
        {
            entity.Room.RemoveEntity(entity);
        }
    }

    private static void PerformMaterialResultsInRoom(in AbstractRoom room, in List<ShelterCraftResultData.MaterialResultData> materialResultDatas)
    {
        //-- Ms7: For extra optimization, we could swap to a swapback array instead of a list, and then remove materials inbetween loops to check for when we have already completed them.
        // We would use a swapback array because removing from a list has it re-ordering itself, which is extra overhead.

        // Try to find the material matching entity in room.
        for (int roomEntityIndex = 0; roomEntityIndex < room.entities.Count; roomEntityIndex++)
        {
            var roomEntity = room.entities[roomEntityIndex];

            for (int materialIndex = 0; materialIndex < materialResultDatas.Count; materialIndex++)
            {
                // Once entity Id's match, will perform rest of checks.
                if (roomEntity.ID != materialResultDatas[materialIndex].entityID)
                {
                    continue;
                }

                PerformMaterialResultOnEntity(roomEntity, materialResultDatas[materialIndex]);
            }
        }
    }

    private static void PerformShelterCraftsResults(RegionState regionState, List<ShelterCraftResultData> shelterCraftResults)
    {
        var materialResultsToPerformInRoom = new Dictionary<int, List<ShelterCraftResultData.MaterialResultData>>();

        for (int i = 0; i < shelterCraftResults.Count; i++)
        {
            var shelterCraft = Content.ShelterCrafts[shelterCraftResults[i].craftID];

            //-- Ms7: For performance reasons in looping, we add everything to perform first,
            // And then loop over later since have to check against id's, more efficient.
            for (int materialIndex = 0; materialIndex < shelterCraftResults[i].materials.Length; materialIndex++)
            {
                if (!materialResultsToPerformInRoom.ContainsKey(shelterCraftResults[i].coord.room))
                    materialResultsToPerformInRoom.Add(shelterCraftResults[i].coord.room, new());

                materialResultsToPerformInRoom[shelterCraftResults[i].coord.room].Add(shelterCraftResults[i].materials[materialIndex]);
            }

            // Actually perform the shelter craft first!
            shelterCraft.craftResult(regionState.world, shelterCraftResults[i]);
        }

        //-- Ms7: Afterwards perform any material results together by room, using the most ever forever optimized loopagesings.
        foreach (KeyValuePair<int, List<ShelterCraftResultData.MaterialResultData>> roomMaterialResults in materialResultsToPerformInRoom)
        {
            var room = regionState.world.GetAbstractRoom(roomMaterialResults.Key);
            PerformMaterialResultsInRoom(room, roomMaterialResults.Value);
        }
    }

    /// <summary>
    /// Occurs when game SaveState is bringing the region state back from it's save?
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="self"></param>
    private static void RegionState_AdaptWorldToRegionState(On.RegionState.orig_AdaptWorldToRegionState orig, RegionState self)
    {
        orig(self);

        var selfCraftingData = self.GetRegionStateCraftingData();

        var slugBaseSaveData = SaveDataExtension.GetSlugBaseData(self.saveState.miscWorldSaveData);
        if (slugBaseSaveData.TryGet(RegionShelterCraftsSaveDataKey, out Dictionary<string, List<ShelterCraftResultData>> allRegionPerformedShelterCrafts)
            && allRegionPerformedShelterCrafts.TryGetValue(self.regionName, out List<ShelterCraftResultData> performedShelterCrafts))
        {
            Plugin.LogDebug($"DID IT!!! {performedShelterCrafts[0].craftID}");
            PerformShelterCraftsResults(self, performedShelterCrafts);

            // When all the crafts have been performed, remove that data.
            allRegionPerformedShelterCrafts.Remove(self.regionName);
        }
    }

    /// <summary>
    /// Occurs when the game SaveState is bringing the region state up to date before hibernation.
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="self"></param>
    /// <param name="playerShelter"></param>
    /// <param name="activeGate"></param>
    private static void RegionState_AdaptRegionStateToWorld(On.RegionState.orig_AdaptRegionStateToWorld orig, RegionState self, int playerShelter, int activeGate)
    {
        MenuInventory shelterInventory = new MenuInventory();

        var playerShelterAbstractRoom = self.world.GetAbstractRoom(playerShelter);

        // Loop through all entities in the shelter room and add them to the inventory.
        for (int i = 0; i < playerShelterAbstractRoom.entities.Count; i++)
        {
            var entity = playerShelterAbstractRoom.entities[i];
            if (entity is AbstractPhysicalObject abstractPhysicalObject 
                && entity is not AbstractCreature abstractCreature)
            {
                shelterInventory.AddItem(new EntityMenuData(abstractPhysicalObject));
            }
        }

        self.GetRegionStateCraftingData().shelterInventories[playerShelter] = shelterInventory;
        self.saveState.GetSaveStateCraftingData().playerShelterInventory = shelterInventory;
        
        orig(self, playerShelter, activeGate);
    }

    private static string RegionState_SaveToString(On.RegionState.orig_SaveToString orig, RegionState self)
    {
        var originalSaveText = orig(self);
        var craftingData = self.GetRegionStateCraftingData();

        // -- Ms7: I just do this in MiscWorldCraftingData, because slugbase makes it easy lol.
        var slugBaseSaveData = SaveDataExtension.GetSlugBaseData(self.saveState.miscWorldSaveData);
        slugBaseSaveData.Set(RegionShelterCraftsSaveDataKey, self.saveState.miscWorldSaveData.GetMiscWorldCraftingSaveData().RoomShelterCraftsToDoOnWakeup);
       
        return originalSaveText;
    }
}
