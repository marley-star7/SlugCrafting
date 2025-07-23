using SlugCrafting.Items.Accessories;
using System;
using static SlugCrafting.Accessories.Accessory;

namespace SlugCrafting;

public class PlayerCraftingData
{
    public List<Accessory> accessories = new();
    public Dictionary<Accessory.EquipRegion, Accessory> equipRegionAccessories = new();

    /// <summary>
    /// The mass applied from accessories for each body chunk.
    /// </summary>
    public float[] accessoriesMass = new float[]
    {
        0,
        0,
    };

    public float scavengeTimer = 0;
    public float craftTimer = 0;

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
                player.CompletePhysicalCraft(currentPossibleCraft.Value);
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

    public static PlayerCraftingData GetPlayerCraftingData(this Player player) => craftingDataConditionalWeakTable.GetValue(player, _ => new PlayerCraftingData(player));

    public static bool IsCrafter(this Player player)
    {
        return player.slugcatStats.name == SlugCraftingEnums.Crafter;
    }

    /// <summary>
    /// Checks if the player can do craft via SlugCrafting Mod.
    /// Labeled as "PhysicalCraft" to differentiate from gourmond crafting.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool CanPhysicalCraft(this Player player)
    {
        // TODO: later set this up so that other scugs can craft if this is enabled.
        return player.slugcatStats.name == SlugCraftingEnums.Crafter;
    }

    //
    // GRASPING AND LETTING GO
    //

    internal static void OnPlayerSwitchGrasp(Player player, int graspFrom, int graspTo)
    {
        // Update the possible craft if the player switched grasps, since crafts are based off primary.
        player.GetPlayerCraftingData().currentPossibleCraft = GetGraspsPhysicalCraft(player);
    }

    public static int GetOtherPlayerGrasp(int grasp)
    {
        return grasp == 0 ? 1 : 0;
    }

    internal static void OnPlayerGrab(Player player, PhysicalObject grabbedObj, int graspUsed, int chunkGrabbed, Creature.Grasp.Shareability shareability, float dominance, bool overrideEquallyDominant, bool pacifying)
    {
        var PlayerSlugCraftingData = player.GetPlayerCraftingData();

        CheckGraspsForScavengeKnifeOrCreature(graspUsed);

        // If the other hand is not empty, check for possible craft.
        if (player.grasps[GetOtherPlayerGrasp(graspUsed)] != null)
            PlayerSlugCraftingData.currentPossibleCraft = GetGraspsPhysicalCraft(player);

        //
        // SCAVENGING ITEMS CHECK
        //

        void CheckGraspsForScavengeKnifeOrCreature(in int graspNum)
        {
            var grasp = player.grasps[graspNum];

            // CREATURE CHECKING
            // Grabbed chunk takes priority first, because can be shared with item and creature.
            if (grasp.grabbedChunk.owner is Creature)
            {
                PlayerSlugCraftingData.creatureGraspUsed = graspNum;

                // Get the first available scavenge spot from the grabbed chunk as the currently targeted scavenge.
                var creatureScavengeData = ((Creature)player.grasps[PlayerSlugCraftingData.creatureGraspUsed].grabbed).GetScavengeData();
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

                    PlayerSlugCraftingData.currentTargetedScavenge = scavenge;
                }
            }

            // ITEM CHECKING
            // Grabbed knife overrides grabbed chunk if detected then.
            if (grasp.grabbed != null && grasp.grabbed is Knife)
                PlayerSlugCraftingData.knifeGraspUsed = graspNum;
        }
    }

    public static Craft? GetGraspsPhysicalCraft(Player player)
    {
        var PlayerSlugCraftingData = player.GetPlayerCraftingData();
        var playerHandAnimationPlayer = player.GetHandAnimationPlayer();

        AbstractPhysicalObject.AbstractObjectType? primaryGraspObjectType = null;
        AbstractPhysicalObject.AbstractObjectType? secondaryGraspObjectType = null;

        // Check each individual hand, dominant and non-dominant hand are treated seperately.
        // If the grasp is not null, also check for scavenge knife or creature.

        if (player.grasps[0] == null || player.grasps[0].grabbed == null)
            return null;
        else
            primaryGraspObjectType = player.grasps[0].grabbed.abstractPhysicalObject.type;

        if (player.grasps[1] == null || player.grasps[1].grabbed == null)
            return null;
        else
            secondaryGraspObjectType = player.grasps[1].grabbed.abstractPhysicalObject.type;

        var craftIngredientsToCheck = (primaryGraspObjectType, secondaryGraspObjectType);

        //Plugin.Logger.LogMessage("ObjectTypes of held objects are: " + player.grasps[0].grabbed.abstractPhysicalObject.type.ToString() + " " + player.grasps[1].grabbed.abstractPhysicalObject.type.ToString());
        if (Core.Content.Crafts.TryGetValue(craftIngredientsToCheck, out var craft))
        {
            if (craft.ingredientValidation(player.grasps[0].grabbed, player.grasps[1].grabbed))
                return craft;
        }
        return null;
    }

    internal static void OnPlayerReleaseGrasp(Player player, int grasp)
    {
        var PlayerSlugCraftingData = player.GetPlayerCraftingData();

        if (grasp <= 1) // Only check for the first two grasps for a release, if so there is obviously no possible craft currently.
            PlayerSlugCraftingData.currentPossibleCraft = null;

        // Reset the scavenge data if we released the grasp.
        if (grasp == PlayerSlugCraftingData.creatureGraspUsed)
        {
            PlayerSlugCraftingData.creatureGraspUsed = -1;
            PlayerSlugCraftingData.currentTargetedScavenge = null;
        }
        else if (grasp == PlayerSlugCraftingData.knifeGraspUsed)
        {
            PlayerSlugCraftingData.knifeGraspUsed = -1;
        }

        // Update the current possible crafts
        PlayerSlugCraftingData.currentPossibleCraft = GetGraspsPhysicalCraft(player);
    }

    //
    // BUNDLING / DEBUNDLING WITH GRAB
    //

    public static void BundleGrabUpdate(this Player selfPlayer, bool eu)
    {
        if (!selfPlayer.JustPressed(ImprovedInput.PlayerKeybind.Grab))
            return;

        //-- MR7: TODO: TEMP FUNCTIONALITY FOR TESTING, LATER HAVE DIFFERENT WAY OF EQUIPPING.
        if (selfPlayer.grasps[0] != null && selfPlayer.grasps[0].grabbed is AccessoryItem)
        {
            selfPlayer.EquipAccessory((selfPlayer.grasps[0].grabbed as AccessoryItem).accessory);
        }

        //-- MR7: Prioritize items in the primary hand first for checks, following the standard of primary item's being the focus for interaction.
        // As alternate into primary gives for the least amount of button presses only when need an item on demand to be used immediately from a bundle,
        // (which can be mitigated via just having an empty hand mean that the bundle is always pulled from into it.)
        //-- Otherwise in cases like a backpack, where you are interacting with that first item, and pulling out of it, it being in the primary, pulling into alternate,
        // since it has other interaction is best. Which you then will likely throw the backpack on your back again or on the ground if is an emergency.

        if (selfPlayer.grasps[0] != null && selfPlayer.grasps[0].grabbed is PlayerCarryableItem)
        {
            var primaryHandItem = selfPlayer.grasps[0].grabbed as PlayerCarryableItem;
            //-- MR7: First look to add Item to second hand's bundle if second hand has item and is bundleable.
            // If second hand empty, pop an item from the primary hand's bundle to put in alternate.

            if (selfPlayer.grasps[1] != null && selfPlayer.grasps[1].grabbed is PlayerCarryableItem) // Both Hands Have Items
            {
                var secondaryHandItem = selfPlayer.grasps[1].grabbed as PlayerCarryableItem;

                if (primaryHandItem.CanBundleWith(secondaryHandItem))
                    secondaryHandItem.AddItemToBundle(primaryHandItem);
                else //-- MR7: Just switch grasps instead, for quality of life.
                    selfPlayer.SwitchHands();
            }
            else
            {
                if (primaryHandItem.GetBundle() != null)
                    selfPlayer.SlugcatGrab(primaryHandItem.PopItemFromBundle(), selfPlayer.FreeHand());
            }
        }
        //-- MR7: Only if primary hand is empty, do we fall back on checking the secondary hand for a bundle to pull from.
        else if (selfPlayer.grasps[1] != null && selfPlayer.grasps[1].grabbed is PlayerCarryableItem)
        {
            var secondaryHandItem = selfPlayer.grasps[1].grabbed as PlayerCarryableItem;

            selfPlayer.SlugcatGrab(secondaryHandItem.PopItemFromBundle(), selfPlayer.FreeHand());
        }
    }

    //
    // SCAVENGING UPDATE
    //

    internal static void ScavengeUpdate(Player self)
    {
        // CHECK IF WANTS TO SCAVENGE

        var playerSlugCraftingData = self.GetPlayerCraftingData();
        var playerHandAnimationPlayer = self.GetHandAnimationPlayer();

        if (self.IsPressed(Inputs.Scavenge)  && playerSlugCraftingData.creatureGraspUsed != -1)
        {
            var currentScavenge = playerSlugCraftingData.currentTargetedScavenge;

            if (currentScavenge != null 
                && currentScavenge.canScavenge == true 
                && (!currentScavenge.requiresKnife || playerSlugCraftingData.knifeGraspUsed != -1)
                )
            {
                playerSlugCraftingData.scavengeTimer++;

                // Cooldown before scavenge starts.
                if (playerSlugCraftingData.scavengeTimer < 10)
                    return;

                playerHandAnimationPlayer.currentAnimationIndex = currentScavenge.handAnimation;
                currentScavenge.scavengeTime--;

                if (currentScavenge.scavengeTime <= 0)
                    CompleteScavenge(self, currentScavenge);
            }
        }
        else
        {
            playerHandAnimationPlayer.currentAnimationIndex = PlayerHandAnimationPlayer.HandAnimationIndex.None;
            playerSlugCraftingData.scavengeTimer = 0; // Reset the timer to craft if not pressing craft.
        }
    }

    public static void CompleteScavenge(this Player self, AbstractPhysicalObjectScavenge scavenge)
    {
        var playerSlugCraftingData = self.GetPlayerCraftingData();

        AbstractPhysicalObject scavengedAbstractObject = scavenge.Scavenge();

        // GRAB OBJECT IF WE SCAVENGED ANYTHING
        if (scavengedAbstractObject != null)
        {
            self.room.abstractRoom.AddEntity(scavengedAbstractObject);
            scavengedAbstractObject.RealizeInRoom();

            self.ReleaseGrasp(playerSlugCraftingData.creatureGraspUsed);
            self.SlugcatGrab(scavengedAbstractObject.realizedObject, self.FreeHand());
            playerSlugCraftingData.scavengeTimer = 0; // Reset the timer
        }
    }

    //
    // CRAFTING STUFF
    //

    // TODO: maybe you craft slower while moving, or cant craft while moving?
    internal static void PhysicalCraftUpdate(Player self)
    {
        var PlayerSlugCraftingData = self.GetPlayerCraftingData();
        var playerHandAnimationPlayer = self.GetHandAnimationPlayer();

        // This is sum shit, but idrc rn it works :"]!!!!!

        // Requirements for Crafting
        if (PlayerSlugCraftingData.currentPossibleCraft != null)
        {
            if (self.IsPressed(Inputs.Craft))
            {
                // Make sure we aren't swallowing while crafting.
                self.swallowAndRegurgitateCounter = 0;

                PlayerSlugCraftingData.craftTimer++; // Reset the timer to craft if not pressing craft.
                playerHandAnimationPlayer.currentAnimationIndex = PlayerSlugCraftingData.currentPossibleCraft.Value.handAnimationIndex;
                // Do the animations update if we have one.
                if (PlayerSlugCraftingData.currentPossibleCraft.Value.handAnimation != null)
                {
                    playerHandAnimationPlayer.Play(PlayerSlugCraftingData.currentPossibleCraft.Value.handAnimation);
                }

                // If the craft timer has reached high enough, complete the craft.
                if (PlayerSlugCraftingData.craftTimer >= PlayerSlugCraftingData.currentPossibleCraft.Value.craftTime)
                    CompletePhysicalCraft(self, PlayerSlugCraftingData.currentPossibleCraft.Value);
            }
            else
            {
                if (PlayerSlugCraftingData.currentPossibleCraft.Value.handAnimation != null)
                {
                    playerHandAnimationPlayer.Stop(PlayerSlugCraftingData.currentPossibleCraft.Value.handAnimation);
                }
                playerHandAnimationPlayer.currentAnimationIndex = PlayerHandAnimationPlayer.HandAnimationIndex.None;
                PlayerSlugCraftingData.craftTimer = 0; // Reset the timer to craft if not pressing craft.
            }
        }
    }

    public static void CompletePhysicalCraft(this Player self, Craft craft)
    {
        var PlayerSlugCraftingData = self.GetPlayerCraftingData();

        // RESET TIMER FOR NEXT CRAFT.
        PlayerSlugCraftingData.craftTimer = 0;

        var primaryCraftObject = self.grasps[0].grabbed;
        var secondaryCraftObject = self.grasps[1].grabbed;
        craft.craftResult(self, primaryCraftObject, secondaryCraftObject);

        // UPDATE THE CURRENT POSSIBLE CRAFTS
        GetGraspsPhysicalCraft(self);
    }

    public static void RealizeAndGrab(this Player self, AbstractPhysicalObject abstractPhysicalObject)
    {
        if (abstractPhysicalObject.realizedObject == null)
            abstractPhysicalObject.RealizeInRoom();

        self.SlugcatGrab(abstractPhysicalObject.realizedObject, self.FreeHand());
    }

    //-- TODO: maybe move this to MRCustom.
    /// <summary>
    /// Copied from source code for how the player ends a roll.
    /// </summary>
    /// <param name="self"></param>
    public static void EndRoll(this Player self)
    {
        self.rollCounter = 0;
        self.rollDirection = 0;
        self.room.PlaySound(SoundID.Slugcat_Roll_Finish, self.mainBodyChunk, loop: false, 1f, 1f);
        self.animation = Player.AnimationIndex.None;
        self.standing = self.input[0].y > -1;
    }

    //
    // ACCESSORIES
    //

    public static void EquipAccessory(this Player selfPlayer, Accessory accessory)
    {
        accessory.Equip(selfPlayer);
    }

    public static void UpdateMass(this Player self)
    {
        //-- MR7: We basically just update malnourished again to what it is, for compatability reasons
        // as this is what changes is almost always what is interfering with the player's mass.
        // SetMalnourished is already hooked to run ApplyAccessoryMass, so this is just a more fitting function name.
        // it effectively acts as a reset to base, before we re-add player accessories, instead of having to minus and divide values all over again.
        self.SetMalnourished(self.Malnourished);
    }
}