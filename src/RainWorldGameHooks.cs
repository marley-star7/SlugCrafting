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

            ILLabel pastReturnStatement = cursor.DefineLabel();

            if (cursor.TryGotoNext(MoveType.After,
                x => x.MatchLdarg(1),
                x => x.MatchIsinst<Menu.EndCredits>(),
                x => x.MatchBrfalse(out ILLabel targetLabel)))
            {
                cursor.MarkLabel(pastReturnStatement);
            }

            cursor.Index = 0;

            if (cursor.TryGotoNext(MoveType.Before,
                x => x.MatchLdarg(1),
                x => x.MatchIsinst<Menu.KarmaLadderScreen>(),
                x => x.MatchBrtrue(out ILLabel targetLabel)))
            {
                cursor.Index += 1;

                cursor.Emit(OpCodes.Isinst, (typeof(ShelterCraftScreen)));
                cursor.Emit(OpCodes.Brtrue_S, pastReturnStatement);

                cursor.Emit(OpCodes.Ldarg_1); // Re-add the load arg we use for ours.
            }

            // Find the target instruction sequence
            if (cursor.TryGotoNext(MoveType.Before,
                x => x.MatchLdarg(1),
                x => x.MatchIsinst<Menu.KarmaLadderScreen>(),
                x => x.MatchBrfalse(out ILLabel targetLabel)))
            {
                Plugin.LogDebug($"Found insertion point at index: {cursor.Index}");

                ILLabel beforeKarmaLadderScreenIf = cursor.DefineLabel();

                cursor.Index += 1;

                cursor.EmitDelegate(() => Plugin.LogDebug("I'm so"));

                cursor.Emit(OpCodes.Isinst, typeof(ShelterCraftScreen));
                cursor.Emit(OpCodes.Brfalse_S, beforeKarmaLadderScreenIf);

                cursor.Emit(OpCodes.Ldarg_1);
                cursor.Emit(OpCodes.Isinst, typeof(ShelterCraftScreen));
                cursor.Emit(OpCodes.Ldloc_S, dataPackageVarIndex); // -- Ms7: DON'T FORGET THE _SSSSSSSSSSSSSSSSSSS AHHGGGGG
                cursor.Emit(OpCodes.Callvirt, typeof(ShelterCraftScreen).GetMethod(
                    "GetDataFromGame",
                    new[] { typeof(KarmaLadderScreen.SleepDeathScreenDataPackage) }
                ));

                cursor.MarkLabel(beforeKarmaLadderScreenIf);

                cursor.Emit(OpCodes.Ldarg_1); // Re-add load arg we stole.
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

        Plugin.LogDebug(il);
    }
}