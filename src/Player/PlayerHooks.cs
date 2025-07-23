


using System.Xml.Serialization;

namespace SlugCrafting;

internal static class PlayerHooks
{
    internal static void Creature_Violence(On.Creature.orig_Violence orig, Creature self, BodyChunk source, Vector2? directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage, Creature.DamageType type, float damage, float stunBonus)
    {
        //-- MR7: We create a class of the parameters so we can easily adjust sent data however the accessories please before it reaches the player.
        var violenceContext = new ViolenceContext(source, directionAndMomentum, hitChunk, hitAppendage, type, damage, stunBonus);

        if (self is Player player)
        {
            var playerCraftingData = player.GetPlayerCraftingData();

            for (int i = 0; i < playerCraftingData.accessories.Count; i++)
            {
                playerCraftingData.accessories[i].OnWearerViolence(violenceContext);
            }
        }

        orig(self, source, violenceContext.directionAndMomentum, violenceContext.hitChunk, violenceContext.hitAppendage, violenceContext.type, violenceContext.damage, violenceContext.stunBonus);
    }

    internal static void Player_Grabbed(On.Player.orig_Grabbed orig, Player self, Creature.Grasp grasp)
    {
        orig(self, grasp);

        for (int i = 0; i < self.GetPlayerCraftingData().accessories.Count; i++)
        {
            self.GetPlayerCraftingData().accessories[i].OnWearerGrabbed(grasp);
        }
    }

    internal static void Player_TerrainImpact(On.Player.orig_TerrainImpact orig, Player self, int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        TerrainImpactContext impactContext = new TerrainImpactContext(chunk, direction, speed, firstContact);
        for (int i = 0; i < self.GetPlayerCraftingData().accessories.Count; i++)
        {
            self.GetPlayerCraftingData().accessories[i].OnWearerTerrainImpact(impactContext);
        }

        orig(self, impactContext.chunkIndex, impactContext.direction, impactContext.speed, impactContext.firstContact);
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

            if (currentAccessory.armorStats == null)
                continue; // This accessory does not modify player.

            bodyChunkAddedMass[currentAccessory.armorStats.wearingBodyChunkIndex] += currentAccessory.armorStats.mass;
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

        if (self.IsCrafter())
        {
            if (!SlugBase.Features.PlayerFeatures.WalkSpeedMul.TryGet(self, out var baseWalkSpeed))
                Plugin.Logger.LogError("Crafter character does not have a base walk speed defined in SlugBase, something has gone terribly wrong...");
            if (!SlugBase.Features.PlayerFeatures.ClimbSpeedMul.TryGet(self, out var basePoleClimbSpeed))
                Plugin.Logger.LogError("Crafter character does not have a base pole climb speed defined in SlugBase, something has gone terribly wrong...");
            if (!SlugBase.Features.PlayerFeatures.TunnelSpeedMul.TryGet(self, out var baseCooridoorClimbSpeed))
                Plugin.Logger.LogError("Crafter character does not have a base corridor climb speed defined in SlugBase, something has gone terribly wrong...");

            //-- MR7: TODO: This is probably innefficient to re-get every accessories modifications every update, but it works for now.
            var accessoryWalkSpeedFac = 1f;

            for (int i = 0; i < selfCraftingData.accessories.Count; i++)
            {
                var currentAccessory = selfCraftingData.accessories[i];

                if (currentAccessory.armorStats == null)
                    continue; // This accessory does not modify player.

                accessoryWalkSpeedFac *= currentAccessory.armorStats.runSpeedMultiplier;
            }

            self.slugcatStats.runspeedFac = baseWalkSpeed[0] * accessoryWalkSpeedFac;
        }
        
        /*
        if (self.rollCounter > 15)
            self.EndRoll();
        */

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
        //-- MR7: TODO: make slugcat hands have a different custom animation when holding alternate use, to indicate it.
        // Probably just holding both hands higher up,

        //-- MR7: Do not pickup items if holding alt use, since it could interfere with keybinds.
        if (selfPlayer.IsCrafter() && selfPlayer.IsPressed(Inputs.AlternateUse))
            selfPlayer.BundleGrabUpdate(eu);
        else
            orig(selfPlayer, eu);
    }

    // Not sure why there is a difference between EatMeatUpdate and MaulingUpdate, nor do I know if this does anything, but just to be safe?
    internal static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
    {
        PlayerExtension.ScavengeUpdate(player);
        PlayerExtension.PhysicalCraftUpdate(player);

        orig(player, eu);
    }
}

