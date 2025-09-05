namespace SlugCrafting;

//-- MS7: I would move this somewhere else if I could think of a better spot.
public static partial class Content
{
    //
    //-- MRANIMATIONS
    //

    //
    //-- FISOBS
    //

    internal static void RegisterSlugCraftingFisobs()
    {
        //-- Ms7: Wrapped in try-catch because fisobs registry CAN cause errorless problems that will drive you crazy, and waste hours of your life.
        try
        {
            Fisobs.Core.Content.Register(new KnifeFisob());

            Fisobs.Core.Content.Register(new SpiderSilkStringFisob());
            Fisobs.Core.Content.Register(new LanternMouseStringFisob());
            Fisobs.Core.Content.Register(new CordFisob());

            Fisobs.Core.Content.Register(new LizardHideFisob());

            Fisobs.Core.Content.Register(new GreenLizardHeadShellFisob());
            Fisobs.Core.Content.Register(new PinkLizardHeadShellFisob());

            Fisobs.Core.Content.Register(new LizardHideBackpackFisob());

            Fisobs.Core.Content.Register(new GreenLizardShellHelmetFisob());
            Fisobs.Core.Content.Register(new GreenLizardShellCuirassFisob());

            Fisobs.Core.Content.Register(new BlueLizardShellHelmetFisob());
        }
        catch (Exception e)
        {
            Plugin.LogGameError(e.Message + e.StackTrace);
        }

        //Fisobs.Core.Content.Register(new DoubleSidedSpearFisob());
    }

    //
    //-- BUNDLES
    //

    internal static void RegisterSlugCraftingItemBundlesProperties()
    {
        Content.RegisterItemBundleProperties(
            Enums.AbstractObjectType.LanternMouseString,
            new ItemBundleProperties
            (
                3
            )
        );
    }
}