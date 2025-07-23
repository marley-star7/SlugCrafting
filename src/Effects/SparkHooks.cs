using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugCrafting.Effects;

public static class SparkHooks
{
    //-- MR7: Base game sparks REQUIRE lizard graphics be sent as a parameter to get the lizard graphics spark effects,
    // Well NAH, I'm changing it so that you don't. Just by mimicking the lizardGraphics source stuff but replacing it with my own.

    //-- MR7: Well NAH, turns out I actually don't need this stuff at all lol, woops.

    internal static void Spark_Update(On.Spark.orig_Update orig, Spark self, bool eu)
    {
        var selfCraftingData = self.GetSparkCraftingData();
        if (selfCraftingData.lizardShellColorGraphics != null && selfCraftingData.lizardShellColorGraphics.whiteFlicker < 5)
        {
            self.life -= 0.2f;
        }

        orig(self, eu);
    }

    internal static void Spark_DrawSprites(On.Spark.orig_DrawSprites orig, Spark self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        var selfCraftingData = self.GetSparkCraftingData();
        if (selfCraftingData.lizardShellColorGraphics != null && selfCraftingData.lizardShellColorGraphics.whiteFlicker > 0)
        {
            sLeaser.sprites[0].color = selfCraftingData.lizardShellColorGraphics.ShellColor(1, 1);
        }

        orig(self, sLeaser, rCam, timeStacker, camPos);
    }
}
