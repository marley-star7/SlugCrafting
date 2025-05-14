using UnityEngine;
using RWCustom;

using SlugCrafting.Items;
using SlugCrafting.Scavenges;
using SlugCrafting.Crafts;

using MarMath;
using ImprovedInput;

namespace SlugCrafting;

public static partial class Hooks
{
    // TODO: add different scavenging times saved to the items scavenge data type thingy when you add it.
    private static bool CanMaul(Player scug)
    {
        PlayerCraftingData playerCraftingData = scug.GetCraftingData();

        if (playerCraftingData.craftTimer == 0)
            return true;
        return false;
    }
    private static void Player_EatMeatUpdate(On.Player.orig_EatMeatUpdate orig, Player self, int graspIndex)
    {
        if (CanMaul(self))
            orig(self, graspIndex);
    }

    // Not sure why there is a difference between EatMeatUpdate and MaulingUpdate, nor do I know if this does anything, but just to be safe?
    private static void Player_MaulingUpdate(On.Player.orig_MaulingUpdate orig, Player self, int graspIndex)
    {
        if (CanMaul(self))
            orig(self, graspIndex);
    }

    private static void Player_GrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu)
    {
        // Call the original method
        orig(self, eu);
    }


    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        // Call Scavenge before Craft update.
        ScavengeUpdate(self);
        CraftUpdate(self);

        orig(self, eu); // Call the original method
    }

    //
    // SCAVENGING STUFF
    //

    private static void ItemTrackerUpdate(Player self)
    {
        // TODO: this is what will fill out and save the tracked items avaliable for crafting or scavenging.
        // It will be in the form of a list (or swapback array) of items that are tracked.
        // The list is dynamically updated and removed as items are added or removed from the player's access.
        // Items are also tracked via their location, on the floor, in the player's grasp, backpack, etc.
    }

    private static void ScavengeUpdate(Player self)
    {
        // CHECK FOR VALID ITEMS

        Knife? graspedKnife = null;
        int graspedKnifeGraspIndex = -1;
        PhysicalObject? graspedPhysicalObject = null;
        int graspedPhysicalObjectGraspIndex = -1;

        for (int i = 0; i < self.grasps.Length; i++)
        {
            var grasp = self.grasps[i];
            if (grasp == null)
                continue;

            // CREATURE CHECKING
            // Grabbed chunk takes priority first, because can be shared with item and creature.
            if (grasp.grabbedChunk.owner is Creature)
            {
                graspedPhysicalObjectGraspIndex = i;
                graspedPhysicalObject = grasp.grabbed;
            }
            // ITEM CHECKING
            // Grabbed knife overrides grabbed chunk if detected then.
            if (grasp.grabbed is Knife)
            {
                graspedKnifeGraspIndex = i;
                graspedKnife = grasp.grabbed as Knife;
            }
            else if (grasp.grabbed is PhysicalObject)
            {
                graspedPhysicalObjectGraspIndex = i;
                graspedPhysicalObject = grasp.grabbed;
            }
        }

        // CHECK IF WANTS TO SCAVENGE

        var playerCraftingData = self.GetCraftingData();

        if (self.IsPressed(Inputs.Input.Scavenge) && graspedKnife != null && graspedPhysicalObject != null)
        {
            graspedKnife.BladeColor = Color.green; // TEMP

            playerCraftingData.scavengeTimer++;

            // Cooldown before scavenge starts.
            if (playerCraftingData.scavengeTimer < 10)
                return;

            var currentScavenge = playerCraftingData.currentScavenge;

            // If player is not currently scavenging a spot, try to find one to scavenge.
            if (currentScavenge == null)
            {
                CreatureScavengeData scavengeData = null;
                // SCAVENGING CURRENTLY ONLY WORKS FOR CREATURES
                if (graspedPhysicalObject is Creature)
                    scavengeData = (graspedPhysicalObject as Creature).GetCreatureScavengeData();
                // If no scavenge data, return.
                if (scavengeData == null)
                    return;

                var scavengeSpot = new ScavengeSpot(self.grasps[graspedPhysicalObjectGraspIndex].grabbedChunk.index, 0, 0);
                currentScavenge = scavengeData.GetScavenge(scavengeSpot);
                // If the grabbed scavenging spot or already scavenged, then search for one that isn't.
                if (scavengeSpot == null || currentScavenge.canScavenge == false)
                    scavengeData.GetNearestValidScavenge(scavengeSpot);
                // If STILL null, there are no valid scavenge spots, return.
                if (scavengeSpot == null)
                    return;
                else // If we found a new valid scavenge spot diff from orig, grab that one's chunk instead.
                    self.grasps[graspedPhysicalObjectGraspIndex].chunkGrabbed = scavengeSpot.bodyChunkIndex;

                // Save the scavenge data so we don't have to waste time searching for it again.
                playerCraftingData.currentScavengeSpot = scavengeSpot;
                playerCraftingData.currentScavenge = currentScavenge;
                playerCraftingData.currentHandAnimation = currentScavenge.animation;
            }

            // DO ANIMATION / SCAVENGE IF WE FOUND A SPOT

            if (currentScavenge.canScavenge == true)
            {
                currentScavenge.scavengeTime--;

                if (currentScavenge.scavengeTime > 0)
                    currentScavenge.animation.PlaySlugcatAnimation(self, graspedKnifeGraspIndex, graspedPhysicalObjectGraspIndex, playerCraftingData.scavengeTimer);
                else // Scavenge is done!
                {
                    AbstractPhysicalObject scavengedAbstractObject = currentScavenge.Scavenge();

                    // GRAB OBJECT IF WE SCAVENGED ANYTHING
                    if (scavengedAbstractObject != null)
                    {
                        self.room.abstractRoom.AddEntity(scavengedAbstractObject);
                        scavengedAbstractObject.RealizeInRoom();

                        self.ReleaseGrasp(graspedPhysicalObjectGraspIndex);
                        self.SlugcatGrab(scavengedAbstractObject.realizedObject, self.FreeHand());
                        playerCraftingData.scavengeTimer = 0; // Reset the timer
                    }
                }
            }
            else
            {
                playerCraftingData.currentScavengeSpot = null; // Reset the scavenge spot if we can't scavenge anymore.
                playerCraftingData.currentScavenge = null; // Reset the scavenge data if we can't scavenge anymore.
                playerCraftingData.scavengeTimer = 0; // Reset the timer to scavenge if not pressing scavenge.
            }
        }
    }

    //
    // CRAFTING STUFF
    //

    private static void CraftUpdate(Player self)
    {
        var playerCraftingData = self.GetCraftingData();

        // Requirements for Crafting
        if (!self.IsPressed(Inputs.Input.Craft) 
            || self.grasps[0] == null 
            || self.grasps[1] == null )
        {
            playerCraftingData.craftTimer = 0; // Reset the timer to craft if not pressing craft.
            return; // No ingredients for the craft.
        }

        // CHECK IF HAVE INGREDIENTS FOR CRAFT

        AbstractPhysicalObject.AbstractObjectType primaryGraspObjectType = self.grasps[0].grabbed.abstractPhysicalObject.type;
        AbstractPhysicalObject.AbstractObjectType secondaryGraspObjectType = self.grasps[1].grabbed.abstractPhysicalObject.type;

        var craftIngredientsToCheck = (primaryGraspObjectType, secondaryGraspObjectType);

        if (Core.Content.Crafts.TryGetValue(craftIngredientsToCheck, out var currentCraft))
        {
            if (playerCraftingData.craftTimer < currentCraft.craftTime)
                currentCraft.animation.PlaySlugcatAnimation(self, 0, 1, playerCraftingData.craftTimer);
            else // Craft is done!
            {
                // RESET TIMER FOR NEXT CRAFT.
                playerCraftingData.craftTimer = 0;

                // TODO: probably make below loop a function if it turns out to be a re-usable pattern.
                // Remove grabbed objects used for crafting
                for (int j = 0; j < self.grasps.Length; j++)
                {
                    AbstractPhysicalObject apo = self.grasps[j].grabbed.abstractPhysicalObject;
                    if (self.room.game.session is StoryGameSession)
                    {
                        (self.room.game.session as StoryGameSession).RemovePersistentTracker(apo);
                    }
                    self.ReleaseGrasp(j);
                    for (int k = apo.stuckObjects.Count - 1; k >= 0; k--)
                    {
                        if (apo.stuckObjects[k] is AbstractPhysicalObject.AbstractSpearStick &&
                            apo.stuckObjects[k].A.type == AbstractPhysicalObject.AbstractObjectType.Spear &&
                            apo.stuckObjects[k].A.realizedObject != null)
                        {
                            (apo.stuckObjects[k].A.realizedObject as Spear).ChangeMode(Weapon.Mode.Free);
                        }
                    }
                    apo.LoseAllStuckObjects();
                    apo.realizedObject.RemoveFromRoom();
                    self.room.abstractRoom.RemoveEntity(apo);
                }

                AbstractPhysicalObject craftedAbstractObject = currentCraft.craftResult(self);

                // GRAB OBJECT IF WE SCAVENGED ANYTHING

                if (craftedAbstractObject != null)
                {
                    self.room.abstractRoom.AddEntity(craftedAbstractObject);
                    craftedAbstractObject.RealizeInRoom();

                    self.SlugcatGrab(craftedAbstractObject.realizedObject, self.FreeHand());
                    playerCraftingData.craftTimer = 0; // Reset the timer
                }
            }
        }
    }
}

// Stuff later for shelter crafts.
/*
        var craftToCheck = (primaryGraspObjectType, secondaryGraspObjectType);

        Craft? currentCraft = playerCraftingData.currentCraft;

        if (currentCraft == null)
        {
            var possibleCrafts = Core.Content.GetCraftsForObjectTypes(typesToCheck);
            if (possibleCrafts.Count == 0)
                return; // No crafts found for the object types.
            else // Set the possible current craft to the first in the list.
                currentCraft = possibleCrafts.ToArray()[0];
        }

        if (Core.Content.HasAllIngredientsForCraft(typesToCheck, currentCraft.Value))
        {
            playerCraftingData.currentCraft = currentCraft;
        }
        else
        {
            playerCraftingData.currentCraft = null; // Reset the craft if we don't have all the ingredients for it anymore.
            return; // No ingredients for the craft.
        }

        // CHECK IF WANTS TO CRAFT

        if (self.IsPressed(Inputs.Input.Craft))
        {
            if (currentCraft.Value.craftTime > 0)
                currentCraft.Value.animation.PlaySlugcatAnimation(self, 0, 1, playerCraftingData.craftTimer);
            else // Craft is done!
            {
                // TODO: probably make below loop a function if it turns out to be a re-usable pattern.
                // Remove grabbed objects used for crafting
                for (int j = 0; j < self.grasps.Length; j++)
                {
                    AbstractPhysicalObject apo = self.grasps[j].grabbed.abstractPhysicalObject;
                    if (self.room.game.session is StoryGameSession)
                    {
                        (self.room.game.session as StoryGameSession).RemovePersistentTracker(apo);
                    }
                    self.ReleaseGrasp(j);
                    for (int k = apo.stuckObjects.Count - 1; k >= 0; k--)
                    {
                        if (apo.stuckObjects[k] is AbstractPhysicalObject.AbstractSpearStick &&
                            apo.stuckObjects[k].A.type == AbstractPhysicalObject.AbstractObjectType.Spear &&
                            apo.stuckObjects[k].A.realizedObject != null)
                        {
                            (apo.stuckObjects[k].A.realizedObject as Spear).ChangeMode(Weapon.Mode.Free);
                        }
                    }
                    apo.LoseAllStuckObjects();
                    apo.realizedObject.RemoveFromRoom();
                    self.room.abstractRoom.RemoveEntity(apo);
                }

                AbstractPhysicalObject craftedAbstractObject = currentCraft.Value.craftResult(self);

                // GRAB OBJECT IF WE SCAVENGED ANYTHING

                if (craftedAbstractObject != null)
                {
                    self.room.abstractRoom.AddEntity(craftedAbstractObject);
                    craftedAbstractObject.RealizeInRoom();

                    self.SlugcatGrab(craftedAbstractObject.realizedObject, self.FreeHand());
                    playerCraftingData.craftTimer = 0; // Reset the timer
                }
            }
        }
    }
*/