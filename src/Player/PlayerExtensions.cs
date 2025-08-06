using MRCustom.Animations;
using SlugCrafting.Crafts;

namespace SlugCrafting;

public class PlayerCraftingData
{
    // --- Fields ---
    private float _scavengeTimer;
    private float _craftTimer;

    // --- Properties ---
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

        if (isPhysicalCrafting && loopedAnimation == currentCraftAnimation.animation)
        {
            if (timesLooped >= craft.totalAnimationLoops)
                player.CompletePhysicalCraft(craft);
            else if (timesLooped > currentCraftAnimation.loopsInAnimation)
                craftAnimationIndex++;
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

    // --- Bundling Logic ---
    public static void BundleGrabUpdate(this Player selfPlayer, bool eu)
    {
        if (!selfPlayer.JustPressed(ImprovedInput.PlayerKeybind.Grab))
            return;

        //-- MS7: TODO: TEMP FUNCTIONALITY FOR TESTING, LATER HAVE DIFFERENT WAY OF EQUIPPING.
        if (selfPlayer.grasps[0] != null && selfPlayer.grasps[0].grabbed is IEquippable accessoryItem)
        {
            accessoryItem.Equip(selfPlayer);
        }

        //-- MS7: Prioritize items in the primary hand first for checks, following the standard of primary item's being the focus for interaction.
        // As alternate into primary gives for the least amount of button presses only when need an item on demand to be used immediately from a bundle,
        // (which can be mitigated via just having an empty hand mean that the bundle is always pulled from into it.)
        //-- Otherwise in cases like a backpack, where you are interacting with that first item, and pulling out of it, it being in the primary, pulling into alternate,
        // since it has other interaction is best. Which you then will likely throw the backpack on your back again or on the ground if is an emergency.

        if (selfPlayer.grasps[0] != null && selfPlayer.grasps[0].grabbed is PlayerCarryableItem)
        {
            var primaryHandItem = selfPlayer.grasps[0].grabbed as PlayerCarryableItem;
            //-- MS7: First look to add Item to second hand's bundle if second hand has item and is bundleable.
            // If second hand empty, pop an item from the primary hand's bundle to put in alternate.

            if (selfPlayer.grasps[1] != null && selfPlayer.grasps[1].grabbed is PlayerCarryableItem) // Both Hands Have Items
            {
                var secondaryHandItem = selfPlayer.grasps[1].grabbed as PlayerCarryableItem;

                if (primaryHandItem.CanBundleWith(secondaryHandItem))
                    secondaryHandItem.AddItemToBundle(primaryHandItem);
                else //-- MS7: Just switch grasps instead, for quality of life.
                    selfPlayer.SwitchHands();
            }
            else
            {
                if (primaryHandItem.GetBundle() != null)
                    selfPlayer.SlugcatGrab(primaryHandItem.PopItemFromBundle(), selfPlayer.FreeHand());
            }
        }
        //-- MS7: Only if primary hand is empty, do we fall back on checking the secondary hand for a bundle to pull from.
        else if (selfPlayer.grasps[1] != null && selfPlayer.grasps[1].grabbed is PlayerCarryableItem)
        {
            var secondaryHandItem = selfPlayer.grasps[1].grabbed as PlayerCarryableItem;

            selfPlayer.SlugcatGrab(secondaryHandItem.PopItemFromBundle(), selfPlayer.FreeHand());
        }
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

    private static void PhysicalCraftEnded(this Player self)
    {
        var playerSlugCraftingData = self.GetPlayerCraftingData();

        // RESET TIMER FOR NEXT CRAFT.
        playerSlugCraftingData.craftTimer = 0;
        playerSlugCraftingData.craftAnimationIndex = 0;
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