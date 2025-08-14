namespace SlugCrafting;

public static class SporePlantHooks
{
    internal static void SporePlant_Collide(On.SporePlant.orig_Collide orig, SporePlant self, PhysicalObject otherObject, int myChunk, int otherChunk)
    {
        orig(self, otherObject, myChunk, otherChunk);
    }

    internal static void SporePlant_Update(On.SporePlant.orig_Update orig, SporePlant self, bool eu)
    {
        orig(self, eu);
    }

    internal static void SporePlant_DrawSprites(On.SporePlant.orig_DrawSprites orig, SporePlant self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
    }
}
