using SlugCrafting.Menus;

namespace SlugCrafting;

internal static class RegionStateHooks
{
    internal static void ApplyHooks()
    {
        On.RegionState.AdaptWorldToRegionState += RegionStateHooks.RegionState_AdaptWorldToRegionState;
        On.RegionState.AdaptRegionStateToWorld += RegionStateHooks.RegionState_AdaptRegionStateToWorld;
    }

    internal static void RemoveHooks()
    {
        On.RegionState.AdaptWorldToRegionState -= RegionStateHooks.RegionState_AdaptWorldToRegionState;
        On.RegionState.AdaptRegionStateToWorld -= RegionStateHooks.RegionState_AdaptRegionStateToWorld;
    }

    private static void RegionState_AdaptWorldToRegionState(On.RegionState.orig_AdaptWorldToRegionState orig, RegionState self)
    {
        orig(self);

        var selfCraftingData = self.GetRegionStateCraftingData();

        foreach (KeyValuePair<int, Queue<(ShelterCraft, ShelterCraft.ShelterCraftResultDataPackage)>> roomsShelterCraftsToDo in selfCraftingData.roomShelterCraftsToDoOnWakeup)
        {
            var abstractRoom = self.world.GetAbstractRoom(roomsShelterCraftsToDo.Key);

            foreach (var shelterCraftData in roomsShelterCraftsToDo.Value)
            {
                var shelterCraft = shelterCraftData.Item1;
                var shelterCraftResultDataPackage = shelterCraftData.Item2;

                shelterCraftResultDataPackage.abstractRoom = abstractRoom;

                shelterCraft.craftResult(shelterCraftResultDataPackage);
            }

            roomsShelterCraftsToDo.Value.Clear(); // All shelterCrafts done in this room, clear the queue.
        }
    }

    /// <summary>
    /// Occurs when the game SaveState is brining the region state up to date before hibernation.
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="self"></param>
    /// <param name="playerShelter"></param>
    /// <param name="activeGate"></param>
    private static void RegionState_AdaptRegionStateToWorld(On.RegionState.orig_AdaptRegionStateToWorld orig, RegionState self, int playerShelter, int activeGate)
    {
        Inventory shelterInventory = new Inventory();

        var playerShelterAbstractRoom = self.world.GetAbstractRoom(playerShelter);

        // Loop through all entities in the shelter room and add them to the inventory.
        for (int i = 0; i < playerShelterAbstractRoom.entities.Count; i++)
        {
            var entity = playerShelterAbstractRoom.entities[i];
            if (entity is AbstractPhysicalObject abstractPhysicalObject 
                && entity is not AbstractCreature abstractCreature)
            {
                shelterInventory.AddItem(abstractPhysicalObject);
            }
        }

        self.GetRegionStateCraftingData().shelterInventories[playerShelter] = shelterInventory;
        self.saveState.GetSaveStateCraftingData().playerShelterInventory = shelterInventory;
        
        orig(self, playerShelter, activeGate);
    }
}
