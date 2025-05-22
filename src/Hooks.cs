namespace SlugCrafting.Hooks;

public static partial class Hooks
{
    // Add hooks
    internal static void ApplyHooks()
    {
        ApplyPlayerHooks();
        ApplyPlayerGraphicsHooks();
        ApplyLizardGraphicsHooks();

        MRCustom.Events.OnPlayerGrab += PlayerExtension.OnPlayerGrab;
        MRCustom.Events.OnPlayerReleaseGrasp += PlayerExtension.OnPlayerReleaseGrasp;
        MRCustom.Events.OnPlayerSwitchGrasp += PlayerExtension.OnPlayerSwitchGrasp;
    }

    // Remove hooks
    internal static void RemoveHooks()
    {
        RemovePlayerHooks();
        RemovePlayerGraphicsHooks();
        RemoveLizardGraphicsHooks();

        MRCustom.Events.OnPlayerGrab -= PlayerExtension.OnPlayerGrab;
        MRCustom.Events.OnPlayerReleaseGrasp -= PlayerExtension.OnPlayerReleaseGrasp;
        MRCustom.Events.OnPlayerSwitchGrasp -= PlayerExtension.OnPlayerSwitchGrasp;
    }
}