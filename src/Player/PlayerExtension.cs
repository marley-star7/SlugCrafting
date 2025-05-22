using RWCustom;
using UnityEngine;

using System.Runtime.CompilerServices;

using SlugCrafting.Scavenges;
using SlugCrafting.Crafts;
using SlugCrafting.Items;

using MRCustom.Animations;
using ImprovedInput;

namespace SlugCrafting;

public class PlayerCraftingData
{
    public int scavengeTimer = 0;
    public int craftTimer = 0;

    public bool canCraft = false;
    public bool canScavenge = false;

    public bool isCrafting;
    public bool isScavenging;

    public int knifeGraspUsed = -1;
    public int creatureGraspUsed = -1;

    public ScavengeSpot? currentTargetedScavengeSpot = null;
    public AbstractPhysicalObjectScavenge? currentTargetedScavenge = null;
    public Craft? currentPossibleCraft = null;

    public WeakReference<Player> playerRef;

    public PlayerCraftingData(Player player)
    {
        playerRef = new WeakReference<Player>(player);
    }

    /*
    public void OnPlayerHandAnimationFinished(string finishedAnimation)
    {
        if (currentPossibleCraft != null && finishedAnimation == currentPossibleCraft.Value.animation)
        {
            if (playerRef.TryGetTarget(out var player))
                player.CompleteCraft(currentPossibleCraft.Value);
        }

        if (currentTargetedScavenge != null && finishedAnimation == currentTargetedScavenge.animation)
        {
            if (playerRef.TryGetTarget(out var player))
                player.CompleteScavenge(currentTargetedScavenge);
        }
    }
    */
}

public static class PlayerExtension
{
    // CRAFTING
    private static readonly ConditionalWeakTable<Player, PlayerCraftingData> craftingDataConditionalWeakTable = new();

    public static PlayerCraftingData GetCraftingData(this Player player) => craftingDataConditionalWeakTable.GetValue(player, _ => new PlayerCraftingData(player));

    //
    // GRASPING AND LETTING GO
    //

    internal static void OnPlayerSwitchGrasp(Player player, int graspFrom, int graspTo)
    {
        // Update the possible craft if the player switched grasps, since crafts are based off primary.
        player.GetCraftingData().currentPossibleCraft = CheckGraspsForPossibleCraft(player);
    }

    public static int GetOtherPlayerGrasp(int grasp)
    {
        return grasp == 0 ? 1 : 0;
    }

    internal static void OnPlayerGrab(Player player, PhysicalObject grabbedObj, int graspUsed, int chunkGrabbed, Creature.Grasp.Shareability shareability, float dominance, bool overrideEquallyDominant, bool pacifying)
    {
        var playerCraftingData = player.GetCraftingData();

        CheckForScavengeKnifeOrCreature(graspUsed);

        // If the other hand is not empty, check for possible craft.
        if (player.grasps[GetOtherPlayerGrasp(graspUsed)] != null)
            playerCraftingData.currentPossibleCraft = CheckGraspsForPossibleCraft(player);

        //
        // SCAVENGING ITEMS CHECK
        //

        void CheckForScavengeKnifeOrCreature(in int graspNum)
        {
            var grasp = player.grasps[graspNum];

            // CREATURE CHECKING
            // Grabbed chunk takes priority first, because can be shared with item and creature.
            if (grasp.grabbedChunk.owner is Creature)
            {
                playerCraftingData.creatureGraspUsed = graspNum;

                // Get the first available scavenge spot from the grabbed chunk as the currently targeted scavenge.
                var creatureScavengeData = ((Creature)player.grasps[playerCraftingData.creatureGraspUsed].grabbed).GetCreatureScavengeData();
                if (creatureScavengeData != null)
                {
                    var scavengeSpot = new ScavengeSpot(player.grasps[graspNum].grabbedChunk.index, 0, 0);
                    var scavenge = creatureScavengeData.GetScavenge(scavengeSpot);

                    // If the grabbed scavenging spot or already scavenged, then search for one that isn't.
                    if (scavengeSpot == null || scavenge.canScavenge == false)
                        creatureScavengeData.GetNearestValidScavenge(scavengeSpot);

                    // DISABLED
                    // If we found a new valid scavenge spot diff from orig, grab that one's chunk instead.
                    // self.grasps[graspedPhysicalObjectGraspIndex].chunkGrabbed = scavengeSpot.bodyChunkIndex;
                    //

                    playerCraftingData.currentTargetedScavenge = scavenge;
                }
            }

            // ITEM CHECKING
            // Grabbed knife overrides grabbed chunk if detected then.
            if (grasp.grabbed is Knife)
                playerCraftingData.knifeGraspUsed = graspNum;
        }
    }

    public static Craft? CheckGraspsForPossibleCraft(Player player)
    {
        var playerCraftingData = player.GetCraftingData();

        AbstractPhysicalObject.AbstractObjectType primaryGraspObjectType = null;
        AbstractPhysicalObject.AbstractObjectType secondaryGraspObjectType = null;
        bool bothHandsGrasped = true;

        // Check each individual hand, dominant and non-dominant hand are treated seperately.
        // If the grasp is not null, also check for scavenge knife or creature.

        if (player.grasps[0] == null)
            bothHandsGrasped = false;
        else
            primaryGraspObjectType = player.grasps[0].grabbed.abstractPhysicalObject.type;

        if (player.grasps[1] == null)
            bothHandsGrasped = false;
        else
            secondaryGraspObjectType = player.grasps[1].grabbed.abstractPhysicalObject.type;

        var craftIngredientsToCheck = (primaryGraspObjectType, secondaryGraspObjectType);

        Plugin.Logger.LogMessage("ObjectTypes of held objects are: " + player.grasps[0].grabbed.abstractPhysicalObject.type.ToString() + " " + player.grasps[1].grabbed.abstractPhysicalObject.type.ToString());
        if (Core.Content.Crafts.TryGetValue(craftIngredientsToCheck, out var craft))
        {
            if (craft.primaryIngredient.validation(player.grasps[0].grabbed) && craft.secondaryIngredient.validation(player.grasps[1].grabbed))
                return craft;
        }
        return null;
    }

    internal static void OnPlayerReleaseGrasp(Player player, int grasp)
    {
        var playerCraftingData = player.GetCraftingData();

        if (grasp <= 1) // Only check for the first two grasps for a release, if so there is obviously no possible craft currently.
            playerCraftingData.currentPossibleCraft = null;

        // Reset the scavenge data if we released the grasp.
        if (grasp == playerCraftingData.creatureGraspUsed)
        {
            playerCraftingData.creatureGraspUsed = -1;
            playerCraftingData.currentTargetedScavenge = null;
        }
        else if (grasp == playerCraftingData.knifeGraspUsed)
        {
            playerCraftingData.knifeGraspUsed = -1;
        }
    }

    //
    // SCAVENGING UPDATE
    //

    internal static void ScavengeUpdate(Player self)
    {
        // CHECK IF WANTS TO SCAVENGE

        var playerCraftingData = self.GetCraftingData();
        var playerHandAnimationData = self.GetHandAnimationData();

        if (self.IsPressed(Inputs.Input.Scavenge) && playerCraftingData.knifeGraspUsed != -1 && playerCraftingData.creatureGraspUsed != -1)
        {
            playerCraftingData.scavengeTimer++;

            // Cooldown before scavenge starts.
            if (playerCraftingData.scavengeTimer < 10)
                return;

            var currentScavenge = playerCraftingData.currentTargetedScavenge;

            if (currentScavenge != null && currentScavenge.canScavenge == true)
            {
                playerHandAnimationData.handAnimation = currentScavenge.handAnimation;
                currentScavenge.scavengeTime--;

                if (currentScavenge.scavengeTime <= 0)
                    CompleteScavenge(self, currentScavenge);
            }
        }
        else
        {
            playerHandAnimationData.handAnimation = PlayerHandAnimationData.HandAnimationIndex.None;
            playerCraftingData.scavengeTimer = 0; // Reset the timer to craft if not pressing craft.
        }
    }

    public static void CompleteScavenge(this Player self, AbstractPhysicalObjectScavenge scavenge)
    {
        var playerCraftingData = self.GetCraftingData();

        AbstractPhysicalObject scavengedAbstractObject = scavenge.Scavenge();

        // GRAB OBJECT IF WE SCAVENGED ANYTHING
        if (scavengedAbstractObject != null)
        {
            self.room.abstractRoom.AddEntity(scavengedAbstractObject);
            scavengedAbstractObject.RealizeInRoom();

            self.ReleaseGrasp(playerCraftingData.creatureGraspUsed);
            self.SlugcatGrab(scavengedAbstractObject.realizedObject, self.FreeHand());
            playerCraftingData.scavengeTimer = 0; // Reset the timer
        }
    }

    //
    // CRAFTING STUFF
    //

    // TODO: maybe you craft slower while moving, or cant craft while moving?
    internal static void CraftUpdate(Player self)
    {
        var playerCraftingData = self.GetCraftingData();
        var playerHandAnimationData = self.GetHandAnimationData();

        // Requirements for Crafting
        if (self.IsPressed(Inputs.Input.Craft) && playerCraftingData.currentPossibleCraft != null)
        {
            // Make sure we aren't swallowing while crafting.
            (self.graphicsModule as PlayerGraphics).swallowing = 0;

            playerCraftingData.craftTimer++; // Reset the timer to craft if not pressing craft.
            playerHandAnimationData.handAnimation = playerCraftingData.currentPossibleCraft.Value.handAnimationIndex;
            // Do the animations update if we have one.
            if (playerCraftingData.currentPossibleCraft.Value.handAnimation != null)
            {
                playerCraftingData.currentPossibleCraft.Value.handAnimation.Update(self);
            }

            // If the craft timer has reached high enough, complete the craft.
            if (playerCraftingData.craftTimer >= playerCraftingData.currentPossibleCraft.Value.craftTime)
                CompleteCraft(self, playerCraftingData.currentPossibleCraft.Value);
        }
        else
        {
            playerHandAnimationData.handAnimation = PlayerHandAnimationData.HandAnimationIndex.None;
            playerCraftingData.craftTimer = 0; // Reset the timer to craft if not pressing craft.
        }
    }

    public static void CompleteCraft(this Player self, Craft craft)
    {
        var playerCraftingData = self.GetCraftingData();

        AbstractPhysicalObject craftedAbstractObject = craft.craftResult(self, self.grasps[0].grabbed, self.grasps[1].grabbed);

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

        // GRAB OBJECT IF WE CRAFTED ANYTHING

        if (craftedAbstractObject != null)
        {
            self.room.abstractRoom.AddEntity(craftedAbstractObject);
            craftedAbstractObject.RealizeInRoom();

            self.SlugcatGrab(craftedAbstractObject.realizedObject, self.FreeHand());
            playerCraftingData.craftTimer = 0; // Reset the timer
        }
    }
}