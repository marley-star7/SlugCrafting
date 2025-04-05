using BepInEx;
using Fisobs.Core;
using System.Linq;
using System.Security.Permissions;
using UnityEngine;
using SlugCrafting.Items;

namespace SlugCrafting
{
    // There are two types of dependencies:
    // 1. BepInDependency.DependencyFlags.HardDependency - The other mod *MUST* be installed, and your mod cannot run without it. This ensures their mod loads before yours, preventing errors.
    // 2. BepInDependency.DependencyFlags.SoftDependency - The other mod doesn't need to be installed, but if it is, it should load before yours.
    //[BepInDependency("author.some_other_mods_guid", BepInDependency.DependencyFlags.HardDependency)]

    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    sealed class Plugin : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "marleystar7.slugcrafting"; // This should be the same as the id in modinfo.json!
        public const string PLUGIN_NAME = "Slug Crafting"; // This should be a human-readable version of your mod's name. This is used for log files and also displaying which mods get loaded. In general, it's a good idea to match this with your modinfo.json as well.
        public const string PLUGIN_VERSION = "0.0.1"; // This follows semantic versioning. For more information, see https://semver.org/ - again, match what you have in modinfo.json

        public void OnEnable()
        {
            Content.Register(new BoneKnifeFisob());

            // This method is called when the plugin is enabled
            Logger.LogInfo("Slug Crafting plugin is loaded!");
        }
    }
}
