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
        // Put your custom hooks here!
        On.Player.GrabUpdate += Player_GrabUpdate;
        On.Player.Update += Player_Update;
        On.Player.EatMeatUpdate += Player_EatMeatUpdate;
        On.Player.MaulingUpdate += Player_MaulingUpdate;

        On.LizardGraphics.InitiateSprites += Lizard_InitiateSprites;
    }
    public static void RemoveHooks()
    {
        // Remove hooks
        On.Player.GrabUpdate -= Player_GrabUpdate;
        On.Player.Update -= Player_Update;
        On.Player.EatMeatUpdate -= Player_EatMeatUpdate;
        On.Player.MaulingUpdate -= Player_MaulingUpdate;

        On.LizardGraphics.InitiateSprites -= Lizard_InitiateSprites;
    }
}