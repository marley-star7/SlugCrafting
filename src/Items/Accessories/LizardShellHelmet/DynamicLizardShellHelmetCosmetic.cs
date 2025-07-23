using UnityEngine;
using RWCustom;
using Noise;
namespace SlugCrafting.Cosmetics;

public class DynamicLizardShellHelmetCosmetic : DynamicSlugcatFullHeadAttachedCosmetic
{
    public LizardShellColorGraphics lizardShellColorGraphics;

    public AccessoryArmorStats armorStats;

    protected int ColorEffectLayer => 0;
    protected int DarkEffectLayer => 1;

    public DynamicLizardShellHelmetCosmetic(AccessoryArmorStats armorStats, SpriteInfo[] spritesInfo, SpriteLayerGroup[] spriteLayers) : base(spritesInfo, spriteLayers)
    {
        this.armorStats = armorStats;
        lizardShellColorGraphics = new LizardShellColorGraphics(Color.green);
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        lizardShellColorGraphics.Update();
    }

    public override void OnWearerDrawSprites(RoomCamera.SpriteLeaser wearerSLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        base.OnWearerDrawSprites(wearerSLeaser, rCam, timeStacker, camPos);
        lizardShellColorGraphics.DrawSpritesUpdate();

        // Make the colors of the sprites in the ColorEffectLayer green.
        for (int i = 0; i < spriteEffectGroups[ColorEffectLayer].sprites.Length; i++)
        {
            var currentSpriteIndex = spriteEffectGroups[ColorEffectLayer].sprites[i];
            sLeaser.sprites[currentSpriteIndex].color = lizardShellColorGraphics.ShellColor(armorStats.health, armorStats.maxHealth);
        }
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        lizardShellColorGraphics.ApplyPalette(palette);

        // Make the colors of the sprites in the DarkEffectLayer black.
        for (int i = 0; i < spriteEffectGroups[DarkEffectLayer].sprites.Length; i++)
        {
            var currentSpriteIndex = spriteEffectGroups[DarkEffectLayer].sprites[i];
            sLeaser.sprites[currentSpriteIndex].color = palette.blackColor;
        }
    }
}
