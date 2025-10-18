namespace SlugCrafting.Items;

public class LizardShellHelmetAccessory : LizardShellArmorAccessory
{
    public LizardShellHelmet lizardShellHelmet => (LizardShellHelmet)base.lizardShellArmor;

    public new LizardShellHelmetItemProperties ItemProperties => lizardShellHelmet.ItemProperties;
    public new LizardShellHelmetAccessoryProperties AccessoryProperties => lizardShellHelmet.AccessoryProperties;

    public LizardShellHelmetAccessory(Player owner, LizardShellHelmet lizardShellHelmet) : base(owner, lizardShellHelmet, EntityBodyChunkIndexes.Player.Head)
    {

    }

    public override void PostWearerDrawSprites(RoomCamera.SpriteLeaser wearerSLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        if (SLeaser == null)
            return;

        var playerGraphics = (PlayerGraphics)wearer.graphicsModule;
        var playerGraphicsCCGData = playerGraphics.GetPlayerGraphicsCCGData();

        if (playerGraphicsCCGData.sLeaser == null)
            return;

        //-- MS7: To achieve the effect of being behind we make get an offset from face angle different to position the head.
        var lookDirX = playerGraphicsCCGData.BaseFaceSprite.x - playerGraphicsCCGData.BaseHeadSprite.x;
        var lookDirY = playerGraphicsCCGData.BaseFaceSprite.y - playerGraphicsCCGData.BaseHeadSprite.y;

        var faceRotationTimeStacked = Vector2.Lerp(playerGraphicsCCGData.lastFaceRotation, playerGraphicsCCGData.faceRotation, timeStacker);
        var rot = Custom.VecToDeg(faceRotationTimeStacked);

        LizardShellHelmet.DrawSpritesContext context = new LizardShellHelmet.DrawSpritesContext(
            posX: playerGraphicsCCGData.BaseHeadSprite.x, 
            posY: playerGraphicsCCGData.BaseHeadSprite.y,
            scaleX: playerGraphicsCCGData.BaseFaceSprite.scaleX, 
            scaleY: playerGraphicsCCGData.BaseFaceSprite.scaleY,
            rotation: rot,
            spriteAngle: playerGraphicsCCGData.faceSpriteAngleAsymmetrical,
            lookDirX: lookDirX, 
            lookDirY: lookDirY
        );

        lizardShellHelmet.DrawSprites(SLeaser, rCam, timeStacker, camPos, context);
    }

    public override void PostWearerTerrainImpact(Player player, int chunkIndex, IntVector2 direction, float speed, bool firstContact)
    {
        var impactChunk = wearer.bodyChunks[chunkIndex];
        var directionVec2 = new Vector2(direction.x, direction.y);

        if (!firstContact
            || chunkIndex != wearingBodyChunkIndex)
            return;

        //-- MS7: Only do terrain impact effects when worn if the impact direction is angled roughly the same as the helmet.
        if (Vector2.Dot(wearer.bodyChunks[chunkIndex].Rotation, directionVec2) < 0f)
            return;

        lizardShellArmor.DoTerrainImpactEffects(impactChunk, directionVec2, speed, firstContact);
    }
}
