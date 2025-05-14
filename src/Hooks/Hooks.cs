using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Fisobs.Core;
using BepInEx;

namespace SlugCrafting;

public static partial class Hooks
{
    // Add hooks
    public static void ApplyHooks()
    {
        On.Player.GrabUpdate += Player_GrabUpdate;
        On.Player.Update += Player_Update;
        On.Player.EatMeatUpdate += Player_EatMeatUpdate;
        On.Player.MaulingUpdate += Player_MaulingUpdate;

        On.PlayerGraphics.Update += PlayerGraphics_Update;

        On.LizardGraphics.InitiateSprites += Lizard_InitiateSprites;
    }

    // Remove hooks
    public static void RemoveHooks()
    {
        On.Player.GrabUpdate -= Player_GrabUpdate;
        On.Player.Update -= Player_Update;
        On.Player.EatMeatUpdate -= Player_EatMeatUpdate;
        On.Player.MaulingUpdate -= Player_MaulingUpdate;

        On.PlayerGraphics.Update -= PlayerGraphics_Update;

        On.LizardGraphics.InitiateSprites -= Lizard_InitiateSprites;
    }
}