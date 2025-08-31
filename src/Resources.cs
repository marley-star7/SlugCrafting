namespace SlugCrafting;

public static class Resources
{
    public const string AtlasesDir = "atlases";

    internal static void LoadResources()
    {
        Futile.atlasManager.LoadAtlas(AtlasesDir + "/craftBodyModeRequirementSymbols");

        //-- ITEMS
        Futile.atlasManager.LoadAtlas(AtlasesDir + "/knife");
        Futile.atlasManager.LoadAtlas(AtlasesDir + "/boneKnife");
        Futile.atlasManager.LoadAtlas(AtlasesDir + "/lizardLeather");

        //-- LE SCUG
        Futile.atlasManager.LoadAtlas(AtlasesDir + "/ccgCrafterEye");

        //-- COSMETICS
        Futile.atlasManager.LoadAtlas(AtlasesDir + "/greenLizardShellHelmet");
        Futile.atlasManager.LoadAtlas(AtlasesDir + "/blueLizardShellHelmet");
    }
}
