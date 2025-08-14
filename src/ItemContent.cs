namespace SlugCrafting.Core;

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
        Fisobs.Core.Content.Register(new KnifeFisob());

        Fisobs.Core.Content.Register(new SpiderSilkStringFisob());
        Fisobs.Core.Content.Register(new LanternMouseStringFisob());
        Fisobs.Core.Content.Register(new CordFisob());

        Fisobs.Core.Content.Register(new LizardHideFisob());

        Fisobs.Core.Content.Register(new GreenLizardShellFisob());
        Fisobs.Core.Content.Register(new PinkLizardShellFisob());

        Fisobs.Core.Content.Register(new LizardHideBackpackFisob());

        Fisobs.Core.Content.Register(new GreenLizardShellHelmetFisob());
        Fisobs.Core.Content.Register(new BlueLizardShellHelmetFisob());

        //Fisobs.Core.Content.Register(new DoubleSidedSpearFisob());
    }

    //
    //-- BUNDLES
    //

    internal static void RegisterSlugCraftingItemBundlesProperties()
    {
        SlugCrafting.Core.Content.RegisterItemBundleProperties(
            SlugCraftingEnums.AbstractObjectType.LanternMouseString,
            new ItemBundleProperties
            (
                3
            )
        );
    }

    //
    //-- SCAVENGES
    //

    internal static void RegisterSlugCraftingScavenges()
    {
        //-- SCAVENGE DATA
        SlugCrafting.Core.Content.RegisterScavengeData(CreatureTemplate.Type.GreenLizard, typeof(GreenLizardScavengeData));
        SlugCrafting.Core.Content.RegisterScavengeData(CreatureTemplate.Type.PinkLizard, typeof(PinkLizardScavengeData));
        SlugCrafting.Core.Content.RegisterScavengeData(CreatureTemplate.Type.LanternMouse, typeof(LanternMouseScavengeData));
    }
}