using MRCustom.Modules.PhysicalObjects;

namespace SlugCrafting.Items;

public abstract class LizardShellArmor
{
    public AbstractLizardShellArmor AbstractLizardShellArmor;

    public LizardShellArmorItemProperties ItemProperties;
    public LizardShellArmorAccessoryProperties AccessoryProperties => ItemProperties.ArmorAccessoryProperties;

    public LizardShellEffectsModule lizardShellEffectsModule;
    public LizardEffectColorGraphics LizardShellEffectColorGraphics => lizardShellEffectsModule.effectColorGraphics;

    public SpriteLayerGroup[] spriteLayerGroups;

    public SpriteEffectGroup[] spriteEffectGroups;
    public SpriteEffectGroup effectColorGroup => spriteEffectGroups[0]; // The first group is the color group, the second is the dark group.
    public SpriteEffectGroup blackColorGroup => spriteEffectGroups[1]; // The second group is the dark group, the first is the color group.
    public SpriteEffectGroup darkEffectColorGroup => spriteEffectGroups[2];

    protected Color blackColor = Color.black;

    public float terrainImpactNoiseModifier = 3;

    public LizardShellArmor(AbstractLizardShellArmor abstractLizardShellArmor, LizardShellArmorItemProperties itemProperties, LizardShellEffectsModule lizardShellEffectsModule)
    {
        this.AbstractLizardShellArmor = abstractLizardShellArmor;
        this.ItemProperties = itemProperties;
        this.lizardShellEffectsModule = lizardShellEffectsModule;

        spriteLayerGroups = AccessoryProperties.SpriteLayerGroups;
        spriteEffectGroups = AccessoryProperties.SpriteEffectGroups;
    }

    public void Update(bool eu)
    {
        lizardShellEffectsModule.Update();
        lizardShellEffectsModule.effectColorGraphics.brightness = Mathf.InverseLerp(0, AccessoryProperties.MaxHealth, AbstractLizardShellArmor.health);
    }

    public abstract void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam);

    protected void ApplySpriteEffectGroupColors(RoomCamera.SpriteLeaser sLeaser)
    {
        var effectColor = LizardShellEffectColorGraphics.ShellColor();
        for (int i = 0; i < effectColorGroup.sprites.Length; i++)
        {
            sLeaser.sprites[effectColorGroup.sprites[i]].color = effectColor;
        }

        // Ideally when the SpriteEffectGroup is set empty, it's just a 0 length array, but for some reason instead it gets set to null???
        // Idek, so doing this check for null so like the mod comes out eventually lol, don't got time for all dis.

        if (darkEffectColorGroup.sprites != null)
        {
            var darkEffectColor = Color.Lerp(effectColor, blackColor, 0.1f);
            for (int i = 0; i < darkEffectColorGroup.sprites.Length; i++)
            {
                sLeaser.sprites[darkEffectColorGroup.sprites[i]].color = darkEffectColor;
            }
        }

        if (blackColorGroup.sprites != null)
        {
            for (int i = 0; i < blackColorGroup.sprites.Length; i++)
            {
                sLeaser.sprites[blackColorGroup.sprites[i]].color = blackColor;
            }
        }
    }

    public virtual void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        if (sLeaser == null)
            return;

        ApplySpriteEffectGroupColors(sLeaser);
    }

    public virtual void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        LizardShellEffectColorGraphics.ApplyPalette(palette);
        blackColor = palette.blackColor;
    }

    // --- The Fun Functions --- //

    public virtual void OnWearerCollide(Player player, PhysicalObject otherObject, int myChunk, int otherChunk)
    {

    }

    public virtual void DoTerrainImpactEffects(BodyChunk impactChunk, Vector2 direction, float speed, bool firstContact)
    {
        lizardShellEffectsModule.DoTerrainImpactEffects(impactChunk, direction, speed, firstContact);
    }

    public void Shatter(UpdatableAndDeletable owner, Vector2 pos)
    {
        lizardShellEffectsModule.DoShatterEffects(pos);
        owner.Destroy();
    }

    public void DoDeflectEffects(BodyChunk chunkHit, Vector2 sourcePos, Vector2 directionAndMomentum, float damage, float stunBonus)
    {
        lizardShellEffectsModule.DoDeflectEffects(chunkHit, sourcePos, directionAndMomentum, damage, stunBonus);
    }

    /// <summary>
    /// Ms7: The higher the z angle is, the closer it is to "facing" the player, change the angle based on that.
    /// </summary>
    /// <param name="rotationZ"></param>
    /// <returns></returns>
    public static string GetAngleForRotationZ(float rotationZ)
    {
        if (Math.Abs(rotationZ) > 0.6f)
        {
            return "A0";
        }
        else if (Math.Abs(rotationZ) > 0.3f)
        {
            return "A1";
        }
        else
        {
            return "A0";
        }
    }
}
