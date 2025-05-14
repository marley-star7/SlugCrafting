using UnityEngine;
using RWCustom;

using SlugCrafting.Items;
using SlugCrafting.Scavenges;
using SlugCrafting.Crafts;

using MarMath;
using ImprovedInput;

namespace SlugCrafting;

public static partial class Hooks
{
    private static void PlayerGraphics_Update(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
    {
        orig(self);

        var playerCraftingData = self.player.GetCraftingData();
        playerCraftingData.currentHandAnimation.PlaySlugcatAnimationGraphics(self);
    }
}
