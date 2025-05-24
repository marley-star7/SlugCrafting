using BepInEx;
using UnityEngine;
using RWCustom;

using Fisobs.Core;
using ImprovedInput;

using SlugCrafting.Items;
using SlugCrafting.Scavenges;
using SlugCrafting.Crafts;
using MRCustom.Animations;
using BepInEx.Logging;
using SlugCrafting.Animations;

namespace SlugCrafting;

// There are two types of dependencies:
// 1. BepInDependency.DependencyFlags.HardDependency - The other mod *MUST* be installed, and your mod cannot run without it. This ensures their mod loads before yours, preventing errors.
// 2. BepInDependency.DependencyFlags.SoftDependency - The other mod doesn't need to be installed, but if it is, it should load before yours.
//[BepInDependency("author.some_other_mods_guid", BepInDependency.DependencyFlags.HardDependency)]

[BepInPlugin(ID, NAME, VERSION)]

// TODO: for some reason enabling these break it...
//[BepInDependency("fisobs", BepInDependency.DependencyFlags.HardDependency)]
//[BepInDependency("marleystar7.marcustom", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("improved-input-config", BepInDependency.DependencyFlags.SoftDependency)]

sealed class Plugin : BaseUnityPlugin
{
    public const string ID = "marleystar7.slugcrafting"; // This should be the same as the id in modinfo.json!
    public const string NAME = "Slug Crafting"; // This should be a human-readable version of your mod's name. This is used for log files and also displaying which mods get loaded. In general, it's a good idea to match this with your modinfo.json as well.
    public const string VERSION = "0.0.1"; // This follows semantic versioning. For more information, see https://semver.org/ - again, match what you have in modinfo.json

    public static bool restartMode = false;
    public static new ManualLogSource Logger;

    /// <summary>
    /// This method is called when the plugin is enabled
    /// </summary>
    public void OnEnable()
    {
        Logger = base.Logger;

        Inputs.Input.OnPluginEnable();
        Enums.HandAnimationIndex.RegisterValues();
        //Animations.Animation.OnPluginEnable();
        RegisterFisobs();
        RegisterCrafts();
        RegisterScavenges();

        // HOOKS
        Hooks.Hooks.ApplyHooks();
        On.RainWorld.OnModsInit += LoadResources;

        Logger.LogInfo("Slug Crafting plugin is loaded!");
    }

    /// <summary>
    /// This method is called when the plugin is disabled
    /// </summary>
    public void OnDisable()
    {
        Enums.HandAnimationIndex.UnregisterValues();
        //VLogger.LogInfo("OnDisable\n" + StackTraceUtility.ExtractStackTrace());
        if (restartMode)
        {
            Hooks.Hooks.RemoveHooks();
        }
    }

    // TODO: add wrapInit function, that just calles the orig self, removing the need for the line?

    /// <summary>
    /// Loads the resources needed for the mod.
    /// For example, load textures, sounds, etc.
    /// You can use the Futile.atlasManager.LoadAtlas method to load atlases.
    /// </summary>
    /// <param name="orig"></param>
    /// <param name="self"></param>
    private static void LoadResources(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        Futile.atlasManager.LoadAtlas("atlases/knife");
        Futile.atlasManager.LoadAtlas("atlases/boneKnife");
        Futile.atlasManager.LoadAtlas("atlases/lizardLeather");

        orig(self);
    }

    //
    // FISOBS
    //

    private static void RegisterFisobs()
    {
        Fisobs.Core.Content.Register(new KnifeFisob());
        Fisobs.Core.Content.Register(new LizardHideFisob());

        Fisobs.Core.Content.Register(new GreenLizardShellFisob());
        Fisobs.Core.Content.Register(new PinkLizardShellFisob());
    }

    //
    // CRAFTS
    //

    private static void RegisterCrafts()
    {
        // CRAFT DATA
        SlugCrafting.Core.Content.RegisterCraft(
            AbstractPhysicalObject.AbstractObjectType.WaterNut,
            AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant,
            new Craft()
            {
                primaryIngredient = new CraftIngredient()
                {
                    type = AbstractPhysicalObject.AbstractObjectType.WaterNut,
                    validation = (in PhysicalObject physicalObject) =>
                    {
                        // Water nut has to be not swollen
                        if (physicalObject is WaterNut && ((WaterNut)physicalObject).AbstrNut.swollen == false)
                            return true;

                        return false;
                    },
                    consume = true,
                },
                secondaryIngredient = new CraftIngredient()
                {
                    type = AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant,
                    consume = true,
                },
                craftResult = (Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject) =>
                {
                    return new AbstractPhysicalObject(
                         crafter.room.world,
                         AbstractPhysicalObject.AbstractObjectType.ScavengerBomb,
                         null,
                         crafter.coord,
                         crafter.room.game.GetNewID()
                     );
                },
                craftTime = 40,
                handAnimationIndex = Enums.HandAnimationIndex.DoubleSwallowCraft,
                handAnimation = new SwallowCraftPlayerHandAnimation(40)
                {
                }
            }
        );

        SlugCrafting.Core.Content.RegisterCraft(
            AbstractPhysicalObject.AbstractObjectType.Spear,
            AbstractPhysicalObject.AbstractObjectType.Rock,
            new Craft()
            {
                primaryIngredient = new CraftIngredient()
                {
                    type = AbstractPhysicalObject.AbstractObjectType.Spear,
                    consume = true,
                },
                secondaryIngredient = new CraftIngredient()
                {
                    type = AbstractPhysicalObject.AbstractObjectType.Rock,
                    consume = false,
                },
                craftResult = (Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject) =>
                {
                    var spear = primaryIngredientObject as Spear;
                    for (int i = 0; i < 2; i++)
                    {
                        crafter.room.AddObject(new ExplosiveSpear.SpearFragment(spear.firstChunk.pos, Custom.RNV() * Mathf.Lerp(3f, 6f, UnityEngine.Random.value), spear));
                    }

                    return new AbstractKnife(
                         crafter.room.world,
                         KnifeFisob.abstractObjectType,
                         crafter.coord,
                         crafter.room.game.GetNewID()
                     );
                },
                craftTime = 200,
                handAnimationIndex = Enums.HandAnimationIndex.SmashIntoCraft,
                handAnimation = new SmashIntoCraftPlayerHandAnimation(200)
                {
                    timeBetweenBeats = 20f,
                    sinBeatingCurveStartRad = 0.7f,

                    beatSound = SoundID.Spear_Bounce_Off_Wall,
                    breakSound = SoundID.Spear_Fragment_Bounce,

                    fullRiseHandOffsetPos = new Vector2(13f, 9f),
                    fullDescentHandOffsetPos = new Vector2(-8f, -17f),
                }
            }
        );
    }

    //
    // SCAVENGES
    //

    private static void RegisterScavenges()
    {
        // SCAVENGE DATA
        SlugCrafting.Core.Content.RegisterScavengeData(CreatureTemplate.Type.PinkLizard, typeof(GreenLizardScavengeData));
        SlugCrafting.Core.Content.RegisterScavengeData(CreatureTemplate.Type.PinkLizard, typeof(PinkLizardScavengeData));
    }
}