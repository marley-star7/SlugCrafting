
namespace SlugCrafting.Items;

internal static class PhysicalObjectHooks
{
    internal static void PhysicalObject_Update(On.PhysicalObject.orig_Update orig, PhysicalObject self, bool eu)
    {
        orig(self, eu);
        self.BundledStickUpdate(eu);
    }
}
