namespace SlugCrafting;

internal static class PlayerGraphicsHooks
{
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

    internal static void PlayerGraphics_Update(On.PlayerGraphics.orig_Update orig, PlayerGraphics self)
    {
        orig(self);
    }

    //
    // IDRAWABLE
    //

    internal static void PlayerGraphics_ctor(On.PlayerGraphics.orig_ctor orig, PlayerGraphics self, PhysicalObject ow)
    {
        orig(self, ow);
    }

    internal static void PlayerGraphics_ApplyPalette(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics playerGraphics, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig(playerGraphics, sLeaser, rCam, palette);

        if (!playerGraphics.player.IsCrafter())
            return;

        //-- MS7: It's barely noticable, but basing the color off the room pallete makes it look a bit better.
        // There is also potential issue that comes with a gray scug that depending on the room palette, especially fog color, they can become almost impossible to see.
        // Tried making some code for this to find an optimal gray based both off the room palette, and fog color, for max readability.
        // (and to help the colorblind folks out)

        Color roomBlackColor = palette.GetColor(RoomPalette.ColorName.BlackColor);
        Color roomFogColor = palette.GetColor(RoomPalette.ColorName.FogColor);
        Color roomSkyColor = palette.GetColor(RoomPalette.ColorName.SkyColor);

        float idealLerpRatio = 0.6f;
        Color idealGray = Color.Lerp(roomBlackColor, Color.white, idealLerpRatio);

        //-- Ms7: Checka and shift against both fog and sky color, since both can heavy heavy impact on readability.
        // Sky color comes first as it's more important, fog color has less impact, it tries to find a balance inbetween.

        idealGray = idealGray.ShiftToColorIfGrayscaleTooClose(roomBlackColor, roomSkyColor, 0.09f); // Shift a bit down again.
        idealGray = idealGray.ShiftToColorIfGrayscaleTooClose(Color.white, roomSkyColor, 0.16f);
        idealGray = idealGray.ShiftToColorIfGrayscaleTooClose(Color.white, roomFogColor, 0.09f);

        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].color = idealGray;
        }
    }
}
