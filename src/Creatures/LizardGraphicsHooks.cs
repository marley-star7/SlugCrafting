using System;
using UnityEngine;
using SlugCrafting.Creatures;

namespace SlugCrafting;

public static partial class Hooks
{
    // Add hooks
    private static void ApplyLizardGraphicsHooks()
    {
        On.LizardGraphics.InitiateSprites += Lizard_InitiateSprites;
    }

    // Add hooks
    private static void RemoveLizardGraphicsHooks()
    {
        On.LizardGraphics.InitiateSprites -= Lizard_InitiateSprites;
    }

    private static void Lizard_InitiateSprites(On.LizardGraphics.orig_InitiateSprites orig, LizardGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);
        // Save on iniate of sprites the sprite leaser for the lizard.
        self.GetLizardGraphicsCraftingData().spriteLeaser = sLeaser;
    }
}