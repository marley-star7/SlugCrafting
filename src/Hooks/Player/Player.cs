using UnityEngine;
using RWCustom;

using SlugCrafting.Items;
using SlugCrafting.Scavenges;
using MarMath;
using ImprovedInput;

namespace SlugCrafting;

public static partial class Hooks
{
    // TODO: add different scavenging times saved to the items scavenge data type thingy when you add it.
    private static bool CanMaul(Player scug)
    {
        PlayerCraftingData playerCraftingData = scug.GetCraftingData();

        if (playerCraftingData.scavengeTimer == 0)
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

    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        var playerCraftingData = self.GetCraftingData();

        ScavengingUpdate(ref self, ref playerCraftingData);

        orig(self, eu);
    }
    private static void Player_GrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu)
    {
        // Call the original method
        orig(self, eu);
    }

    public static void CraftingUpdate(ref Player self, ref PlayerCraftingData playerCraftingData)
    {

    }

    public static void ScavengingUpdate(ref Player self, ref PlayerCraftingData playerCraftingData)
    {

        //
        // CHECK WHAT HOLDING FOR SCAVENGE OR CRAFT
        //

        Knife? graspedKnife = null;
        int graspedKnifeGraspIndex = -1;
        PhysicalObject? graspedPhysicalObject = null;
        int graspedPhysicalObjectGraspIndex = -1;

        // Check if the player is holding a graspedKnife and a creature simultaneously
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
                graspedPhysicalObject = grasp.grabbed as PhysicalObject;
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
                graspedPhysicalObject = grasp.grabbed as PhysicalObject;
            }
        }

        //
        // IF CAN SCAVENGE
        //

        if (self.IsPressed(Inputs.Input.Scavenge) && graspedKnife != null && graspedPhysicalObject != null)
        {
            graspedKnife.BladeColor = Color.green; // TEMP

            playerCraftingData.scavengeTimer++;

            // Cooldown before scavenge starts.
            if (playerCraftingData.scavengeTimer < 10)
                return;

            var scavenge = playerCraftingData.scavenge;

            // If player is not currently scavenging a spot, try to find one to scavenge.
            if (scavenge == null)
            {
                CreatureScavengeData scavengeData = null;
                // SCAVENGING CURRENTLY ONLY WORKS FOR CREATURES
                if (graspedPhysicalObject is Creature)
                    scavengeData = (graspedPhysicalObject as Creature).GetCreatureScavengeData();
                // If no scavenge data, return.
                if (scavengeData == null)
                    return;

                var scavengeSpot = new ScavengeSpot(self.grasps[graspedPhysicalObjectGraspIndex].grabbedChunk.index, 0, 0);
                scavenge = scavengeData.GetScavenge(scavengeSpot);
                // If the grabbed scavenging spot or already scavenged, then search for one that isn't.
                if (scavengeSpot == null || scavenge.canScavenge == false)
                    scavengeData.GetNearestValidScavenge(scavengeSpot);
                // If STILL null, there are no valid scavenge spots, return.
                if (scavengeSpot == null)
                    return;
                else // If we found a new valid scavenge spot diff from orig, grab that one's chunk instead.
                    self.grasps[graspedPhysicalObjectGraspIndex].chunkGrabbed = scavengeSpot.bodyChunkIndex;

                // Save the scavenge data so we don't have to waste time searching for it again.
                playerCraftingData.scavengeSpot = scavengeSpot;
                playerCraftingData.scavenge = scavenge;
            }

            // DO SCAVENGE ANIMATION
            if (scavenge.canScavenge == true)
            {
                scavenge.scavengeTime--;

                if (scavenge.scavengeTime > 0)
                    scavenge.animation.PlaySlugcatAnimation(self, graspedKnifeGraspIndex, graspedPhysicalObjectGraspIndex, playerCraftingData.scavengeTimer);
                else // Scavenge is done!
                {
                    AbstractPhysicalObject scavengedAbstractObject = scavenge.Scavenge();

                    var tilePosition = self.room.GetTilePosition(graspedPhysicalObject.bodyChunks[0].pos);
                    var pos = new WorldCoordinate(self.room.abstractRoom.index, tilePosition.x, tilePosition.y, 0);

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
                playerCraftingData.scavengeSpot = null; // Reset the scavenge spot if we can't scavenge anymore.
                playerCraftingData.scavenge = null; // Reset the scavenge data if we can't scavenge anymore.
                playerCraftingData.scavengeTimer = 0; // Reset the timer to scavenge if not pressing scavenge.
            }
        }
    }
}