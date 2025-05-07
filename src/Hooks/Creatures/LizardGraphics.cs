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
    // Save the sprite leaser for each lizard in the lizard's graphics,
    // So we can refrence later when stealing for scavenging their heads!
    public class LizardGraphicsData
    {
        public RoomCamera.SpriteLeaser spriteLeaser;
    }

    // Save on iniate of sprites the sprite leaser for the lizard.
    public static readonly ConditionalWeakTable<LizardGraphics, LizardGraphicsData> lizardGraphicsStorage = new ConditionalWeakTable<LizardGraphics, LizardGraphicsData>();
    private static void Lizard_InitiateSprites(On.LizardGraphics.orig_InitiateSprites orig, LizardGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);

        // Check if the lizard is already in the dictionary.
        if (lizardGraphicsStorage.TryGetValue(self, out LizardGraphicsData? lizardGraphicsData))
        {
            lizardGraphicsData.spriteLeaser = sLeaser;
        }
        else
        {
            lizardGraphicsStorage.Add(self, new LizardGraphicsData()
            {
                spriteLeaser = sLeaser,
            });
        }
    }
}