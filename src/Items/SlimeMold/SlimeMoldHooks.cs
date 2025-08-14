namespace SlugCrafting.Items;

public class SlimeMoldHooks
{
    internal static void SlimeMold_Update(On.SlimeMold.orig_Update orig, SlimeMold self, bool eu)
    {
        self.UpdateSetRotationForImpaledSpearStick(ref self.setRotation);

        orig(self, eu);
    }
}
