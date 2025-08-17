// TODO: make item bundles specifically an abstraction of containers, and containers have the functionality for moving between em, 
// TODO: rename container to PhysicalObjectContainer, and then StorageContainer, ItemBundle, etc, that way can move between containers easily.
// TODO: each container has a lead object, which is the only one with collision.

namespace SlugCrafting;

//-- MS7: I might occasionally leave these "guide" comment's around in case someone wishes to learn modding based off this mod's code.
// I did something similar with "Da Vinki", open source is a helpful learning tool, and I gotta show gratitude by making it easier for the next guy.

// There are two types of dependencies:
// 1. BepInDependency.DependencyFlags.HardDependency - The other mod *MUST* be installed, and your mod cannot run without it. This ensures their mod loads before yours, preventing errors.
// 2. BepInDependency.DependencyFlags.SoftDependency - The other mod doesn't need to be installed, but if it is, it should load before yours.
//[BepInDependency("author.some_other_mods_guid", BepInDependency.DependencyFlags.HardDependency)]

[BepInDependency("slime-cubed.slugbase", BepInDependency.DependencyFlags.HardDependency)]
//[BepInDependency("Fisobs", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("marley-star7.marcustom", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("marley-star7.ccg", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("improved-input-config", BepInDependency.DependencyFlags.SoftDependency)]

[BepInPlugin(ID, NAME, VERSION)]
sealed class Plugin : BaseUnityPlugin
{
    public const string ID = "marley-star7.slugcrafting"; //-- This should be the same as the id in modinfo.json!
    public const string NAME = "Slug Crafting"; //-- This should be a human-readable version of your mod's name. This is used for log files and also displaying which mods get loaded. In general, it's a good idea to match this with your modinfo.json as well.
    public const string VERSION = "0.0.1"; //-- This follows semantic versioning. For more information, see https://semver.org/ - again, match what you have in modinfo.json

    public static bool isPostInit;
    public static bool restartMode = false;

    public static bool improvedInputEnabled;
    public static int improvedInputVersion = 0;

    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private static new ManualLogSource Logger;
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public void OnEnable()
    {
        //-- I love logging loggers, logging logs with my own logger, my personal logger.
        // Someday's I think about logging with my logger, and I get all loggy...
        Logger = base.Logger;

        //CompartmentalizedCreatureGraphics.Core.Content.AddCharacterCosmeticPreset(SlugCraftingEnums.Crafter, crafterCosmeticsPreset);

        Core.Content.RegisterSlugCraftingFisobs();
        RegisterSlugCraftingShelterCrafts();
        RegisterSlugCraftingCrafts();
        Core.Content.RegisterSlugCraftingItemBundlesProperties();

        On.RainWorld.OnModsInit += Extras.WrapInit(LoadPlugin);
        On.RainWorld.PostModsInit += RainWorld_PostModsInit;

        SlugCraftingEnums.PlayerHandAnimations.RegisterValues();

        try
        {
            Inputs.RegisterInputs();
        }
        catch
        {
            throw new Exception("Improved Input not enabled, or loaded after SlugCrafting.");
        }

        Logger.LogInfo("Slug Crafting is loaded!");
    }

    private static void LoadPlugin(RainWorld rainWorld)
    {
        Resources.LoadResources();

        //-- Do not re-apply hooks on restart mode!
        if (!restartMode)
        {
            Hooks.ApplyHooks();
        }
    }

    public void OnDisable()
    {
        //VLogger.LogInfo("OnDisable\n" + StackTraceUtility.ExtractStackTrace());
        if (restartMode)
        {
            Hooks.RemoveHooks();
        }
    }

    internal static void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld rainWorld)
    {
        orig(rainWorld);
        try
        {
            if (Plugin.isPostInit)
                return;
            else
                Plugin.isPostInit = true;

            Plugin.improvedInputEnabled = ModManager.ActiveMods.Exists((mod) => mod.id == "improved-input-config");
            Plugin.improvedInputVersion = Int32.Parse(ModManager.ActiveMods.First((mod) => mod.id == "improved-input-config").version.Substring(0, 1));
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e.Message);
        }
    }

    internal static void LogInfo(object ex) => Logger.LogInfo(ex);

    internal static void LogMessage(object ex) => Logger.LogMessage(ex);

    // -- Ms7: String prints are expensive!
    // So just incase we forget any #if's anywhere to encase debug logs to be for debug builds only to reduce hit on user performance.
    internal static void LogDebug(object ex)
    {
#if DEBUG
        Logger.LogDebug(ex);
#endif
    }

    internal static void LogWarning(object ex) => Logger.LogWarning(ex);

    internal static void LogError(object ex) => Logger.LogError(ex);

    internal static void LogFatal(object ex) => Logger.LogFatal(ex);

    //
    //-- CRAFTS
    //

    public static void DefaultImpaleObjectOnSpearCraftResult(Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject)
    {
        var spear = (Spear)primaryIngredientObject;
        var objectToImpale = (PhysicalObject)secondaryIngredientObject;

        spear.ImpalePhysicalObject(objectToImpale);
    }

    public static void DefaultTieObjectToCordCraftResult(Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject)
    {
        var objectToTie = (PhysicalObject)primaryIngredientObject;
        var cordObject = (CordItem)secondaryIngredientObject;

        cordObject.TieObject(objectToTie.abstractPhysicalObject, 0, 0);
    }

    public static bool PrimaryIngredientChunkNotScavengedValidation(in Craft craft, in Creature crafter, in PhysicalObject primaryIngredientObject, in PhysicalObject secondaryIngredientObject)
    {
        if (primaryIngredientObject is not Creature creature)
            return false;

        if (creature.abstractCreature.GetAbstractCreatureCraftingData().scavengedBodyChunks.Contains(craft.primaryIngredient.bodyChunkIndex))
            return false;

        return true;
    }

    public static bool SecondaryIngredientChunkNotScavengedValidation(in Craft craft, in Creature crafter, in PhysicalObject primaryIngredientObject, in PhysicalObject secondaryIngredientObject)
    {
        if (secondaryIngredientObject is not Creature creature)
            return false;

        if (creature.abstractCreature.GetAbstractCreatureCraftingData().scavengedBodyChunks.Contains(craft.secondaryIngredient.bodyChunkIndex))
            return false;

        return true;
    }

    public static void ScavengeLizardHeadInPrimaryHandCraftResult(Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject)
    {
        var lizard = crafter.grasps[0].grabbed as Lizard;

        crafter.ReleaseGrasp(0);

        var player = (crafter as Player);
        player.RealizeAndGrab(new AbstractLizardShell(lizard));
        lizard.abstractCreature.GetAbstractCreatureCraftingData().scavengedBodyChunks.Add(CreatureBodyChunkIndex.Lizard.Head);
    }

    public static void ScavengeLizardHeadInSecondaryHandCraftResult(Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject)
    {
        var lizard = crafter.grasps[1].grabbed as Lizard;

        crafter.ReleaseGrasp(1);

        var player = (crafter as Player);
        player.RealizeAndGrab(new AbstractLizardShell(lizard));
        lizard.abstractCreature.GetAbstractCreatureCraftingData().scavengedBodyChunks.Add(CreatureBodyChunkIndex.Lizard.Head);
    }

    private static void GreenLizardShellHelmetShelterCraftResult(in ShelterCraft shelterCraft, Creature crafter)
    {

    }

    internal static void RegisterSlugCraftingShelterCrafts()
    {
        Core.Content.RegisterShelterCraft(
            new ShelterCraft(
                new CraftIngredient[] { SlugCraftingEnums.CraftIngredients.GreenLizardShell },
                SlugCraftingEnums.AbstractObjectType.GreenLizardShellHelmet,
                GreenLizardShellHelmetShelterCraftResult
            )
        );
    }

    internal static void RegisterSlugCraftingCrafts()
    {
        //
        //-MS7 TODO: can probably remove the "CraftIngredient" type and just search by abstract object?
        // Use craft result instead to decide wether something is consumed via an easy function, would definitely fit nicer.
        //

        //-- CRAFT DATA
        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.WaterNut),
                secondaryIngredient = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant),

                ingredientValidation = (in Craft craft, in Creature crafter, in PhysicalObject primaryIngredientObject, in PhysicalObject secondaryIngredientObject) =>
                {
                    var waterNut = (WaterNut)primaryIngredientObject;

                    if (waterNut.AbstrNut.swollen)
                        return false;
                    else
                        return true;
                },

                craftResult = (Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject) =>
                {
                    crafter.RemoveGrabbedObject(0);
                    crafter.RemoveGrabbedObject(1);

                    var player = (crafter as Player);
                    player.RealizeAndGrab(new AbstractPhysicalObject(
                         crafter.room.world,
                         AbstractPhysicalObject.AbstractObjectType.ScavengerBomb,
                         null,
                         crafter.coord,
                         crafter.room.game.GetNewID()
                         ));
                },

                animations = new Craft.Animation[]
                {
                    new Craft.Animation(1, SlugCraftingEnums.PlayerHandAnimations.BiteStruggleNutLeftHand),
                    new Craft.Animation(1, SlugCraftingEnums.PlayerHandAnimations.DoubleSwallow)
                },
                needBothHandsFree = true,
            }
        );

        //
        // SPEAR PRIMARY CRAFTS
        //

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Spear),
                secondaryIngredient = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Rock),

                craftResult = (Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject) =>
                {
                    var spear = primaryIngredientObject as Spear;
                    //-- Spawn two break parts for all spears.
                    for (int i = 0; i < 2; i++)
                    {
                        crafter.room.AddObject(new ExplosiveSpear.SpearFragment(spear.firstChunk.pos, Custom.RNV() * Mathf.Lerp(3f, 6f, UnityEngine.Random.value), spear));
                    }
                    //-- Add a puff ball to fall off if the spear is explosive.
                    if (spear is ExplosiveSpear)
                    {
                        var explosiveSpear = spear as ExplosiveSpear;
                        crafter.room.AddObject(new PuffBallSkin(spear.firstChunk.pos + spear.rotation * (spear.pivotAtTip ? 0f : 10f), Custom.RNV() * Mathf.Lerp(3f, 6f, UnityEngine.Random.value), explosiveSpear.redColor, Color.Lerp(explosiveSpear.redColor, new Color(0f, 0f, 0f), 0.3f)));
                    }

                    //-- Delete the spear.
                    crafter.RemoveGrabbedObject(0);

                    var player = (crafter as Player);
                    player.RealizeAndGrab(
                        new AbstractKnife(
                             crafter.room.world,
                             SlugCraftingEnums.AbstractObjectType.Knife,
                             crafter.coord,
                             crafter.room.game.GetNewID()
                         )
                    );
                },

                animations = new Craft.Animation[]
                {
                    new Craft.Animation(1, SlugCraftingEnums.PlayerHandAnimations.KnapSpearFirstHit),
                    new Craft.Animation(5, SlugCraftingEnums.PlayerHandAnimations.KnapSpearLoop),
                    new Craft.Animation(1, SlugCraftingEnums.PlayerHandAnimations.KnapSpearBreak)
                },
            }
        );

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Spear),
                secondaryIngredient = new CraftIngredient(SlugCraftingEnums.AbstractObjectType.Knife),

                ingredientValidation = (in Craft craft, in Creature crafter, in PhysicalObject primaryIngredientObject, in PhysicalObject secondaryIngredientObject) =>
                {
                    // TODO: for some reason this validation isn't working / updating.
                    // Only can craft if the spear is not already sharpened.
                    var spear = (Spear)primaryIngredientObject;
                    if (spear.GetSpearCraftingData().sidedMode == SpearCraftingData.SidedMode.SingleSided)
                        return true;
                    else
                        return false;
                },

                craftResult = (Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject) =>
                {
                    //- MS7 was desperately trying to do a better method of making double sided spears, and failed.

                    /*
                    var origSpear = primaryIngredientObject as Spear;

                    var abstractDoubleSidedSpear = new AbstractDoubleSidedSpear(
                        origSpear.abstractPhysicalObject,
                        crafter.room.world,
                        null,
                        crafter.coord,
                        crafter.room.game.GetNewID()
                    );

                    var player = crafter as Player;

                    player.RemoveGrabbedObject(0);

                    //-- Realize it.
                    crafter.room.abstractRoom.AddEntity(abstractDoubleSidedSpear);
                    abstractDoubleSidedSpear.RealizeInRoom();

                    //-- Grab it
                    var newDoubleSidedSpear = abstractDoubleSidedSpear.realizedObject;
                    player.SlugcatGrab(newDoubleSidedSpear, player.FreeHand());

                    return null;
                    */

                    //-- Drop the original spear.
                    crafter.ReleaseGrasp(0);

                    var origSpear = primaryIngredientObject as Spear;
                    var origSpearCraftingData = origSpear.GetSpearCraftingData();

                    //-- Create the new spear front to hold.
                    var newAbstractFrontSpear = new AbstractSpear(
                        crafter.room.world,
                        null,
                        crafter.coord,
                        crafter.room.game.GetNewID(),
                        false // Not explosive
                    );

                    //-- Realize it.
                    crafter.room.abstractRoom.AddEntity(newAbstractFrontSpear);
                    newAbstractFrontSpear.RealizeInRoom();

                    //-- Make sure the spears are connected.
                    var newSpear = newAbstractFrontSpear.realizedObject as Spear;
                    var newSpearCraftingData = newSpear.GetSpearCraftingData();

                    origSpearCraftingData.sidedMode = SpearCraftingData.SidedMode.DoubleSidedBack;
                    origSpearCraftingData.oppositeSidedSpear = newSpear;

                    newSpearCraftingData.sidedMode = SpearCraftingData.SidedMode.DoubleSidedFront;
                    newSpearCraftingData.oppositeSidedSpear = origSpear;

                    //-- Grab the new spear front.
                    var player = crafter as Player;
                    player.SlugcatGrab(newSpear, player.FreeHand());
                },

                animations = new Craft.Animation[]
                {
                    new Craft.Animation(1, SlugCraftingEnums.PlayerHandAnimations.ImpaleOnSpear)
                },
            }
        );

        // --- Spear Impaling Crafts List ---

        var defaultImpaleOnSpearCraftAnimation = new Craft.Animation[]
        {
            new Craft.Animation(1, SlugCraftingEnums.PlayerHandAnimations.KnapSpearBreak)
        };

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Spear),
                secondaryIngredient = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Lantern),

                craftResult = DefaultImpaleObjectOnSpearCraftResult,
                animations = defaultImpaleOnSpearCraftAnimation,
            }
        );

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Spear),
                secondaryIngredient = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.SlimeMold),

                craftResult = DefaultImpaleObjectOnSpearCraftResult,
                animations = defaultImpaleOnSpearCraftAnimation,
            }
        );

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Spear),
                secondaryIngredient = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.SporePlant),

                ingredientValidation = (in Craft craft, in Creature crafter, in PhysicalObject _, in PhysicalObject secondaryIngredientObject) =>
                {
                    var sporePlant = (SporePlant)secondaryIngredientObject;
                    // Spore plant has to be pacified first.
                    if (sporePlant.AbstrSporePlant.pacified == true)
                        return true;
                    else
                        return false;
                },

                craftResult = DefaultImpaleObjectOnSpearCraftResult,
                animations = defaultImpaleOnSpearCraftAnimation,
            }
        );

        // --- String Tying Crafts List ---

        var defaultCordTieCraftAnimations = new Craft.Animation[]
        {
            new Craft.Animation(1, SlugCraftingEnums.PlayerHandAnimations.DoubleSwallow)
        };

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Lantern),
                secondaryIngredient = new CraftIngredient(SlugCraftingEnums.AbstractObjectType.Cord),

                craftResult = DefaultTieObjectToCordCraftResult,
                animations = defaultCordTieCraftAnimations
            }
        );

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.ScavengerBomb),
                secondaryIngredient = new CraftIngredient(SlugCraftingEnums.AbstractObjectType.Cord),

                craftResult = DefaultTieObjectToCordCraftResult,
                animations = defaultCordTieCraftAnimations
            }
        );

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Spear),
                secondaryIngredient = new CraftIngredient(SlugCraftingEnums.AbstractObjectType.Cord),

                craftResult = DefaultTieObjectToCordCraftResult,
                animations = defaultCordTieCraftAnimations
            }
        );

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Rock),
                secondaryIngredient = new CraftIngredient(SlugCraftingEnums.AbstractObjectType.Cord),

                craftResult = DefaultTieObjectToCordCraftResult,
                animations = defaultCordTieCraftAnimations
            }
        );

        /////////////////////////////////
        // --- Creature Scavenges --- ///
        /////////////////////////////////

        // --- Green Lizard --- //
        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = SlugCraftingEnums.CraftIngredients.GreenLizardHead,
                secondaryIngredient = SlugCraftingEnums.CraftIngredients.Knife,
                ingredientValidation = PrimaryIngredientChunkNotScavengedValidation,

                craftResult = ScavengeLizardHeadInPrimaryHandCraftResult,
                animations = SlugCraftingEnums.CraftAnimationSets.DefaultSawBackForthUsingLeftHandAnimations,
            }
        );

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = SlugCraftingEnums.CraftIngredients.Knife,
                secondaryIngredient = SlugCraftingEnums.CraftIngredients.GreenLizardHead,
                ingredientValidation = SecondaryIngredientChunkNotScavengedValidation,

                craftResult = ScavengeLizardHeadInSecondaryHandCraftResult,
                animations = SlugCraftingEnums.CraftAnimationSets.DefaultSawBackForthUsingRightHandAnimations,
            }
        );

        // --- Pink Lizard --- //
        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = SlugCraftingEnums.CraftIngredients.PinkLizardHead,
                secondaryIngredient = SlugCraftingEnums.CraftIngredients.Knife,
                ingredientValidation = PrimaryIngredientChunkNotScavengedValidation,

                craftResult = ScavengeLizardHeadInPrimaryHandCraftResult,
                animations = SlugCraftingEnums.CraftAnimationSets.DefaultSawBackForthUsingLeftHandAnimations,
            }
        );

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = SlugCraftingEnums.CraftIngredients.Knife,
                secondaryIngredient = SlugCraftingEnums.CraftIngredients.PinkLizardHead,
                ingredientValidation = SecondaryIngredientChunkNotScavengedValidation,

                craftResult = ScavengeLizardHeadInSecondaryHandCraftResult,
                animations = SlugCraftingEnums.CraftAnimationSets.DefaultSawBackForthUsingRightHandAnimations,
            }
        );

        // --- Blue Lizard --- //
        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = SlugCraftingEnums.CraftIngredients.BlueLizardHead,
                secondaryIngredient = SlugCraftingEnums.CraftIngredients.Knife,
                ingredientValidation = PrimaryIngredientChunkNotScavengedValidation,

                craftResult = ScavengeLizardHeadInPrimaryHandCraftResult,
                animations = SlugCraftingEnums.CraftAnimationSets.DefaultSawBackForthUsingLeftHandAnimations,
            }
        );

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = SlugCraftingEnums.CraftIngredients.Knife,
                secondaryIngredient = SlugCraftingEnums.CraftIngredients.BlueLizardHead,
                ingredientValidation = SecondaryIngredientChunkNotScavengedValidation,

                craftResult = ScavengeLizardHeadInSecondaryHandCraftResult,
                animations = SlugCraftingEnums.CraftAnimationSets.DefaultSawBackForthUsingRightHandAnimations,
            }
        );
    }
}