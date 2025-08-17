namespace SlugCrafting;

public static partial class Hooks
{
    internal static void ApplyHooks()
    {
        ProcessManagerHooks.ApplyHooks();

        ApplyPlayerHooks();
        ApplyPlayerGraphicsHooks();
        ApplySlugcatHandHooks();

        ApplyRegionStateHooks();

        ApplyPhysicalObjectHooks();
        ApplyPlayerCarryableItemHooks();
        ApplyLanternHooks();
        ApplySlimeMoldHooks();

        ApplyWeaponHooks();
        ApplySpearHooks();
        ApplySporePlantHooks();

        //ApplySparkHooks(); //-- MS7: Uneeded, disabled

        WeaponsExtInit.WeaponsExtInit.Init();
    }

    internal static void RemoveHooks()
    {
        ProcessManagerHooks.RemoveHooks();

        On.RainWorld.PostModsInit -= Plugin.RainWorld_PostModsInit;

        RemovePlayerHooks();
        RemovePlayerGraphicsHooks();
        RemoveSlugcatHandHooks();

        RemoveRegionStateHooks();

        RemovePhysicalObjectHooks();
        RemovePlayerCarryableItemHooks();
        RemoveLanternHooks();
        RemoveSlimeMoldHooks();

        RemoveWeaponHooks();
        RemoveSpearHooks();
        RemoveSporePlantHooks();

        //RemoveSparkHooks();
    }

    // Player

    private static void ApplyPlayerHooks()
    {
        On.Player.Update += PlayerHooks.Player_Update;
        On.Player.MovementUpdate += PlayerHooks.Player_MovementUpdate;
        On.Player.GrabUpdate += PlayerHooks.Player_GrabUpdate;
        On.Player.EatMeatUpdate += PlayerHooks.Player_EatMeatUpdate;
        On.Player.MaulingUpdate += PlayerHooks.Player_MaulingUpdate;

        On.Player.CanIPickThisUp += PlayerHooks.Player_CanIPickThisUp;
        On.Player.Grabbed += PlayerHooks.Player_Grabbed;
        On.Player.HeavyCarry += PlayerHooks.Player_HeavyCarry;
        On.Player.TerrainImpact += PlayerHooks.Player_TerrainImpact;

        On.Player.SetMalnourished += PlayerHooks.Player_SetMalnourished;

        On.Creature.Violence += PlayerHooks.Creature_Violence;

        MREvents.OnPlayerGrab += PlayerHooks.OnPlayerGrab;
        MREvents.OnPlayerReleaseGrasp += PlayerHooks.OnPlayerReleaseGrasp;
        MREvents.OnPlayerSwitchGrasp += PlayerHooks.OnPlayerSwitchGrasp;
    }

    private static void RemovePlayerHooks()
    {
        On.Player.Update -= PlayerHooks.Player_Update;
        On.Player.GrabUpdate -= PlayerHooks.Player_GrabUpdate;
        On.Player.MovementUpdate -= PlayerHooks.Player_MovementUpdate;
        On.Player.EatMeatUpdate -= PlayerHooks.Player_EatMeatUpdate;
        On.Player.MaulingUpdate -= PlayerHooks.Player_MaulingUpdate;

        On.Player.CanIPickThisUp -= PlayerHooks.Player_CanIPickThisUp;
        On.Player.Grabbed -= PlayerHooks.Player_Grabbed;
        On.Player.HeavyCarry -= PlayerHooks.Player_HeavyCarry;
        On.Player.TerrainImpact -= PlayerHooks.Player_TerrainImpact;

        On.Player.SetMalnourished -= PlayerHooks.Player_SetMalnourished;

        On.Creature.Violence -= PlayerHooks.Creature_Violence;

        MREvents.OnPlayerGrab -= PlayerHooks.OnPlayerGrab;
        MREvents.OnPlayerReleaseGrasp -= PlayerHooks.OnPlayerReleaseGrasp;
        MREvents.OnPlayerSwitchGrasp -= PlayerHooks.OnPlayerSwitchGrasp;
    }

    // PlayerGraphics

    private static void ApplyPlayerGraphicsHooks()
    {
        On.PlayerGraphics.ctor += PlayerGraphicsHooks.PlayerGraphics_ctor;
        On.PlayerGraphics.Update += PlayerGraphicsHooks.PlayerGraphics_Update;

        //On.PlayerGraphics.DrawSprites += PlayerGraphicsHooks.PlayerGraphics_DrawSprites;
        On.PlayerGraphics.ApplyPalette += PlayerGraphicsHooks.PlayerGraphics_ApplyPalette;
    }

    private static void RemovePlayerGraphicsHooks()
    {
        On.PlayerGraphics.ctor -= PlayerGraphicsHooks.PlayerGraphics_ctor;
        On.PlayerGraphics.Update -= PlayerGraphicsHooks.PlayerGraphics_Update;

        //On.PlayerGraphics.DrawSprites += PlayerGraphicsHooks.PlayerGraphics_DrawSprites;
        On.PlayerGraphics.ApplyPalette -= PlayerGraphicsHooks.PlayerGraphics_ApplyPalette;
    }

    // SlugcatHand

    private static void ApplySlugcatHandHooks()
    {
        On.SlugcatHand.EngageInMovement += SlugcatHandHooks.SlugcatHand_EngageInMovement;
        On.SlugcatHand.Update += SlugcatHandHooks.SlugcatHand_Update;
    }

    private static void RemoveSlugcatHandHooks()
    {
        On.SlugcatHand.EngageInMovement -= SlugcatHandHooks.SlugcatHand_EngageInMovement;
        On.SlugcatHand.Update -= SlugcatHandHooks.SlugcatHand_Update;
    }

    private static void ApplyRegionStateHooks()
    {
        On.RegionState.AdaptRegionStateToWorld += RegionStateHooks.RegionState_AdaptRegionStateToWorld;
    }

    private static void RemoveRegionStateHooks()
    {
        On.RegionState.AdaptRegionStateToWorld -= RegionStateHooks.RegionState_AdaptRegionStateToWorld;
    }

    // PhysicalObject

    private static void ApplyPhysicalObjectHooks()
    {
        On.PhysicalObject.HitByWeapon += PhysicalObjectHooks.PhysicalObject_HitByWeapon;
        On.PhysicalObject.Collide += PhysicalObjectHooks.PhysicalObject_Collide;
        On.PhysicalObject.Update += PhysicalObjectHooks.PhysicalObject_Update;
    }

    private static void RemovePhysicalObjectHooks()
    {
        On.PhysicalObject.HitByWeapon -= PhysicalObjectHooks.PhysicalObject_HitByWeapon;
        On.PhysicalObject.Collide -= PhysicalObjectHooks.PhysicalObject_Collide;
        On.PhysicalObject.Update -= PhysicalObjectHooks.PhysicalObject_Update;
    }

    // PlayerCarryableItem

    private static void ApplyPlayerCarryableItemHooks()
    {
        On.PlayerCarryableItem.Update += PlayerCarryableHooks.PlayerCarryableItem_Update;
    }

    private static void RemovePlayerCarryableItemHooks()
    {
        On.PlayerCarryableItem.Update -= PlayerCarryableHooks.PlayerCarryableItem_Update;
    }
    
    // Lantern

    private static void ApplyLanternHooks()
    {
        On.Lantern.Update += LanternHooks.Lantern_Update;
    }

    private static void RemoveLanternHooks()
    {
        On.Lantern.Update -= LanternHooks.Lantern_Update;
    }

    // SlimeMold

    private static void ApplySlimeMoldHooks()
    {
        On.SlimeMold.Update += SlimeMoldHooks.SlimeMold_Update;
    }

    private static void RemoveSlimeMoldHooks()
    {
        On.SlimeMold.Update -= SlimeMoldHooks.SlimeMold_Update;
    }

    private static void ApplyWeaponHooks()
    {
        On.Weapon.AddToContainer += WeaponHooks.Weapon_AddToContainer;

        On.Weapon.HitSomething += WeaponHooks.Weapon_HitSomething;
        On.Weapon.Update += WeaponHooks.Update;
    }

    private static void RemoveWeaponHooks()
    {
        On.Weapon.AddToContainer -= WeaponHooks.Weapon_AddToContainer;

        On.Weapon.HitSomething -= WeaponHooks.Weapon_HitSomething;
        On.Weapon.Update -= WeaponHooks.Update;
    }

    // Spear

    private static void ApplySpearHooks()
    {
        On.Spear.Update += SpearHooks.Spear_Update;
        On.Spear.Thrown += SpearHooks.Spear_Thrown;
        On.Spear.LodgeInCreature_CollisionResult_bool += SpearHooks.Spear_LodgeInCreature;
        On.Spear.ChangeMode += SpearHooks.Spear_ChangeMode;

        On.Spear.HitSomething += SpearHooks.Spear_HitSomething;

        On.Spear.InitiateSprites += SpearHooks.Spear_InitiateSprites;
        On.Spear.DrawSprites += SpearHooks.Spear_DrawSprites;

        On.PhysicalObject.NewRoom += SpearHooks.Spear_NewRoom;
        On.UpdatableAndDeletable.Destroy += SpearHooks.Spear_Destroy;
    }

    private static void RemoveSpearHooks()
    {
        On.Spear.ChangeMode -= SpearHooks.Spear_ChangeMode;
        On.Spear.Update -= SpearHooks.Spear_Update;
        On.Spear.Thrown -= SpearHooks.Spear_Thrown;
        On.Spear.LodgeInCreature_CollisionResult_bool -= SpearHooks.Spear_LodgeInCreature;
        On.Spear.ChangeMode -= SpearHooks.Spear_ChangeMode;

        On.Spear.HitSomething += SpearHooks.Spear_HitSomething;

        On.Spear.InitiateSprites -= SpearHooks.Spear_InitiateSprites;
        On.Spear.DrawSprites -= SpearHooks.Spear_DrawSprites;

        On.PhysicalObject.NewRoom -= SpearHooks.Spear_NewRoom;
        On.UpdatableAndDeletable.Destroy -= SpearHooks.Spear_Destroy;
    }

    // SporePlant

    private static void ApplySporePlantHooks()
    {
        On.SporePlant.Update += SporePlantHooks.SporePlant_Update;
        On.SporePlant.Collide += SporePlantHooks.SporePlant_Collide;
        On.SporePlant.DrawSprites += SporePlantHooks.SporePlant_DrawSprites;
    }

    private static void RemoveSporePlantHooks()
    {
        On.SporePlant.Update -= SporePlantHooks.SporePlant_Update;
        On.SporePlant.Collide -= SporePlantHooks.SporePlant_Collide;
        On.SporePlant.DrawSprites -= SporePlantHooks.SporePlant_DrawSprites;
    }

    // Spark

    private static void ApplySparkHooks()
    {
        On.Spark.Update += SparkHooks.Spark_Update;
        On.Spark.DrawSprites += SparkHooks.Spark_DrawSprites;
    }

    private static void RemoveSparkHooks()
    {
        On.Spark.Update -= SparkHooks.Spark_Update;
        On.Spark.DrawSprites -= SparkHooks.Spark_DrawSprites;
    }
}