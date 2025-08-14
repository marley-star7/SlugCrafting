using MRCustom.Animations;
using SlugCrafting.Crafts;

namespace SlugCrafting;

public class PlayerCraftingData
{
    public Creature.Grasp containerViewGrasp;

    private Dictionary<Accessory.EquipRegion, Accessory> _equipRegionAccessories = new();
    public Dictionary<Accessory.EquipRegion, Accessory> equipRegionAccessories { get => _equipRegionAccessories; }

    public List<Accessory> accessories { get; } = new();

    public float[] accessoriesMass { get; } = new float[2] { 0, 0 };

    public ScavengeSpot? currentTargetedScavengeSpot { get; set; }

    public AbstractPhysicalObjectScavenge? currentTargetedScavenge { get; set; }

    public Craft? currentPossibleCraft { get; set; }

    private float _scavengeTimer;
    public float scavengeTimer
    {
        get => _scavengeTimer;
        set => _scavengeTimer = value;
    }

    private float _craftTimer;
    public float craftTimer
    {
        get => _craftTimer;
        set => _craftTimer = value;
    }

    public int craftAnimationIndex = 0;
    public int craftAnimationsTotalTimesLooped = 0;

    public bool isPhysicalCrafting => _craftTimer > 0;

    public bool physicalCraftingEnabled = false;

    public WeakReference<Player> playerRef { get; }

    public PlayerCraftingData(Player player)
    {
        playerRef = new WeakReference<Player>(player);
        player.GetHandAnimationPlayer().AnimationLooped += OnPlayerHandAnimationLooped;

        if (player.IsCrafter())
            physicalCraftingEnabled = true;
    }

    private void OnPlayerHandAnimationLooped(PlayerHandAnimationPlayer.AnimationIndex loopedAnimation, int timesLooped)
    {
        if (!playerRef.TryGetTarget(out Player player) || currentPossibleCraft == null)
            return;

        var craft = currentPossibleCraft.Value;
        var currentCraftAnimation = craft.animations[craftAnimationIndex];

        craftAnimationsTotalTimesLooped++;

        Plugin.LogDebug($"totalAnimationLoops: {craft.totalAnimationLoops}, loops: {timesLooped}, {isPhysicalCrafting}, {loopedAnimation == currentCraftAnimation.animation}");

        if (isPhysicalCrafting && loopedAnimation == currentCraftAnimation.animation)
        {
            if (craftAnimationsTotalTimesLooped >= craft.totalAnimationLoops)
                player.CompletePhysicalCraft(craft);
            else if (timesLooped >= currentCraftAnimation.loopsInAnimation)
                player.ContinueToNextCraftAnimation();
        }
    }
}

//
// EXTENSIONS
//

public static class PlayerCraftingExtensions
{
    // --- Crafting Data Management ---
    private static readonly ConditionalWeakTable<Player, PlayerCraftingData> _craftingDataTable = new();

    public static PlayerCraftingData GetPlayerCraftingData(this Player player) =>
        _craftingDataTable.GetValue(player, p => new PlayerCraftingData(p));

    public static bool IsCrafter(this Player player) =>
        player.slugcatStats.name == SlugCraftingEnums.Crafter;

    public static bool CanPhysicalCraft(this Player player) => player.GetPlayerCraftingData().physicalCraftingEnabled;

    // --- Grasping Utilities ---
    public static Craft? GetGraspsPhysicalCraft(this Player player)
    {
        var craftingData = player.GetPlayerCraftingData();
        var animationPlayer = player.GetHandAnimationPlayer();

        var primaryType = player.grasps[0]?.grabbed?.abstractPhysicalObject.type;
        var secondaryType = player.grasps[1]?.grabbed?.abstractPhysicalObject.type;

        if (primaryType == null || secondaryType == null)
            return null;

        if (Core.Content.Crafts.TryGetValue((primaryType, secondaryType), out var craft) &&
            craft.ingredientValidation(player.grasps[0].grabbed, player.grasps[1].grabbed))
        {
            return craft;
        }
        return null;
    }

    private static void TryEquipAccessory(this Player player)
    {
        if (player.grasps[0]?.grabbed is IEquippable accessory)
        {
            accessory.Equip(player); // TODO: Replace with proper equipping logic later
        }
    }

    private static bool TryPopItemFromContainerCyclerAndGrab(this Player player, VisibleItemContainerCycler containerCycler)
    {
        if (!containerCycler.HasItemInTargetedSlot())
            return false;

        var abstractItemToGrab = containerCycler.PopItemFromTargetedSlot();
        if (abstractItemToGrab == null)
            return false;

        if (abstractItemToGrab.realizedObject == null)
        {
            abstractItemToGrab.pos = player.coord;
            player.room.abstractRoom.AddEntity(abstractItemToGrab);
            abstractItemToGrab.pos = player.abstractCreature.pos;
            abstractItemToGrab.RealizeInRoom();
        }

        player.SlugcatGrab(abstractItemToGrab.realizedObject, player.FreeHand());

        return true;
    }

    private static void HandleBundlingLogic(Player player)
    {
        var secondaryHand = player.grasps[1];

        // Prioritize primary hand interactions
        if (player.grasps[0]?.grabbed is PlayerCarryableItem primaryItem)
        {
            // Case: Secondary hand is empty → pop from primary's bundle
            if (secondaryHand == null)
            {
                AbstractPhysicalObject? itemToGrab = null;
                if (primaryItem is IHaveVisibleItemContainerCycler primaryItemContainerCycler && player.TryPopItemFromContainerCyclerAndGrab(primaryItemContainerCycler.visibleItemContainerCycler))
                    return;

                var bundle = primaryItem.GetBundle();
                if (bundle != null)
                    itemToGrab = bundle.PopItem();

                if (itemToGrab != null)
                {
                    if (itemToGrab.realizedObject == null)
                    {
                        itemToGrab.RealizeInRoom();
                    }

                    if (itemToGrab.realizedObject != null)
                        player.SlugcatGrab(itemToGrab.realizedObject, player.FreeHand());
                }

                return;
            }

            // Case: Both hands have items
            if (secondaryHand.grabbed is PlayerCarryableItem secondaryItem)
            {
                if (secondaryItem is IHaveVisibleItemContainerCycler secondaryItemContainerCycler)
                {
                    secondaryItemContainerCycler.visibleItemContainerCycler.PutItemInTargetedSlot(primaryItem.abstractPhysicalObject);
                }
                else if (primaryItem.GetBundle() != null && primaryItem.GetBundle().TryAddItem(primaryItem.abstractPhysicalObject)) { }
                else
                {
                    player.SwitchHands(); // Fallback: Swap items
                }
            }
        }
        // Case only right hand has item
        else if (player.grasps[1]?.grabbed is PlayerCarryableItem secondaryItem)
        {
            TryPopItemFromSecondaryAndGrab(player, secondaryItem);
        }
    }

    private static void TryPopItemFromSecondaryAndGrab(Player player, PlayerCarryableItem secondaryItem)
    {
        AbstractPhysicalObject? itemToGrab = null;
        if (secondaryItem is IHaveVisibleItemContainerCycler secondaryItemContainerCycler && secondaryItemContainerCycler.itemContainer.HasItemInSlot(secondaryItemContainerCycler.visibleItemContainerCycler.currentlyTargetedSlot))
        {
            itemToGrab = secondaryItemContainerCycler.visibleItemContainerCycler.PopItemFromTargetedSlot();
        }
        else
        {
            /*
            var itemToGrab = secondaryItem.GetBundle().PopItem();
            if (itemToGrab != null)
            {
                player.SlugcatGrab(itemToGrab.realizedObject, player.FreeHand());
            }
            */
        }
    }

    // --- Bundling Logic ---
    internal static void WhileInputAlternateUsePressed(this Player player)
    {
        // Early exit if grab key not pressed
        if (!player.JustPressed(ImprovedInput.PlayerKeybind.Grab))
            return;

        // Temporary equip logic (for testing)
        TryEquipAccessory(player);

        HandleBundlingLogic(player);
    }

    internal static void OnInputAlternateUseJustReleased(this Player player)
    {

    }

    // --- Crafting Logic ---
    public static bool CanPerformCraftInCurrentAnimation(this Player self, Craft craft)
    {
        if (craft.bodyModeRequirement == Craft.BodyModeRequirements.Stand && !(self.bodyMode == Player.BodyModeIndex.Stand || self.bodyMode == Player.BodyModeIndex.Default))
            return false;

        else if (craft.bodyModeRequirement == Craft.BodyModeRequirements.Sneak && self.bodyMode != Player.BodyModeIndex.Crawl)
            return false;

        if (self.animation != Player.AnimationIndex.None &&
            self.animation != Player.AnimationIndex.StandUp &&
            self.animation != Player.AnimationIndex.StandOnBeam &&
            self.animation != Player.AnimationIndex.BeamTip &&
            self.animation != Player.AnimationIndex.DownOnFours)
            return false;

        if (craft.needBothHandsFree && self.animation == Player.AnimationIndex.ClimbOnBeam)
            return false;

        return true;
    }

    public static void ContinueToNextCraftAnimation(this Player self)
    {
        var playerSCData = self.GetPlayerCraftingData();

        playerSCData.craftAnimationIndex++;
        var nextCraftAnimation = playerSCData.currentPossibleCraft.Value.animations[playerSCData.craftAnimationIndex].animation;
        self.GetHandAnimationPlayer().Play(nextCraftAnimation);
    }

    private static void PhysicalCraftEnded(this Player self)
    {
        var playerSlugCraftingData = self.GetPlayerCraftingData();

        // RESET TIMER FOR NEXT CRAFT.
        playerSlugCraftingData.craftTimer = 0;
        playerSlugCraftingData.craftAnimationIndex = 0;
        playerSlugCraftingData.craftAnimationsTotalTimesLooped = 0;
    }

    public static void CheckGraspsForPossiblePhysicalCraft(this Player player)
    {
        player.GetPlayerCraftingData().currentPossibleCraft = player.GetGraspsPhysicalCraft();
    }

    internal static void CraftInputStart(this Player player)
    {
        player.CheckGraspsForPossiblePhysicalCraft();

        var sCData = player.GetPlayerCraftingData();
        if (sCData.currentPossibleCraft == null)
            return;

        var animationPlayer = player.GetHandAnimationPlayer();
        var craft = sCData.currentPossibleCraft.Value;
        var currentAnim = craft.animations[sCData.craftAnimationIndex].animation;

        animationPlayer.Play(currentAnim);
    }

    internal static void CraftInputUpdate(this Player player)
    {
        var sCData = player.GetPlayerCraftingData();
        var animationPlayer = player.GetHandAnimationPlayer();

        if (sCData.currentPossibleCraft == null)
            return;

        var craft = sCData.currentPossibleCraft.Value;

        if (player.CanPerformCraftInCurrentAnimation(craft))
        {
            player.swallowAndRegurgitateCounter = 0;
            sCData.craftTimer++;
        }
    }

    internal static void CraftInputRelease(this Player self)
    {
        var selfCData = self.GetPlayerCraftingData();
        if (selfCData.currentPossibleCraft == null) 
                return;

        var animationPlayer = self.GetHandAnimationPlayer();
        var craft = selfCData.currentPossibleCraft.Value;
        var wouldBeAnim = craft.animations[selfCData.craftAnimationIndex].animation;

        animationPlayer.Stop(wouldBeAnim);
        self.PhysicalCraftEnded();
    }

    public static void CompletePhysicalCraft(this Player self, Craft craft)
    {
        self.GetHandAnimationPlayer().Stop();

        var primaryCraftObject = self.grasps[0].grabbed;
        var secondaryCraftObject = self.grasps[1].grabbed;
        craft.craftResult(self, primaryCraftObject, secondaryCraftObject);

        // UPDATE THE CURRENT POSSIBLE CRAFTS
        GetGraspsPhysicalCraft(self);
        self.PhysicalCraftEnded();
    }

    public static void CancelPhysicalCraft(this Player self)
    {
        var playerSlugCraftingData = self.GetPlayerCraftingData();

        if (playerSlugCraftingData.currentPossibleCraft != null)
        {
            playerSlugCraftingData.craftTimer = -1;

            var playerHandAnimationPlayer = self.GetHandAnimationPlayer();
            playerHandAnimationPlayer.Stop(playerSlugCraftingData.currentPossibleCraft.Value.animations[playerSlugCraftingData.craftAnimationIndex].animation);
        }
        self.PhysicalCraftEnded();
    }

    // TODO: can probably just make scavenges crafts internally, i mean they function the same way lol, and this way you can craft with a creature.
    // --- Scavenging ---
    public static void CheckGraspsForPossibleScavenge(this Player self)
    {
        var selfSCData = self.GetPlayerCraftingData();

        // MS7: Check primary hand first.
        for (int graspNum = 0; graspNum <= 1; graspNum++)
        {
            var grasp = self.grasps[graspNum];
            if (grasp == null || grasp.grabbedChunk == null)
                continue;

            if (grasp.grabbedChunk.owner is Creature)
            {
                // Get the first available scavenge spot from the grabbed chunk as the currently targeted scavenge.
                var creatureScavengeData = ((Creature)grasp.grabbed).GetScavengeData();
                if (creatureScavengeData == null)
                    continue;

                var scavengeSpot = new ScavengeSpot(grasp.grabbedChunk.index, 0, 0);
                var scavenge = creatureScavengeData.GetScavenge(scavengeSpot);

                // If the grabbed scavenging spot or already scavenged, then search for one that isn't.
                if (scavengeSpot == null || scavenge.canScavenge == false)
                    creatureScavengeData.GetNearestValidScavenge(scavengeSpot);

                // DISABLED
                // If we found a new valid scavenge spot diff from orig, grab that one's chunk instead.
                // self.grasps[graspedPhysicalObjectGraspIndex].chunkGrabbed = scavengeSpot.bodyChunkIndex;
                //

                selfSCData.currentTargetedScavenge = scavenge;
            }
        }
    }

    /// <summary>
    /// Returns the first grasp index holding a knife, prioritizes primary hand first.
    /// Returns -1 if no grasp was found.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static int GetKnifeGraspIndex(this Player player)
    {
        for (int graspNum = 0; graspNum <= 1; graspNum++)
        {
            if (player.grasps[graspNum] == null
                || player.grasps[graspNum].grabbed is not Knife graspedKnife)
                continue;

            return graspNum;
        }

        return -1;
    }

    /// <summary>
    /// Returns the first grasp holding a knife, prioritizes primary hand first.
    /// Returns null if no grasp was found.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static Creature.Grasp GetKnifeGrasp(this Player player)
    {
        int knifeGrasp = GetKnifeGraspIndex(player);
        if (knifeGrasp == -1)
            return null;
        else
            return player.grasps[knifeGrasp];
    }

    public static bool CanPerformScavengeInCurrentAnimation(this Player player, AbstractPhysicalObjectScavenge scavenge)
    {
        return true;
    }

    internal static void ScavengeInputStart(this Player player)
    {
        var playerSCData = player.GetPlayerCraftingData();

        var currentScavenge = playerSCData.currentTargetedScavenge;
        var knifeGraspIndex = player.GetKnifeGraspIndex();

        if (currentScavenge == null
            || currentScavenge.canScavenge == false
            || (currentScavenge.requiresKnife && knifeGraspIndex != -1)
            )
            return;

        var creatureGraspIndex = MarPlayerExtensions.GetOtherGrasp(knifeGraspIndex);
        var scavengeCurrentAnimationIndex = currentScavenge.GetAnimationForCreatureInGraspIndex(creatureGraspIndex);

        player.GetHandAnimationPlayer().Play(scavengeCurrentAnimationIndex);
    }

    internal static void ScavengeInputUpdate(this Player self)
    {
        var playerSCData = self.GetPlayerCraftingData();
        var currentScavenge = playerSCData.currentTargetedScavenge;

        if (currentScavenge == null)
            return;

        playerSCData.scavengeTimer++;

        if (currentScavenge.scavengeTime <= 0)
            CompleteScavenge(self, currentScavenge);
    }

    internal static void ScavengeInputRelease(this Player self)
    {
        var playerSCData = self.GetPlayerCraftingData();

        if (playerSCData.currentTargetedScavenge == null)
            return;

        playerSCData.scavengeTimer = 0; // Reset the timer to craft if not pressing craft.

        var creatureGraspIndex = MarPlayerExtensions.GetOtherGrasp(self.GetKnifeGraspIndex());
        var scavengeCurrentAnimationIndex = playerSCData.currentTargetedScavenge.GetAnimationForCreatureInGraspIndex(creatureGraspIndex);
        self.GetHandAnimationPlayer().Stop(scavengeCurrentAnimationIndex);
    }

    public static void CompleteScavenge(this Player player, AbstractPhysicalObjectScavenge scavenge)
    {
        var playerSlugCraftingData = player.GetPlayerCraftingData();

        AbstractPhysicalObject scavengedAbstractObject = scavenge.Scavenge();

        // Grab object if we scavenged anything.
        if (scavengedAbstractObject != null)
        {
            player.room.abstractRoom.AddEntity(scavengedAbstractObject);
            scavengedAbstractObject.RealizeInRoom();

            player.ReleaseGrasp(MarPlayerExtensions.GetOtherGrasp(player.GetKnifeGraspIndex()));
            player.SlugcatGrab(scavengedAbstractObject.realizedObject, player.FreeHand());
            playerSlugCraftingData.scavengeTimer = 0; // Reset the timer
        }
    }

    // --- Accessory Management ---
    public static void EquipAccessory(this Player player, Accessory accessory) => 
        player.GetPlayerCraftingData().accessories.Add(accessory);

    public static void UnequipAccessory(this Player player, Accessory accessory) => 
        player.GetPlayerCraftingData().accessories.Remove(accessory);

    public static void UpdateMass(this Player player) => 
        player.SetMalnourished(player.Malnourished);
}