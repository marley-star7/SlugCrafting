using MonoMod.Cil;

namespace SlugCrafting;

internal static class PlayerHooks
{
	internal static void ApplyHooks()
	{
		On.Player.Update += PlayerHooks.Player_Update;
		On.Player.MovementUpdate += PlayerHooks.Player_MovementUpdate;
		On.Player.GrabUpdate += PlayerHooks.Player_GrabUpdate;
		On.Player.EatMeatUpdate += PlayerHooks.Player_EatMeatUpdate;
		On.Player.MaulingUpdate += PlayerHooks.Player_MaulingUpdate;

        IL.Player.UpdateBodyMode += PlayerHooks.Player_UpdateBodyMode;

		On.Player.CanIPickThisUp += PlayerHooks.Player_CanIPickThisUp;
		On.Player.Grabbed += PlayerHooks.Player_Grabbed;
		On.Player.HeavyCarry += PlayerHooks.Player_HeavyCarry;
		On.Player.TerrainImpact += PlayerHooks.Player_TerrainImpact;

		On.Player.SetMalnourished += PlayerHooks.Player_SetMalnourished;

		On.Creature.Violence += PlayerHooks.Creature_Violence;

		MREvents.OnPlayerGrab += PlayerHooks.OnPlayerGrab;
		MREvents.OnPlayerReleaseGrasp += PlayerHooks.OnPlayerReleaseGrasp;
		MREvents.OnPlayerSwitchGrasp += PlayerHooks.OnPlayerSwitchGrasp;
	}

    private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        throw new NotImplementedException();
    }

    internal static void RemoveHooks()
	{
		On.Player.Update -= PlayerHooks.Player_Update;
		On.Player.GrabUpdate -= PlayerHooks.Player_GrabUpdate;
		On.Player.MovementUpdate -= PlayerHooks.Player_MovementUpdate;
		On.Player.EatMeatUpdate -= PlayerHooks.Player_EatMeatUpdate;
		On.Player.MaulingUpdate -= PlayerHooks.Player_MaulingUpdate;

		IL.Player.UpdateBodyMode -= PlayerHooks.Player_UpdateBodyMode;

		On.Player.CanIPickThisUp -= PlayerHooks.Player_CanIPickThisUp;
		On.Player.Grabbed -= PlayerHooks.Player_Grabbed;
		On.Player.HeavyCarry -= PlayerHooks.Player_HeavyCarry;
		On.Player.TerrainImpact -= PlayerHooks.Player_TerrainImpact;

		On.Player.SetMalnourished -= PlayerHooks.Player_SetMalnourished;

		On.Creature.Violence -= PlayerHooks.Creature_Violence;

		MREvents.OnPlayerGrab -= PlayerHooks.OnPlayerGrab;
		MREvents.OnPlayerReleaseGrasp -= PlayerHooks.OnPlayerReleaseGrasp;
		MREvents.OnPlayerSwitchGrasp -= PlayerHooks.OnPlayerSwitchGrasp;
	}

    private static void Player_UpdateBodyMode(ILContext il)
    {
        /*
        try
        {
            ILCursor cursor = new ILCursor(il);
            cursor.Index = 0;

            ILLabel climbBeamSkipLabel = null;

            // More specific pattern matching to target the exact location
            if (cursor.TryGotoNext(MoveType.Before,
                // x => x.MatchLdarg(0), // We steal this ldarg for ourself
                x => x.MatchCall<Creature>("get_mainBodyChunk"),
                x => x.MatchLdfld<BodyChunk>("pos"),
                x => x.MatchCallvirt<Room>("GetTile"),
                x => x.MatchLdfld<Room.Tile>("verticalBeam"),
                x => x.MatchBrfalse(out _),
				x => x.MatchLdsfld<ModManager>("MSC"),
				x => x.MatchBrfalse(out _),
				x => x.MatchLdarg(0),
				x => x.MatchLdfld<Player>("monkAscension"),
				x => x.MatchBrtrue(out climbBeamSkipLabel) // This is the branch to IL_2DF4
				)
			)
            {

                // Insert our custom condition check right before the monkAscension check
                cursor.EmitDelegate((Player player) =>
                {
                    // Do not climb poles if we are crafting.
                    return player.GetPlayerCraftingData().isHandCrafting;
                });

                // Branch to the original failure case if our condition fails
                cursor.Emit(OpCodes.Brtrue, climbBeamSkipLabel);
				cursor.Emit(OpCodes.Ldarg_0);
			}
            else
            {
                Plugin.LogGameError("Error in IL Hook for Player_UpdateBodyMode:" + il);
			}
        }
        catch (Exception e)
        {
            Plugin.LogGameError("Error in Player_UpdateBodyMode: " + e.Message + e.StackTrace);
		}
        */
	}

    internal static void Creature_Violence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
    {
        //-- MS7: We create a class of the parameters so we can easily adjust sent data however the accessories please before it reaches the player.
        var violenceContext = new ViolenceContext(source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);

        if (self is Player player)
        {
            var playerCraftingData = player.GetPlayerCraftingData();

            for (int i = 0; i < playerCraftingData.accessories.Count; i++)
            {
                if (playerCraftingData.accessories[i].TryGetModule<ArmorAccessoryModule>(out var armorAccessoryModule))
                {
                    armorAccessoryModule.PreWearerViolence(violenceContext);
                }
            }
        }

        orig(self, violenceContext.source, violenceContext.directionAndMomentum, violenceContext.hitChunk, violenceContext.hitAppendage, violenceContext.type, violenceContext.damage, violenceContext.stunBonus);
    }

    internal static void Player_Grabbed(On.Player.orig_Grabbed orig, Player self, Creature.Grasp grasp)
    {
        orig(self, grasp);

        for (int i = 0; i < self.GetPlayerCraftingData().accessories.Count; i++)
        {
            self.GetPlayerCraftingData().accessories[i].PostWearerGrabbed(grasp);
        }
    }

    internal static bool Player_HeavyCarry(On.Player.orig_HeavyCarry orig, Player self, PhysicalObject obj)
    {
        if (obj is LizardHeadShell && self.privSneak > 0.5f)
        {
            return false;
        }

        return orig(self, obj);
    }

    internal static void Player_TerrainImpact(On.Player.orig_TerrainImpact orig, Player self, int chunkIndex, IntVector2 direction, float speed, bool firstContact)
    {
        orig(self, chunkIndex, direction, speed, firstContact);

        for (int i = 0; i < self.GetPlayerCraftingData().accessories.Count; i++)
        {
            self.GetPlayerCraftingData().accessories[i].PostWearerTerrainImpact(self, chunkIndex, direction, speed, firstContact);
        }
    }

    // TODO: add different scavenging times saved to the items scavenge data type thingy when you add it.
    private static bool MaulAllowed(Player scug)
    {
        PlayerCraftingData playerCraftingData = scug.GetPlayerCraftingData();

        if (playerCraftingData.craftTimer == 0)
            return true;
        
        return false;
    }

    /// <summary>
    /// Internal so locked off from mis-use, use extension method UpdateMass instead.
    /// Used only internally within SetMalnourished for now.
    /// </summary>
    /// <param name="self"></param>
    private static void ApplyAccessoryMass(Player self)
    {
        var selfCraftingData = self.GetPlayerCraftingData();
        float[] bodyChunkAddedMass = new float[]
        {
            0f,
            0f,
        };

        for (int i = 0; i < selfCraftingData.accessories.Count; i++)
        {
            var currentAccessory = selfCraftingData.accessories[i];
            bodyChunkAddedMass[currentAccessory.wearingBodyChunkIndex] += currentAccessory.mass;
        }

        self.bodyChunks[0].mass += bodyChunkAddedMass[0];
        self.bodyChunks[1].mass += bodyChunkAddedMass[1];
    }

    internal static void Player_SetMalnourished(On.Player.orig_SetMalnourished orig, Player self, bool m)
    {
        orig(self, m);
        ApplyAccessoryMass(self);
    }

    internal static void Player_MovementUpdate(On.Player.orig_MovementUpdate orig, Player self, bool eu)
    {
        var selfCraftingData = self.GetPlayerCraftingData();
        orig(self, eu);
    }

    internal static void Player_EatMeatUpdate(On.Player.orig_EatMeatUpdate orig, Player self, int graspIndex)
    {
        if (MaulAllowed(self))
            orig(self, graspIndex);
    }

    // Not sure why there is a difference between EatMeatUpdate and MaulingUpdate, nor do I know if this does anything, but just to be safe?
    internal static void Player_MaulingUpdate(On.Player.orig_MaulingUpdate orig, Player self, int graspIndex)
    {
        if (MaulAllowed(self))
            orig(self, graspIndex);
    }

    internal static void Player_GrabUpdate(On.Player.orig_GrabUpdate orig, Player selfPlayer, bool eu)
    {
        //-- MS7: TODO: make slugcat hands have a different custom animation when holding alternate use, to indicate it.
        // Probably just holding both hands higher up,

        //-- MS7: Do not pickup items if holding alt use, since it could interfere with keybinds.
        if (!selfPlayer.IsPressed(Inputs.AlternateUse))
            orig(selfPlayer, eu);
    }

    // Not sure why there is a difference between EatMeatUpdate and MaulingUpdate, nor do I know if this does anything, but just to be safe?
    internal static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
    {
        var sCData = player.GetPlayerCraftingData();

        if (sCData.craftTimer < 0)
        {
            sCData.craftTimer++;
        }

        // --- Bundle Inputs Stuff ---

        if (player.CanHandCraft())
        {
            // --- Crafts Inputs Stuff ---
            if (player.IsPressed(Inputs.Craft) && player.IsPressed(PlayerKeybind.Up))
            {
                if (player.JustPressed(Inputs.Craft) || player.JustPressed(PlayerKeybind.Up))
                {
                    player.CraftInputStart();
                }
                else
                {
                    player.CraftInputUpdate();
                }
            }
			else if (player.JustReleased(Inputs.Craft) || player.JustReleased(PlayerKeybind.Up))
			{
				player.CraftInputRelease();
			}
		}

        if (player.IsPressed(Inputs.AlternateUse))
            player.WhileInputAlternateUsePressed();
        else if (player.JustReleased(Inputs.AlternateUse))
            player.OnInputAlternateUseJustReleased();

        orig(player, eu);

        /*
        if (player.vinePos != null && player.vinePos.vine is PoleMimic poleMimic)
        {
            var appendagePos = new PhysicalObject.Appendage.Pos(poleMimic.appendages[poleMimic.appendages.Count - 1], 1, 1);
            poleMimic.SeverAndCordify(appendagePos);
        }
        */
    }

    //
    // MREvents
    //

    internal static void OnPlayerSwitchGrasp(Player player, int graspFrom, int graspTo)
    {
        // Cancel the physical craft if we have one.
        player.CancelPhysicalCraft();
        player.CheckGraspsForPossibleHandCraft();
    }

    internal static void OnPlayerReleaseGrasp(this Player player, int grasp)
    {
        var playerSlugCraftingData = player.GetPlayerCraftingData();

        // Update the current possible crafts
        player.CheckGraspsForPossibleHandCraft();
    }

    internal static void OnPlayerGrab(Player player, PhysicalObject grabbedObj, int graspUsed, int chunkGrabbed, Creature.Grasp.Shareability shareability, float dominance, bool overrideEquallyDominant, bool pacifying)
    {
        player.CheckGraspsForPossibleHandCraft();
    }

    internal static bool Player_CanIPickThisUp(On.Player.orig_CanIPickThisUp orig, Player self, PhysicalObject obj)
    {
        return orig(self, obj);
    }

    //
    //
    //
}

