using RWCustom;
using UnityEngine;

using SlugCrafting.Items;
using MRCustom.Animations;
using MRCustom.Math;
using IL.JollyCoop;

namespace SlugCrafting;

public static partial class Hooks
{
    //-- Add hooks
    internal static void ApplyPlayerGraphicsHooks()
    {
        On.PlayerGraphics.Update += PlayerGraphics_Update;
        On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
        On.PlayerGraphics.ApplyPalette += PlayerGraphics_ApplyPalette;
    }

    //-- Remove hooks
    internal static void RemovePlayerGraphicsHooks()
    {
        On.PlayerGraphics.Update -= PlayerGraphics_Update;
        On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
        On.PlayerGraphics.ApplyPalette -= PlayerGraphics_ApplyPalette;
    }

    private static void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);

        if (!self.player.IsCrafter())
            return;

        // TODO: when idle for a bit, he will close his eyes in peace.

        //-- Replace the face sprite with the crafter face sprite.
        var faceSprite = sLeaser.sprites[9];
        if (faceSprite.element != null && !faceSprite.element.name.StartsWith("crafter"))
        {
            string oldName = faceSprite.element.name;
            faceSprite.element = Futile.atlasManager.GetElementWithName("crafter" + oldName);
        }

    }

    private static void PlayerGraphics_Update(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
    {
        orig(self);
    }

    private static void PlayerGraphics_ApplyPalette(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(self, sLeaser, rCam, palette);

        if (!self.player.IsCrafter())
            return;

        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
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

            sLeaser.sprites[i].color = idealGray;
        }
    }
}
