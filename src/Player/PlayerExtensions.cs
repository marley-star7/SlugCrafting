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

    public HandCraft? currentPossibleHandCraft { get; set; }

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

    public bool isHandCrafting => _craftTimer > 0;

    public bool handCraftingEnabled = false;

    public WeakReference<Player> playerRef { get; }

    public PlayerCraftingData(Player player)
    {
        playerRef = new WeakReference<Player>(player);
        player.GetHandAnimationPlayer().AnimationLooped += OnPlayerHandAnimationLooped;

        if (player.IsCrafter() 
            || player.SlugCatClass.value == "Project") // Temp code for testing.
        {
            handCraftingEnabled = true;
        }
    }

    private void OnPlayerHandAnimationLooped(PlayerHandAnimationPlayer.AnimationIndex loopedAnimation, int timesLooped)
    {
        if (!playerRef.TryGetTarget(out Player player) || currentPossibleHandCraft == null)
            return;

        var handCraft = currentPossibleHandCraft.Value;
        var currentCraftAnimation = handCraft.animations[craftAnimationIndex];

        craftAnimationsTotalTimesLooped++;

        Plugin.LogDebug($"totalAnimationLoops: {handCraft.totalAnimationLoops}, loops: {timesLooped}, {isHandCrafting}, {loopedAnimation == currentCraftAnimation.animationIndex}");

        if (isHandCrafting && loopedAnimation == currentCraftAnimation.animationIndex)
        {
            if (craftAnimationsTotalTimesLooped >= handCraft.totalAnimationLoops)
                player.CompletePhysicalCraft(handCraft);
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
    private static readonly ConditionalWeakTable<Player, PlayerCraftingData> _craftingDataTable = new();

    public static PlayerCraftingData GetPlayerCraftingData(this Player player) =>
        _craftingDataTable.GetValue(player, p => new PlayerCraftingData(p));

    public static bool IsCrafter(this Player player) =>
        player.slugcatStats.name == Enums.SlugcatStats.Name.Crafter;

    public static bool CanHandCraft(this Player player) => player.GetPlayerCraftingData().handCraftingEnabled;

    public static CraftRecipe.Material? GetCraftRecipeMaterialInGrasp(this Player player, int graspIndex)
    {
        var grasp = player.grasps[graspIndex];

        if (grasp != null && grasp.grabbed != null)
        {
            AbstractPhysicalObject.AbstractObjectType graspObjectType = grasp.grabbed.abstractPhysicalObject.type;
            CreatureTemplate.Type? graspCreatureTemplateType = null;

            if (grasp.grabbed is Creature grabbedGreature)
                graspCreatureTemplateType = grabbedGreature.Template.type;

            return new CraftRecipe.Material(graspObjectType, grasp.chunkGrabbed, graspCreatureTemplateType);
        }
        return null;
    }

    // --- Grasping Utilities ---
    public static HandCraft? GetGraspsHandCraft(this Player player)
    {
        var craftingData = player.GetPlayerCraftingData();
        var animationPlayer = player.GetHandAnimationPlayer();

        CraftRecipe.Material? primaryMaterial = player.GetCraftRecipeMaterialInGrasp(0);
        CraftRecipe.Material? secondaryMaterial = player.GetCraftRecipeMaterialInGrasp(1);

        if (Content.HandCrafts.TryGetValue((primaryMaterial, secondaryMaterial), out var handCraft) &&
            handCraft.ingredientValidation(handCraft, player, player.grasps[0].grabbed, player.grasps[1].grabbed))
        {
            return handCraft;
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
    public static bool CanPerformCraftInCurrentAnimation(this Player self, HandCraft handCraft)
    {
        if (handCraft.recipe.bodyModeRequirement == CraftRecipe.BodyModeRequirement.Stand && !(self.bodyMode == Player.BodyModeIndex.Stand || self.bodyMode == Player.BodyModeIndex.Default))
            return false;

        else if (handCraft.recipe.bodyModeRequirement == CraftRecipe.BodyModeRequirement.Sneak && self.bodyMode != Player.BodyModeIndex.Crawl)
            return false;

        if (self.animation != Player.AnimationIndex.None &&
            self.animation != Player.AnimationIndex.StandUp &&
            self.animation != Player.AnimationIndex.StandOnBeam &&
            self.animation != Player.AnimationIndex.BeamTip &&
            self.animation != Player.AnimationIndex.DownOnFours)
            return false;

        if (handCraft.needBothHandsFree && self.animation == Player.AnimationIndex.ClimbOnBeam)
            return false;

        return true;
    }

    public static void ContinueToNextCraftAnimation(this Player self)
    {
        var playerSCData = self.GetPlayerCraftingData();

        playerSCData.craftAnimationIndex++;
        var nextCraftAnimation = playerSCData.currentPossibleHandCraft.Value.animations[playerSCData.craftAnimationIndex].animationIndex;
        self.GetHandAnimationPlayer().Play(nextCraftAnimation);
    }

    public static void LoseHandCraft(this Player self)
    {
        var playerSlugCraftingData = self.GetPlayerCraftingData();
        playerSlugCraftingData.currentPossibleHandCraft = null;

        EndHandCraft(self);
    }

    private static void EndHandCraft(this Player self)
    {
        var playerSlugCraftingData = self.GetPlayerCraftingData();

        if (playerSlugCraftingData.currentPossibleHandCraft != null)
        {
            self.GetHandAnimationPlayer().Stop(playerSlugCraftingData.currentPossibleHandCraft.Value.animations[playerSlugCraftingData.craftAnimationIndex].animationIndex);
        }

        // RESET TIMER FOR NEXT CRAFT.
        playerSlugCraftingData.craftTimer = 0;
        playerSlugCraftingData.craftAnimationIndex = 0;
        playerSlugCraftingData.craftAnimationsTotalTimesLooped = 0;
    }

    public static void CheckGraspsForPossibleHandCraft(this Player player)
    {
        var playerSlugCraftingData = player.GetPlayerCraftingData();

        var newPossibleHandCraft = player.GetGraspsHandCraft();

        // If we don't have one a craft anymore, run code to lose it.
        if (playerSlugCraftingData.currentPossibleHandCraft != null && newPossibleHandCraft == null)
        {
            player.LoseHandCraft();
        }

        playerSlugCraftingData.currentPossibleHandCraft = newPossibleHandCraft;
    }

    internal static void CraftInputStart(this Player player)
    {
        player.CheckGraspsForPossibleHandCraft();

        var sCData = player.GetPlayerCraftingData();
        if (sCData.currentPossibleHandCraft == null)
            return;

        var handCraft = sCData.currentPossibleHandCraft.Value;

#if DEBUG
        Plugin.LogDebug($"-- Player starting handCraft! --");
        Plugin.LogDebug($"Primary ingredient: {handCraft.primaryIngredient.material.objectType} {handCraft.primaryIngredient.material.creatureType} | chunk: {handCraft.primaryIngredient.material.bodyChunkIndex}");
        Plugin.LogDebug($"Secondary ingredient: {handCraft.secondaryIngredient.material.objectType} {handCraft.secondaryIngredient.material.creatureType} | chunk: {handCraft.secondaryIngredient.material.bodyChunkIndex}");
#endif

        var animationPlayer = player.GetHandAnimationPlayer();
        var currentAnim = handCraft.animations[sCData.craftAnimationIndex].animationIndex;

        animationPlayer.Play(currentAnim);
    }

    internal static void CraftInputUpdate(this Player player)
    {
        var sCData = player.GetPlayerCraftingData();
        var animationPlayer = player.GetHandAnimationPlayer();

        if (sCData.currentPossibleHandCraft == null)
        {
            return;
        }

        if (player.CanPerformCraftInCurrentAnimation(sCData.currentPossibleHandCraft.Value))
        {
            player.swallowAndRegurgitateCounter = 0;
            player.noGrabCounter = 1; // Cannot grab poles while crafting.
            sCData.craftTimer++;
        }
    }

    internal static void CraftInputRelease(this Player self)
    {
        var selfCData = self.GetPlayerCraftingData();
        if (selfCData.currentPossibleHandCraft == null) 
                return;

        var animationPlayer = self.GetHandAnimationPlayer();
        var handCraft = selfCData.currentPossibleHandCraft.Value;
        var wouldBeAnim = handCraft.animations[selfCData.craftAnimationIndex].animationIndex;

        animationPlayer.Stop(wouldBeAnim);
        self.EndHandCraft();
    }

    public static void CompletePhysicalCraft(this Player self, HandCraft handCraft)
    {
        var primaryCraftObject = self.grasps[0].grabbed;
        var secondaryCraftObject = self.grasps[1].grabbed;
        handCraft.craftResult(self, primaryCraftObject, secondaryCraftObject);

        self.GetHandAnimationPlayer().Stop();

        // UPDATE THE CURRENT POSSIBLE CRAFTS
        self.CheckGraspsForPossibleHandCraft();
        self.EndHandCraft();
    }

    public static void CancelPhysicalCraft(this Player self)
    {
        var playerSlugCraftingData = self.GetPlayerCraftingData();

        if (playerSlugCraftingData.currentPossibleHandCraft != null)
        {
            playerSlugCraftingData.craftTimer = -1;

            var playerHandAnimationPlayer = self.GetHandAnimationPlayer();
            playerHandAnimationPlayer.Stop(playerSlugCraftingData.currentPossibleHandCraft.Value.animations[playerSlugCraftingData.craftAnimationIndex].animationIndex);
        }
        self.EndHandCraft();
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

    // --- Accessory Management ---
    public static void EquipAccessory(this Player player, Accessory accessory) => 
        player.GetPlayerCraftingData().accessories.Add(accessory);

    public static void UnequipAccessory(this Player player, Accessory accessory) => 
        player.GetPlayerCraftingData().accessories.Remove(accessory);

    public static void UpdateMass(this Player player) => 
        player.SetMalnourished(player.Malnourished);
}