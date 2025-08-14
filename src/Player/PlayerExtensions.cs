using MRCustom.Animations;
using SlugCrafting.Crafts;

namespace SlugCrafting;

public class PlayerCraftingData
{
    // --- Fields ---
    private float _scavengeTimer;
    private float _craftTimer;

    // --- Properties ---
    public Creature.Grasp containerViewGrasp;

    public List<Accessory> accessories { get; } = new();
    public Dictionary<Accessory.EquipRegion, Accessory> equipRegionAccessories { get; } = new();
    public float[] accessoriesMass { get; } = new float[2] { 0, 0 };
    public ScavengeSpot? currentTargetedScavengeSpot { get; set; }
    public AbstractPhysicalObjectScavenge? currentTargetedScavenge { get; set; }
    public Craft? currentPossibleCraft { get; set; }
    public WeakReference<Player> playerRef { get; }

    public float scavengeTimer
    {
        get => _scavengeTimer;
        set => _scavengeTimer = value;
    }

    public float craftTimer
    {
        get => _craftTimer;
        set => _craftTimer = value;
    }

    public bool isPhysicalCrafting => _craftTimer > 0;

    public bool physicalCraftingEnabled = false;

    public int craftAnimationIndex = 0;
    public int craftAnimationsTotalTimesLooped = 0;

    public int knifeGraspUsed = -1;
    public int creatureGraspUsed = -1;

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
    public static int GetOtherGrasp(int grasp) =>
        grasp == 0 ? 1 : 0;

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
    private static bool CanPerformCraft(Player self, Craft craft)
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

    internal static void OnInputCraftJustPressed(this Player player)
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

    internal static void WhileInputCraftPressed(this Player player)
    {
        var sCData = player.GetPlayerCraftingData();
        var animationPlayer = player.GetHandAnimationPlayer();

        if (sCData.currentPossibleCraft == null)
            return;

        var craft = sCData.currentPossibleCraft.Value;

        if (CanPerformCraft(player, craft))
        {
            player.swallowAndRegurgitateCounter = 0;
            sCData.craftTimer++;
        }
    }

    internal static void OnInputCraftJustReleased(this Player player)
    {
        var sCData = player.GetPlayerCraftingData();
        if (sCData.currentPossibleCraft == null) 
                return;

        var animationPlayer = player.GetHandAnimationPlayer();
        var craft = sCData.currentPossibleCraft.Value;
        var wouldBeAnim = craft.animations[sCData.craftAnimationIndex].animation;


        animationPlayer.Stop(wouldBeAnim);
        player.PhysicalCraftEnded();
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

    // --- Scavenging ---
    internal static void OnInputScavengeJustPressed(this Player self)
    {

    }

    internal static void WhileInputScavengePressed(this Player self)
    {
        var playerSCData = self.GetPlayerCraftingData();

        if (playerSCData.creatureGraspUsed != -1)
        {
            var currentScavenge = playerSCData.currentTargetedScavenge;

            if (currentScavenge != null
                && currentScavenge.canScavenge == true
                && (!currentScavenge.requiresKnife || playerSCData.knifeGraspUsed != -1)
                )
            {
                playerSCData.scavengeTimer++;

                // Cooldown before scavenge starts.
                if (playerSCData.scavengeTimer < 10)
                    return;

                currentScavenge.scavengeTime--;

                if (currentScavenge.scavengeTime <= 0)
                    CompleteScavenge(self, currentScavenge);
            }
        }
    }

    internal static void OnInputScavengeJustReleased(this Player self)
    {
        var playerSCData = self.GetPlayerCraftingData();
        playerSCData.scavengeTimer = 0; // Reset the timer to craft if not pressing craft.
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

    // --- Accessory Management ---
    public static void EquipAccessory(this Player player, Accessory accessory) => 
        player.GetPlayerCraftingData().accessories.Add(accessory);

    public static void UnequipAccessory(this Player player, Accessory accessory) => 
        player.GetPlayerCraftingData().accessories.Remove(accessory);

    public static void UpdateMass(this Player player) => 
        player.SetMalnourished(player.Malnourished);
}