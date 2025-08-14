
namespace SlugCrafting;

internal static class PlayerHooks
{
    internal static void Creature_Violence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
    {
        //-- MS7: We create a class of the parameters so we can easily adjust sent data however the accessories please before it reaches the player.
        var violenceContext = new ViolenceContext(source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);

        if (self is Player player)
        {
            var playerCraftingData = player.GetPlayerCraftingData();

            for (int i = 0; i < playerCraftingData.accessories.Count; i++)
            {
                playerCraftingData.accessories[i].PreWearerViolence(violenceContext);
            }
        }

        orig(self, violenceContext.source, violenceContext.directionAndMomentum, violenceContext.hitChunk, violenceContext.hitAppendage, violenceContext.type, violenceContext.damage, violenceContext.stunBonus);
    }

    internal static void Player_Grabbed(On.Player.orig_Grabbed orig, Player self, Creature.Grasp grasp)
    {
        orig(self, grasp);

        for (int i = 0; i < self.GetPlayerCraftingData().accessories.Count; i++)
        {
            self.GetPlayerCraftingData().accessories[i].OnWearerGrabbed(grasp);
        }
    }

    internal static bool Player_HeavyCarry(On.Player.orig_HeavyCarry orig, Player self, PhysicalObject obj)
    {
        if (obj is LizardShell && self.privSneak > 0.5f)
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
            self.GetPlayerCraftingData().accessories[i].OnWearerTerrainImpact(self, chunkIndex, direction, speed, firstContact);
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
        // TODO: this,

        // --- Scavenge Inputs Stuff ---
        if (player.JustPressed(Inputs.Scavenge))
            player.OnInputScavengeJustPressed();
        else if (player.IsPressed(Inputs.Scavenge))
            player.WhileInputScavengePressed();
        else if (player.JustReleased(Inputs.Scavenge))
            player.OnInputScavengeJustReleased();

        if (player.CanPhysicalCraft())
        {
            // --- Crafts Inputs Stuff ---
            if (player.JustPressed(Inputs.Craft))
                player.OnInputCraftJustPressed();
            else if (player.IsPressed(Inputs.Craft))
                player.WhileInputCraftPressed();
            else if (player.JustReleased(Inputs.Craft))
                player.OnInputCraftJustReleased();
        }

        if (player.IsPressed(Inputs.AlternateUse))
            player.WhileInputAlternateUsePressed();
        else if (player.JustReleased(Inputs.AlternateUse))
            player.OnInputAlternateUseJustReleased();

        orig(player, eu);
    }

    //
    // MREvents
    //

    internal static void OnPlayerSwitchGrasp(Player player, int graspFrom, int graspTo)
    {
        // Cancel the physical craft if we have one.
        player.CancelPhysicalCraft();
        player.CheckGraspsForPossiblePhysicalCraft();
    }

    internal static void OnPlayerReleaseGrasp(this Player player, int grasp)
    {
        var playerSlugCraftingData = player.GetPlayerCraftingData();

        if (grasp <= 1) // Only check for the first two grasps for a release, if so there is obviously no possible craft currently.
            playerSlugCraftingData.currentPossibleCraft = null;

        // Reset the scavenge data if we released the grasp.
        if (grasp == playerSlugCraftingData.creatureGraspUsed)
        {
            playerSlugCraftingData.creatureGraspUsed = -1;
            playerSlugCraftingData.currentTargetedScavenge = null;
        }
        else if (grasp == playerSlugCraftingData.knifeGraspUsed)
        {
            playerSlugCraftingData.knifeGraspUsed = -1;
        }

        // Update the current possible crafts
        playerSlugCraftingData.currentPossibleCraft = player.GetGraspsPhysicalCraft();
    }

    internal static void OnPlayerGrab(Player player, PhysicalObject grabbedObj, int graspUsed, int chunkGrabbed, Creature.Grasp.Shareability shareability, float dominance, bool overrideEquallyDominant, bool pacifying)
    {
        var playerSlugCraftingData = player.GetPlayerCraftingData();

        CheckGraspsForScavengeKnifeOrCreature(graspUsed);

        // If the other hand is not empty, check for possible craft.
        if (player.grasps[PlayerCraftingExtensions.GetOtherGrasp(graspUsed)] != null)
            playerSlugCraftingData.currentPossibleCraft = player.GetGraspsPhysicalCraft();

        //
        // SCAVENGING ITEMS CHECK
        //

        void CheckGraspsForScavengeKnifeOrCreature(in int graspNum)
        {
            var grasp = player.grasps[graspNum];
            if (grasp == null || grasp.grabbedChunk == null)
                return;

            // CREATURE CHECKING
            // Grabbed chunk takes priority first, because can be shared with item and creature.
            if (grasp.grabbedChunk.owner is Creature)
            {
                playerSlugCraftingData.creatureGraspUsed = graspNum;

                // Get the first available scavenge spot from the grabbed chunk as the currently targeted scavenge.
                var creatureScavengeData = ((Creature)player.grasps[playerSlugCraftingData.creatureGraspUsed].grabbed).GetScavengeData();
                if (creatureScavengeData != null)
                {
                    var scavengeSpot = new ScavengeSpot(player.grasps[graspNum].grabbedChunk.index, 0, 0);
                    var scavenge = creatureScavengeData.GetScavenge(scavengeSpot);

                    // If the grabbed scavenging spot or already scavenged, then search for one that isn't.
                    if (scavengeSpot == null || scavenge.canScavenge == false)
                        creatureScavengeData.GetNearestValidScavenge(scavengeSpot);

                    // DISABLED
                    // If we found a new valid scavenge spot diff from orig, grab that one's chunk instead.
                    // self.grasps[graspedPhysicalObjectGraspIndex].chunkGrabbed = scavengeSpot.bodyChunkIndex;
                    //

                    playerSlugCraftingData.currentTargetedScavenge = scavenge;
                }
            }

            // ITEM CHECKING
            // Grabbed knife overrides grabbed chunk if detected then.
            if (grasp.grabbed != null && grasp.grabbed is Knife)
                playerSlugCraftingData.knifeGraspUsed = graspNum;
        }
    }

    internal static bool Player_CanIPickThisUp(On.Player.orig_CanIPickThisUp orig, Player self, PhysicalObject obj)
    {
        return orig(self, obj);
    }

    //
    //
    //
}

