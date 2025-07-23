namespace SlugCrafting;

public static partial class Hooks
{
    internal static void ApplyHooks()
    {
        ApplyPlayerHooks();
        ApplyPlayerGraphicsHooks();

        ApplyPlayerCarryableItemHooks();
        ApplySpearHooks();
        ApplySporePlantHooks();

        //ApplySparkHooks(); //-- MR7: Uneeded, disabled
    }

    internal static void RemoveHooks()
    {
        On.RainWorld.PostModsInit -= Plugin.RainWorld_PostModsInit;

        RemovePlayerHooks();
        RemovePlayerGraphicsHooks();

        RemovePlayerCarryableItemHooks();
        RemoveSpearHooks();
        RemoveSporePlantHooks();

        //RemoveSparkHooks();
    }

    // PLAYER HOOKS

    private static void ApplyPlayerHooks()
    {
        On.Player.Update += PlayerHooks.Player_Update;
        On.Player.MovementUpdate += PlayerHooks.Player_MovementUpdate;
        On.Player.GrabUpdate += PlayerHooks.Player_GrabUpdate;
        On.Player.EatMeatUpdate += PlayerHooks.Player_EatMeatUpdate;
        On.Player.MaulingUpdate += PlayerHooks.Player_MaulingUpdate;

        On.Player.Grabbed += PlayerHooks.Player_Grabbed;
        On.Player.TerrainImpact += PlayerHooks.Player_TerrainImpact;

        On.Player.SetMalnourished += PlayerHooks.Player_SetMalnourished;

        On.Creature.Violence += PlayerHooks.Creature_Violence;

        MREvents.OnPlayerGrab += PlayerExtension.OnPlayerGrab;
        MREvents.OnPlayerReleaseGrasp += PlayerExtension.OnPlayerReleaseGrasp;
        MREvents.OnPlayerSwitchGrasp += PlayerExtension.OnPlayerSwitchGrasp;
    }

    private static void RemovePlayerHooks()
    {
        On.Player.Update -= PlayerHooks.Player_Update;
        On.Player.GrabUpdate -= PlayerHooks.Player_GrabUpdate;
        On.Player.MovementUpdate -= PlayerHooks.Player_MovementUpdate;
        On.Player.EatMeatUpdate -= PlayerHooks.Player_EatMeatUpdate;
        On.Player.MaulingUpdate -= PlayerHooks.Player_MaulingUpdate;

        On.Player.Grabbed -= PlayerHooks.Player_Grabbed;
        On.Player.TerrainImpact -= PlayerHooks.Player_TerrainImpact;

        On.Player.SetMalnourished -= PlayerHooks.Player_SetMalnourished;

        On.Creature.Violence -= PlayerHooks.Creature_Violence;

        MREvents.OnPlayerGrab -= PlayerExtension.OnPlayerGrab;
        MREvents.OnPlayerReleaseGrasp -= PlayerExtension.OnPlayerReleaseGrasp;
        MREvents.OnPlayerSwitchGrasp -= PlayerExtension.OnPlayerSwitchGrasp;
    }

    // PLAYER GRAPHICS

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

    // PLAYER CARRYABLE ITEMS

    private static void ApplyPlayerCarryableItemHooks()
    {
        On.PlayerCarryableItem.Update += PlayerCarryableHooks.PlayerCarryableItem_Update;
    }

    private static void RemovePlayerCarryableItemHooks()
    {
        On.PlayerCarryableItem.Update -= PlayerCarryableHooks.PlayerCarryableItem_Update;
    }

    // SPEAR

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

    // SPORE PLANT

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

    // SPARK

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