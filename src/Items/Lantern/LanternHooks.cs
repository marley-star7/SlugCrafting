namespace SlugCrafting.Items;

public class LanternHooks
{
    internal static void Lantern_Update(On.Lantern.orig_Update orig, Lantern self, bool eu)
    {
        self.UpdateSetRotationForImpaledSpearStick(ref self.setRotation);
        self.setRotation = -self.setRotation; // Ms7: It looks better flipped.

        orig(self, eu);
    }
}
