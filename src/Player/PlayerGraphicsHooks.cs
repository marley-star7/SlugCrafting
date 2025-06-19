using RWCustom;
using UnityEngine;

using MRCustom.Animations;
using MRCustom.Math;

using SlugCrafting.Items;
using SlugCrafting.Accessories;

using CompartmentalizedCreatureGraphics.SlugcatCosmetics;
using CompartmentalizedCreatureGraphics;

namespace SlugCrafting;

public static partial class Hooks
{
    //-- Add hooks
    internal static void ApplyPlayerGraphicsHooks()
    {
        On.PlayerGraphics.Update += PlayerGraphics_Update;

        On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
        //On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
        On.PlayerGraphics.ApplyPalette += PlayerGraphics_ApplyPalette;
    }

    //-- Remove hooks
    internal static void RemovePlayerGraphicsHooks()
    {
        On.PlayerGraphics.Update -= PlayerGraphics_Update;

        On.PlayerGraphics.InitiateSprites -= PlayerGraphics_InitiateSprites;
        //On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
        On.PlayerGraphics.ApplyPalette -= PlayerGraphics_ApplyPalette;
    }

    /* 
    Sprite 0 = BodyA
    Sprite 1 = HipsA
    Sprite 2 = Tail
    Sprite 3 = HeadA || B
    Sprite 4 = LegsA
    Sprite 5 = Arm
    Sprite 6 = Arm
    Sprite 7 = TerrainHand
    sprite 8 = TerrainHand
    sprite 9 = FaceA
    sprite 10 = Futile_White with shader Flatlight
    sprite 11 = pixel Mark of comunication
    */

    private static void PlayerGraphics_Update(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
    {
        orig(self);
    }

    //
    // IDRAWABLE
    //

    private static void AddCrafterDynamicCosmetics(PlayerGraphics playerGraphics)
    {
        var playerGraphicsCCGData = playerGraphics.GetPlayerGraphicsCCGData();

        //
        // BUILD THE COMPARTMENTALIZED SCUG
        //

        PlayerGraphicsCCGData.AddDefaultVanillaSlugcatDynamicLeftEarCosmetic(playerGraphics);
        PlayerGraphicsCCGData.AddDefaultVanillaSlugcatDynamicRightEarCosmetic(playerGraphics);

        // LEFT EYE
        playerGraphics.AddDynamicCosmetic(new DynamicSlugcatEye()
        {
            spriteName = "ccgCrafterEye",
            defaultAnglePositions = PlayerGraphicsCCGData.DefaultVanillaLeftEyeAnglePositions,
            side = -1,
            defaultScaleX = -1,
            snapValue = PlayerGraphicsCCGData.DefaultVanillaFaceSnapValue,
        }
        );

        // RIGHT EYE
        playerGraphics.AddDynamicCosmetic(new DynamicSlugcatEye()
        {
            spriteName = "ccgCrafterEye",
            defaultAnglePositions = PlayerGraphicsCCGData.DefaultVanillaRightEyeAnglePositions,
            side = 1,
            defaultScaleX = 1,
            snapValue = PlayerGraphicsCCGData.DefaultVanillaFaceSnapValue,
        }
        );

        PlayerGraphicsCCGData.AddDefaultVanillaSlugcatDynamicNoseCosmetic(playerGraphics);
    }

    private static void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics playerGraphics, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        var playerGraphicsCCGData = playerGraphics.GetPlayerGraphicsCCGData();

        if (playerGraphics.player.IsCrafter())
        {
            playerGraphicsCCGData.compartmentalizedGraphicsEnabled = true; // Crafter always requires it, since thats how armors work.
            playerGraphicsCCGData.onInitiateSpritesDynamicCosmeticsToAdd = AddCrafterDynamicCosmetics;
        }

        orig(playerGraphics, sLeaser, rCam);
    }

    private static void PlayerGraphics_ApplyPalette(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics playerGraphics, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(playerGraphics, sLeaser, rCam, palette);

        if (!playerGraphics.player.IsCrafter())
            return;

        //-- MR7: It's barely noticable, but basing the color off the room pallete makes it look a bit better.
        // There is also potential issue that comes with a gray scug that depending on the room palette, especially fog color, they can become almost impossible to see.
        // Tried making some code for this to find an optimal gray based both off the room palette, and fog color, for max readability.
        // (and to help the colorblind folks out)

        float idealLerpRatio = 0.6f;
        float visibilityLerpRatioModifierFullStrength = 0.13f;

        Color roomBlackColor = palette.GetColor(RoomPalette.ColorName.BlackColor);
        Color roomFogColor = palette.GetColor(RoomPalette.ColorName.FogColor);

        Color idealGray = Color.Lerp(roomBlackColor, Color.white, idealLerpRatio);

        float adjustmentIfTooCloseRatio;
        if (idealGray.grayscale < roomFogColor.grayscale)
        {
            //Plugin.Logger.LogDebug("Ideal gray is darker than room fog color gray, lightening the gray even further.");
            float inverseLerp = Mathf.InverseLerp(0, roomFogColor.grayscale, idealGray.grayscale);
            adjustmentIfTooCloseRatio = inverseLerp * visibilityLerpRatioModifierFullStrength * 1.5f; //- MR7: Multiply a bit more since we need to put in extra work to get out of the darker section.
            idealGray = Color.Lerp(roomBlackColor, Color.white, idealLerpRatio + adjustmentIfTooCloseRatio);
        }
        else
        {
            //Plugin.Logger.LogDebug("Ideal gray is lighter than room fog color gray, lighter the gray further.");
            float inverseLerp = Mathf.InverseLerp(roomFogColor.grayscale, Color.white.grayscale, idealGray.grayscale);
            adjustmentIfTooCloseRatio = (1 - inverseLerp) * visibilityLerpRatioModifierFullStrength; // 1 - 0 because the inverse lerp here is closest to ideal gray at 0
            idealGray = Color.Lerp(roomBlackColor, Color.white, idealLerpRatio + adjustmentIfTooCloseRatio);
        }

        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].color = idealGray;
        }
    }
}
