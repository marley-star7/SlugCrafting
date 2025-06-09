using UnityEngine;
using RWCustom;

using BepInEx;
using BepInEx.Logging;

namespace SlugCrafting;

//-- MR7: I might occasionally leave these "guide" comment's around in case someone wishes to learn modding based off this mod's code.
// I did something similar with "Da Vinki", open source is a helpful learning tool, and I gotta show gratitude by making it easier for the next guy.

// There are two types of dependencies:
// 1. BepInDependency.DependencyFlags.HardDependency - The other mod *MUST* be installed, and your mod cannot run without it. This ensures their mod loads before yours, preventing errors.
// 2. BepInDependency.DependencyFlags.SoftDependency - The other mod doesn't need to be installed, but if it is, it should load before yours.
//[BepInDependency("author.some_other_mods_guid", BepInDependency.DependencyFlags.HardDependency)]

// TODO: for some reason enabling these break it...
//[BepInDependency("slime-cubed.slugbase")]
//[BepInDependency("fisobs", BepInDependency.DependencyFlags.HardDependency)]
//[BepInDependency("marleystar7.marcustom", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("improved-input-config", BepInDependency.DependencyFlags.SoftDependency)]

[BepInPlugin(ID, NAME, VERSION)]
sealed class Plugin : BaseUnityPlugin
{
    public const string ID = "marleystar7.slugcrafting"; //-- This should be the same as the id in modinfo.json!
    public const string NAME = "Slug Crafting"; //-- This should be a human-readable version of your mod's name. This is used for log files and also displaying which mods get loaded. In general, it's a good idea to match this with your modinfo.json as well.
    public const string VERSION = "0.0.1"; //-- This follows semantic versioning. For more information, see https://semver.org/ - again, match what you have in modinfo.json

    public static bool isPostInit;
    public static bool restartMode = false;

    public static bool improvedInputEnabled;
    public static int improvedInputVersion = 0;

    internal static ManualLogSource Logger;

    public void OnEnable()
    {
        //-- I love logging loggers, logging logs with my own logger, my personal logger.
        // Someday's I think about logging with my logger, and I get all loggy...
        Logger = base.Logger;

        Enums.HandAnimationIndex.RegisterValues();

        Core.Content.RegisterSlugCraftingFisobs();
        Core.Content.RegisterSlugCraftingCrafts();
        Core.Content.RegisterSlugCraftingItemBundlesProperties();
        Core.Content.RegisterSlugCraftingScavenges();

        On.RainWorld.OnModsInit += Extras.WrapInit(LoadPlugin);
        On.RainWorld.PostModsInit += RainWorld_PostModsInit;

        Logger.LogInfo("Slug Crafting plugin is loaded!");
    }

    private static void LoadPlugin(RainWorld rainWorld)
    {
        //-- Do not re-apply hooks on restart mode!
        if (!restartMode)
        {
            Hooks.ApplyHooks();
        }

        Resources.LoadResources();
    }

    public void OnDisable()
    {
        Enums.HandAnimationIndex.UnregisterValues();
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
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError(e.Message);
        }

        Plugin.improvedInputEnabled = ModManager.ActiveMods.Exists((mod) => mod.id == "improved-input-config");
        if (Plugin.improvedInputEnabled)
        {
            try
            {
                Inputs.RegisterInputs();
            }
            catch
            {
                throw new Exception("Improved Input enabled but also not enabled???");
            }
        }

        Plugin.improvedInputVersion = Int32.Parse(ModManager.ActiveMods.First((mod) => mod.id == "improved-input-config").version.Substring(0, 1));
    }
}