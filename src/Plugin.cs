using BepInEx;
using UnityEngine;
using RWCustom;

using Fisobs.Core;
using ImprovedInput;

using SlugCrafting.Items;
using SlugCrafting.Scavenges;

namespace SlugCrafting;

// There are two types of dependencies:
// 1. BepInDependency.DependencyFlags.HardDependency - The other mod *MUST* be installed, and your mod cannot run without it. This ensures their mod loads before yours, preventing errors.
// 2. BepInDependency.DependencyFlags.SoftDependency - The other mod doesn't need to be installed, but if it is, it should load before yours.
//[BepInDependency("author.some_other_mods_guid", BepInDependency.DependencyFlags.HardDependency)]

[BepInPlugin(ID, NAME, VERSION)]
[BepInDependency("improved-input-config", BepInDependency.DependencyFlags.SoftDependency)]
sealed class Plugin : BaseUnityPlugin
{
    public const string ID = "marleystar7.slugcrafting"; // This should be the same as the id in modinfo.json!
    public const string NAME = "Slug Crafting"; // This should be a human-readable version of your mod's name. This is used for log files and also displaying which mods get loaded. In general, it's a good idea to match this with your modinfo.json as well.
    public const string VERSION = "0.0.1"; // This follows semantic versioning. For more information, see https://semver.org/ - again, match what you have in modinfo.json

    public static bool restartMode = false;

    /// <summary>
    /// This method is called when the plugin is enabled
    /// </summary>
    public void OnEnable()
    {
        Inputs.Input.OnPluginEnable();

        // FISOBS
        Fisobs.Core.Content.Register(new KnifeFisob());
        Fisobs.Core.Content.Register(new LizardLeatherFisob());

        Fisobs.Core.Content.Register(new GreenLizardShellFisob());
        Fisobs.Core.Content.Register(new PinkLizardShellFisob());

        // SCAVENGE DATA
        SlugCrafting.Core.Content.RegisterScavengeData(CreatureTemplate.Type.PinkLizard, typeof(GreenLizardScavengeData));
        SlugCrafting.Core.Content.RegisterScavengeData(CreatureTemplate.Type.PinkLizard, typeof(PinkLizardScavengeData));

        // HOOKS
        Hooks.ApplyHooks();
        On.RainWorld.OnModsInit += LoadResources;

        Logger.LogInfo("Slug Crafting plugin is loaded!");
    }

    /// <summary>
    /// This method is called when the plugin is disabled
    /// </summary>
    public void OnDisable()
    {
        //VLogger.LogInfo("OnDisable\n" + StackTraceUtility.ExtractStackTrace());
        if (restartMode)
        {
            Hooks.RemoveHooks();
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
}
