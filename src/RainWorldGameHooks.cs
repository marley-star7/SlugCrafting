using SlugCrafting.Menus;

namespace SlugCrafting;

internal static class RainWorldGameHooks
{
    internal static void ApplyHooks()
    {
        On.RainWorldGame.ctor += RainWorldGame_ctor;

        IL.RainWorldGame.CommunicateWithUpcomingProcess += RainWorldGame_CommunicateWithUpcomingProcess;
    }

    internal static void RemoveHooks()
    {
        On.RainWorldGame.ctor -= RainWorldGame_ctor;

        IL.RainWorldGame.CommunicateWithUpcomingProcess -= RainWorldGame_CommunicateWithUpcomingProcess;
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

    private static void RainWorldGame_CommunicateWithUpcomingProcess(ILContext il)
    {
        ILCursor cursor = new ILCursor(il);
        Plugin.LogDebug("=== Starting CommunicateWithUpcomingProcess patch ===");

        try
        {
            cursor.Index = 0;
            Plugin.LogDebug($"Initial cursor position: {cursor.Index}");

            // Debug all local variables
            Plugin.LogDebug("Local variables:");
            foreach (var v in il.Body.Variables)
            {
                Plugin.LogDebug($"- Index {v.Index}: {v.VariableType.FullName}");
            }

            byte dataPackageVarIndex = (byte)6; // Default value
            bool foundDataPackage = false;

            foreach (var v in il.Body.Variables)
            {
                if (v.VariableType.FullName.Contains("SleepDeathScreenDataPackage"))
                {
                    dataPackageVarIndex = (byte)v.Index;
                    foundDataPackage = true;
                    Plugin.LogDebug($"Found data package at index: {dataPackageVarIndex}");
                    break;
                }
            }

            if (!foundDataPackage)
            {
                Plugin.LogDebug("Using default data package index: 6");
            }

            // Find the target instruction sequence
            bool foundInsertionPoint = cursor.TryGotoNext(MoveType.Before,
                x => x.MatchLdarg(1),
                x => x.MatchIsinst<Menu.KarmaLadderScreen>()
            );

            if (foundInsertionPoint)
            {
                Plugin.LogDebug($"Found insertion point at index: {cursor.Index}");

                ILLabel beforeKarmaLadderScreenIf = cursor.DefineLabel();
                cursor.MarkLabel(beforeKarmaLadderScreenIf);

                // If the current process is ShelterCraftScreen, return, skipping setting the game data for everything else.
                cursor.Emit(OpCodes.Ldarg_0);
                cursor.EmitDelegate((RainWorldGame rainWorldGame) =>
                {
                    return rainWorldGame.manager.currentMainLoop is ShelterCraftScreen shelterCraftScreen;
                });
                cursor.Emit(OpCodes.Brtrue, OpCodes.Ret);

                // 1. Type check for next process being shelterCraftScreen, if so save the data.
                cursor.Emit(OpCodes.Ldarg_1);
                cursor.Emit(OpCodes.Isinst, typeof(ShelterCraftScreen));
                cursor.Emit(OpCodes.Brfalse, beforeKarmaLadderScreenIf);

                cursor.Emit(OpCodes.Ldloc_S, dataPackageVarIndex);
                cursor.EmitDelegate((ShelterCraftScreen nextProcess,
                    KarmaLadderScreen.SleepDeathScreenDataPackage sleepDeathScreenDataPackage) =>
                {
                    nextProcess.GetDataFromGame(sleepDeathScreenDataPackage);
                });
            }
            else
            {
                Plugin.LogError("Failed to find insertion point! Available instructions:");
                while (cursor.TryGotoNext(MoveType.Before, x => true))
                {
                    Plugin.LogError($"IL_{cursor.Index:X4}: {cursor.Instrs[cursor.Index]}");
                }
            }
        }
        catch (Exception ex)
        {
            Plugin.LogError($"CRITICAL PATCH ERROR: {ex}");
            Plugin.LogError($"Cursor position: {cursor.Index}");
            Plugin.LogError($"Stack trace:\n{ex.StackTrace}");
            throw;
        }

        Plugin.LogDebug("=== Finished CommunicateWithUpcomingProcess patch ===");
    }
}