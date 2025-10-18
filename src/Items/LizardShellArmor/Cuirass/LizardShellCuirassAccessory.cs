namespace SlugCrafting.Items;

public class LizardShellCuirassAccessory : LizardShellArmorAccessory
{
    public LizardShellCuirass lizardShellCuirass => (LizardShellCuirass)base.lizardShellArmor;

    public new LizardShellCuirassItemProperties ItemProperties => lizardShellCuirass.ItemProperties;
    public new LizardShellCuirassAccessoryProperties AccessoryProperties => lizardShellCuirass.AccessoryProperties;

    public LizardShellCuirassAccessory(Player owner, LizardShellCuirass lizardShellCuirass) : base(owner, lizardShellCuirass, EntityBodyChunkIndexes.Player.Body)
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

        LizardShellCuirass.DrawSpritesContext bodyDrawContext = new LizardShellCuirass.DrawSpritesContext(
            posX: playerGraphicsCCGData.BaseBodySprite.x, 
            posY: playerGraphicsCCGData.BaseBodySprite.y,
            scaleX: playerGraphicsCCGData.BaseBodySprite.scaleX, 
            scaleY: playerGraphicsCCGData.BaseBodySprite.scaleY,
            rotation: playerGraphicsCCGData.BaseBodySprite.rotation,
            spriteAngle: "A0",
            anchorX: playerGraphicsCCGData.BaseBodySprite.anchorX,
            anchorY: playerGraphicsCCGData.BaseBodySprite.anchorY
        );

        LizardShellCuirass.DrawSpritesContext hipsDrawContext = new LizardShellCuirass.DrawSpritesContext(
            posX: playerGraphicsCCGData.BaseHipsSprite.x,
            posY: playerGraphicsCCGData.BaseHipsSprite.y,
            scaleX: playerGraphicsCCGData.BaseHipsSprite.scaleX,
            scaleY: playerGraphicsCCGData.BaseHipsSprite.scaleY,
            rotation: playerGraphicsCCGData.BaseHipsSprite.rotation,
            spriteAngle: "A0",
            anchorX: playerGraphicsCCGData.BaseHipsSprite.anchorX,
            anchorY: playerGraphicsCCGData.BaseHipsSprite.anchorY
        );

        lizardShellCuirass.DrawSprites(SLeaser, rCam, timeStacker, camPos, bodyDrawContext, hipsDrawContext);
    }

    public override void PostWearerTerrainImpact(Player player, int chunkIndex, IntVector2 direction, float speed, bool firstContact)
    {
        var impactChunk = wearer.bodyChunks[chunkIndex];
        var directionVec2 = new Vector2(direction.x, direction.y);

        if (!firstContact
            || chunkIndex != wearingBodyChunkIndex)
            return;

        //-- MS7: Does not collide if the collision was too far down.
        if (Vector2.Dot(wearer.bodyChunks[chunkIndex].Rotation, directionVec2) > 0.7f)
            return;

        lizardShellArmor.DoTerrainImpactEffects(impactChunk, directionVec2, speed, firstContact);
    }
}
