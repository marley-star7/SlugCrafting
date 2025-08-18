
namespace SlugCrafting;

internal static class RegionStateHooks
{
    internal static void RegionState_AdaptRegionStateToWorld(On.RegionState.orig_AdaptRegionStateToWorld orig, RegionState self, int playerShelter, int activeGate)
    {
        // -- Ms7: Loop through all the objects in player's shelter room to see if it would change during hibernation.

        List<AbstractWorldEntity> newRoomEntityList = new List<AbstractWorldEntity>();

        var playerShelterAbstractRoom = self.world.GetAbstractRoom(playerShelter);
        for (int i = 0; i < playerShelterAbstractRoom.entities.Count; i++)
        {
            var entity = playerShelterAbstractRoom.entities[i];
            if (entity is AbstractPhysicalObject abstractPhysicalObject)
            {
                if (entity is AbstractLizardHeadShell abstractLizardShell && abstractLizardShell.type == SlugCraftingEnums.AbstractObjectType.GreenLizardHeadShell)
                {
                    // Replace the add with a green liz shell helmet instead.
                    newRoomEntityList.Add(new AbstractLizardShellHelmet(abstractLizardShell, SlugCraftingEnums.AbstractObjectType.GreenLizardShellHelmet, playerShelterAbstractRoom.realizedRoom.game.GetNewID()));
                    continue;
                }
            }

            newRoomEntityList.Add(entity);
        }

        playerShelterAbstractRoom.entities = newRoomEntityList;

        orig(self, playerShelter, activeGate);
    }
}
