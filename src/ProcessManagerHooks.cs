
using SlugCrafting.Crafts;
using SlugCrafting.Menus;

namespace SlugCrafting;

public static class ProcessManagerHooks
{
    internal static void ApplyHooks()
    {
        On.ProcessManager.RequestMainProcessSwitch_ProcessID += ProcessManager_RequestMainProcessSwitch_ProcessID;
        On.ProcessManager.PostSwitchMainProcess += ProcessManager_PostSwitchMainProcess;

        IL.ProcessManager.PostSwitchMainProcess += ProcessManager_PostSwitchMainProcess;

        On.PlayerProgression.Revert += PlayerProgression_Revert;
    }

    internal static void RemoveHooks()
    {
        On.ProcessManager.RequestMainProcessSwitch_ProcessID -= ProcessManager_RequestMainProcessSwitch_ProcessID;
        On.ProcessManager.PostSwitchMainProcess -= ProcessManager_PostSwitchMainProcess;

        IL.ProcessManager.PostSwitchMainProcess -= ProcessManager_PostSwitchMainProcess;

        On.PlayerProgression.Revert -= PlayerProgression_Revert;
    }

    private static void PlayerProgression_Revert(On.PlayerProgression.orig_Revert orig, PlayerProgression self)
    {
        // -- Ms7: Testing purposes to see if IL worked.

        orig(self);
        Plugin.LogDebug("Base game did a progression reverted!");
    }

    private static void ProcessManager_RequestMainProcessSwitch_ProcessID(On.ProcessManager.orig_RequestMainProcessSwitch_ProcessID orig, ProcessManager self, ProcessManager.ProcessID ID)
    {
        // -- Ms7: I know this functionality should be an ILhook into rain world game and not... this, this is bad practice lol.
        // BUT, I don't care right now, this works and removes need for ILHook, avoiding the standard.

        if (ID == ProcessManager.ProcessID.SleepScreen && self.currentMainLoop.ID != SlugCraftingEnums.ProcessID.ShelterCraft)
            ID = SlugCraftingEnums.ProcessID.ShelterCraft;

        orig(self, ID);
    }


    internal static void ProcessManager_PostSwitchMainProcess(ILContext il)
    {
        // -- Ms7: Hook for this:
        // if (ID != ProcessID.Initialization && ID != ProcessID.SleepScreen && ID != ProcessID.GhostScreen && ID != ProcessID.DeathScreen && ID != ProcessID.KarmaToMaxScreen && ID != ProcessID.Dream && ID != ProcessID.StarveScreen && (!ModManager.MSC || (ID != MoreSlugcatsEnums.ProcessID.KarmaToMinScreen && ID != MoreSlugcatsEnums.ProcessID.VengeanceGhostScreen)))
        // {
        //    rainWorld.progression.Revert();
        // }

        ILCursor cursor = new ILCursor(il);

        try
        {
            cursor.Index = 0;

            // First find the position where the Revert() sequence starts
            if (cursor.TryGotoNext(MoveType.Before,
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<ProcessManager>("rainWorld"),
                x => x.MatchLdfld<RainWorld>("progression"),
                x => x.MatchCallvirt<PlayerProgression>("Revert")
            ))
            {
                // Mark the start of the sequence as our jump-back point
                var beforeRevert = cursor.DefineLabel();
                cursor.MarkLabel(beforeRevert);

                // Find the position AFTER Revert() call to mark our jump target
                cursor.TryGotoNext(MoveType.After,
                    x => x.MatchCallvirt<PlayerProgression>("Revert"));

                var afterRevert = cursor.DefineLabel();
                cursor.MarkLabel(afterRevert);

                // Go back to the start of the sequence
                cursor.GotoLabel(beforeRevert);

                // Load the ID argument (arg 1)
                cursor.Emit(OpCodes.Ldarg_1);
                cursor.EmitDelegate((ProcessManager.ProcessID id) =>
                {
                    var isShelterCraft = id == SlugCraftingEnums.ProcessID.ShelterCraft;
                    Plugin.LogDebug($"ProcessID: {id}, IsShelterCraft: {isShelterCraft}");
                    return isShelterCraft;
                });

                // Skip the Revert() call if condition is true
                cursor.Emit(OpCodes.Brtrue, afterRevert);
            }
        }
        catch (Exception ex)
        {
            Plugin.LogError(
                $"Error at cursor index {cursor.Index}:\n" +
                $"Exception: {ex}\n\n" +
                $"IL Context:\n{il}");
            throw;
        }
    }


    private static void ProcessManager_PostSwitchMainProcess(On.ProcessManager.orig_PostSwitchMainProcess orig, ProcessManager self, ProcessManager.ProcessID ID)
    {
        if (ID == SlugCraftingEnums.ProcessID.ShelterCraft)
        {
            self.currentMainLoop = new ShelterCraftScreen(self, ID);
        }

        Plugin.LogDebug($"Switching main process to {ID}");

        orig(self, ID);

        Plugin.LogDebug($"Post Swtiched main process to {ID}");
    }

}

