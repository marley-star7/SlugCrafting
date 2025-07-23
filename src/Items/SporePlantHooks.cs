namespace SlugCrafting;

public static class SporePlantHooks
{
    internal static void SporePlant_Collide(On.SporePlant.orig_Collide orig, SporePlant self, PhysicalObject otherObject, int myChunk, int otherChunk)
    {
        //if (otherObject != self.GetSlugCraftingData().stuckInSpear)
            orig(self, otherObject, myChunk, otherChunk);
    }

    internal static void SporePlant_Update(On.SporePlant.orig_Update orig, SporePlant self, bool eu)
    {
        orig(self, eu);

        // Make sure player can't interact with the spore plant.
        var sporePlantData = self.GetSporePlantCraftingData();
        if (sporePlantData.stuckInSpear == null)
            return;

        /*
        self.forbiddenToPlayer = 10;
        self.CollideWithObjects = false;
        self.CollideWithTerrain = false;

        var spear = sporePlantData.stuckInSpear;

        if (spear.stuckInObject != null || spear.stuckInWall != null)
        {
            self.collisionLayer = self.DefaultCollLayer;
            self.Pacified = false;
        }
        else
        {
            // Make sure the spore plant does not collide with the spear.
            self.collisionLayer = 0;
            self.Pacified = true;
        }
        */
    }

    internal static void SporePlant_DrawSprites(On.SporePlant.orig_DrawSprites orig, SporePlant self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        orig(self, sLeaser, rCam, timeStacker, camPos);
        /*
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            var sprite = sLeaser.sprites[i];
            sprite.sortZ -= 100;
        }
        */
    }
}
