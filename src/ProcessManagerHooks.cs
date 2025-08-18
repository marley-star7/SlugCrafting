
using SlugCrafting.Menus;

namespace SlugCrafting;

public static class ProcessManagerHooks
{
    internal static void ApplyHooks()
    {
        On.ProcessManager.RequestMainProcessSwitch_ProcessID += ProcessManager_RequestMainProcessSwitch_ProcessID;
        On.ProcessManager.PostSwitchMainProcess += ProcessManager_PostSwitchMainProcess;
    }

    internal static void RemoveHooks()
    {
        On.ProcessManager.RequestMainProcessSwitch_ProcessID -= ProcessManager_RequestMainProcessSwitch_ProcessID;
        On.ProcessManager.PostSwitchMainProcess -= ProcessManager_PostSwitchMainProcess;
    }

    private static void ProcessManager_RequestMainProcessSwitch_ProcessID(On.ProcessManager.orig_RequestMainProcessSwitch_ProcessID orig, ProcessManager self, ProcessManager.ProcessID ID)
    {
        // -- Ms7: I know this functionality should be an ILhook into rain world game and not... this, this is bad practice lol.
        // BUT, I don't care right now, this works and removes need for ILHook, avoiding the standard.

        if (ID == ProcessManager.ProcessID.SleepScreen && self.currentMainLoop.ID != SlugCraftingEnums.ProcessID.ShelterCraft)
            ID = SlugCraftingEnums.ProcessID.ShelterCraft;

        orig(self, ID);
    }


    private static void ProcessManager_PostSwitchMainProcess(On.ProcessManager.orig_PostSwitchMainProcess orig, ProcessManager self, ProcessManager.ProcessID ID)
    {
        if (ID == SlugCraftingEnums.ProcessID.ShelterCraft)
        {
            self.currentMainLoop = new ShelterCraftScreen(self, ID);
        }

        orig(self, ID);
    }

}

