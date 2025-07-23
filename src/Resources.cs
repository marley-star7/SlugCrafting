namespace SlugCrafting;

public static class Resources
{
    internal static void LoadResources()
    {
        //-- ITEMS
        Futile.atlasManager.LoadAtlas("atlases/knife");
        Futile.atlasManager.LoadAtlas("atlases/boneKnife");
        Futile.atlasManager.LoadAtlas("atlases/lizardLeather");

        //-- LE SCUG
        Futile.atlasManager.LoadAtlas("atlases/ccgCrafterEye");

        //-- COSMETICS
        Futile.atlasManager.LoadAtlas("atlases/greenLizardShellHelmet");
    }
}
