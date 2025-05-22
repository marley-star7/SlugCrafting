using RWCustom;
using UnityEngine;

using SlugCrafting.Items;
using MRCustom.Animations;
using MRCustom.Math;

namespace SlugCrafting.Hooks;

public static partial class Hooks
{
    // Add hooks
    private static void ApplyPlayerGraphicsHooks()
    {
        On.PlayerGraphics.Update += PlayerGraphics_Update;
    }

    // Remove hooks
    private static void RemovePlayerGraphicsHooks()
    {
        On.PlayerGraphics.Update -= PlayerGraphics_Update;
    }

    private static void PlayerGraphics_Update(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
    {
        orig(self);

        var playerHandAnimationData = self.player.GetHandAnimationData();
        var playerCraftingData = self.player.GetCraftingData();

        if (playerCraftingData.craftTimer > 0 && playerCraftingData.currentPossibleCraft.Value.handAnimation != null)
        {
            playerCraftingData.currentPossibleCraft.Value.handAnimation.GraphicsUpdate(self);
        }
    }
}
