namespace SlugCrafting;

public static class ProcessManagerHooks
{
    internal static void ApplyHooks()
    {
        On.ProcessManager.PostSwitchMainProcess += ProcessManager_PostSwitchMainProcess;
    }

    internal static void RemoveHooks()
    {
        On.ProcessManager.PostSwitchMainProcess -= ProcessManager_PostSwitchMainProcess;
    }

    internal static void ProcessManager_PostSwitchMainProcess(On.ProcessManager.orig_PostSwitchMainProcess orig, ProcessManager self, ProcessManager.ProcessID ID)
    {
    }
}
