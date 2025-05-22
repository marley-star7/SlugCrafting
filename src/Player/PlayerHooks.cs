using RWCustom;
using UnityEngine;

using SlugCrafting.Items;
using MRCustom.Animations;
using MRCustom.Math;

namespace SlugCrafting.Hooks;

public static partial class Hooks
{
    // Add hooks
    private static void ApplyPlayerHooks()
    {
        On.Player.Update += Player_Update;
        On.Player.EatMeatUpdate += Player_EatMeatUpdate;
        On.Player.MaulingUpdate += Player_MaulingUpdate;
        //On.Player.NewRoom += Player_NewRoom;
    }

    // Remove hooks
    private static void RemovePlayerHooks()
    {
        On.Player.Update -= Player_Update;
        On.Player.EatMeatUpdate -= Player_EatMeatUpdate;
        On.Player.MaulingUpdate -= Player_MaulingUpdate;
        //On.Player.NewRoom -= Player_NewRoom;
    }

    /*
    public static void Player_NewRoom(On.Player.orig_NewRoom orig, Player self, Room newRoom)
    {
        orig(self, newRoom);

        self.GetHandAnimationData().AnimationFinished += self.GetCraftingData().OnPlayerHandAnimationFinished;
    }
    */

    // TODO: add different scavenging times saved to the items scavenge data type thingy when you add it.
    private static bool CanMaul(Player scug)
    {
        PlayerCraftingData playerCraftingData = scug.GetCraftingData();

        if (playerCraftingData.craftTimer == 0)
            return true;
        
        return false;
    }

    private static void Player_EatMeatUpdate(On.Player.orig_EatMeatUpdate orig, Player self, int graspIndex)
    {
        if (CanMaul(self))
            orig(self, graspIndex);
    }

    // Not sure why there is a difference between EatMeatUpdate and MaulingUpdate, nor do I know if this does anything, but just to be safe?
    private static void Player_MaulingUpdate(On.Player.orig_MaulingUpdate orig, Player self, int graspIndex)
    {
        if (CanMaul(self))
            orig(self, graspIndex);
    }

    // Not sure why there is a difference between EatMeatUpdate and MaulingUpdate, nor do I know if this does anything, but just to be safe?
    private static void Player_Update(On.Player.orig_Update orig, Player player, bool eu)
    {
        PlayerExtension.ScavengeUpdate(player);
        PlayerExtension.CraftUpdate(player);

        orig(player, eu);
    }
}

