using SlugBase.SaveData;

namespace SlugCrafting;

public static partial class Hooks
{
    private static void ApplyRainWorldGameHooks()
    {
        On.RainWorldGame.ctor += RainWorldGame_ctor;
    }

    private static void RemoveRainWorldGameHooks()
    {
        On.RainWorldGame.ctor -= RainWorldGame_ctor;
    }

    private static void RainWorldGame_ctor(On.RainWorldGame.orig_ctor orig, RainWorldGame self, ProcessManager manager)
    {
        orig(self, manager);

        if (Plugin.restartMode)
        {
            ApplyHooks();

            Resources.LoadResources();
            Plugin.RainWorld_PostModsInit((_) => { }, self.rainWorld);
        }
    }
}