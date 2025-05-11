using System;
using UnityEngine;
using RWCustom;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Menu;
using MonoMod.Cil;

using SlugCrafting.Items;
using ImprovedInput;
using System.Runtime.CompilerServices;

namespace SlugCrafting;

public static partial class Hooks
{
    
    private static void Lizard_InitiateSprites(On.LizardGraphics.orig_InitiateSprites orig, LizardGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);
        // Save on iniate of sprites the sprite leaser for the lizard.
        self.GetGraphicsData().spriteLeaser = sLeaser;
    }
}

/// <summary>
/// Saves mainly the sprite leaser for each lizard in the lizard's graphics,
/// So we can reference later when stealing their heads via scavenging!
/// </summary>
public class LizardGraphicsData
{
    public RoomCamera.SpriteLeaser spriteLeaser;
}

public static class LizardGraphicsExtension
{
    private static readonly ConditionalWeakTable<LizardGraphics, LizardGraphicsData> lizardsGraphicsStorage = new();

    public static LizardGraphicsData GetGraphicsData(this LizardGraphics lizardGraphics) => lizardsGraphicsStorage.GetValue(lizardGraphics, _ => new LizardGraphicsData());
}