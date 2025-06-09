using RWCustom;
using UnityEngine;

using MRCustom.Animations;
using MRCustom.Math;

using ImprovedInput;

using SlugCrafting.Items;

namespace SlugCrafting;

public static partial class Hooks
{
    // Add hooks
    private static void ApplyPlayerHooks()
    {
        On.Player.Update += Player_Update;
        On.Player.GrabUpdate += Player_GrabUpdate;
        On.Player.EatMeatUpdate += Player_EatMeatUpdate;
        On.Player.MaulingUpdate += Player_MaulingUpdate;
    }

    // Remove hooks
    private static void RemovePlayerHooks()
    {
        On.Player.Update -= Player_Update;
        On.Player.GrabUpdate -= Player_GrabUpdate;
        On.Player.EatMeatUpdate -= Player_EatMeatUpdate;
        On.Player.MaulingUpdate -= Player_MaulingUpdate;
    }

    // TODO: add different scavenging times saved to the items scavenge data type thingy when you add it.
    private static bool MaulAllowed(Player scug)
    {
        PlayerCraftingData playerCraftingData = scug.GetPlayerCraftingData();

        if (playerCraftingData.craftTimer == 0)
            return true;
        
        return false;
    }

    private static void Player_EatMeatUpdate(On.Player.orig_EatMeatUpdate orig, Player self, int graspIndex)
    {
        if (MaulAllowed(self))
            orig(self, graspIndex);
    }

    // Not sure why there is a difference between EatMeatUpdate and MaulingUpdate, nor do I know if this does anything, but just to be safe?
    private static void Player_MaulingUpdate(On.Player.orig_MaulingUpdate orig, Player self, int graspIndex)
    {
        if (MaulAllowed(self))
            orig(self, graspIndex);
    }

    private static void Player_GrabUpdate(On.Player.orig_GrabUpdate orig, Player selfPlayer, bool eu)
    {
        if (selfPlayer.IsCrafter() && selfPlayer.IsPressed(ImprovedInput.PlayerKeybind.Special))
        {

        }
        else
            orig(selfPlayer, eu);
    }

    // Not sure why there is a difference between EatMeatUpdate and MaulingUpdate, nor do I know if this does anything, but just to be safe?
    private static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
    {
        PlayerExtension.ScavengeUpdate(player);
        PlayerExtension.PhysicalCraftUpdate(player);

        orig(player, eu);
    }
}

