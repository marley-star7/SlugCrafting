namespace SlugCrafting.Items;

public abstract class LizardShellArmor
{
    public AbstractLizardShellArmor AbstractLizardShellArmor;

    public LizardShellArmorItemProperties ItemProperties;
    public LizardShellArmorAccessoryProperties AccessoryProperties => ItemProperties.ArmorAccessoryProperties;

    public LizardEffectColorGraphics lizardEffectColorGraphics;

    public SpriteLayerGroup[] spriteLayerGroups;

    public SpriteEffectGroup[] spriteEffectGroups;
    public SpriteEffectGroup effectColorGroup => spriteEffectGroups[0]; // The first group is the color group, the second is the dark group.
    public SpriteEffectGroup blackColorGroup => spriteEffectGroups[1]; // The second group is the dark group, the first is the color group.
    public SpriteEffectGroup darkEffectColorGroup => spriteEffectGroups[2];

    protected Color blackColor = Color.black;

    public float terrainImpactNoiseModifier = 3;

    public LizardShellArmor(AbstractLizardShellArmor abstractLizardShellArmor, LizardShellArmorItemProperties itemProperties)
    {
        this.AbstractLizardShellArmor = abstractLizardShellArmor;
        this.ItemProperties = itemProperties;

        lizardEffectColorGraphics = new LizardEffectColorGraphics(itemProperties.ArmorAccessoryProperties.DefaultShellColor);

        spriteLayerGroups = AccessoryProperties.SpriteLayerGroups;
        spriteEffectGroups = AccessoryProperties.SpriteEffectGroups;
    }

    public void Update(bool eu)
    {
        lizardEffectColorGraphics.Update();
    }

    public abstract void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam);

    protected void ApplySpriteEffectGroupColors(RoomCamera.SpriteLeaser sLeaser)
    {
        var effectColor = lizardEffectColorGraphics.ShellColor(AbstractLizardShellArmor.health, ItemProperties.ArmorAccessoryProperties.MaxHealth);
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
        lizardEffectColorGraphics.ApplyPalette(palette);
        blackColor = palette.blackColor;
    }

    // --- The Fun Functions --- //

    public virtual void OnWearerCollide(Player player, PhysicalObject otherObject, int myChunk, int otherChunk)
    {

    }

    public virtual void DoTerrainImpactEffects(BodyChunk impactChunk, Vector2 direction, float speed, bool firstContact)
    {
        var owner = impactChunk.owner;
        var vol = Mathf.Clamp(speed * 0.07f, 0, 0.7f); //--MS7: Limit volume to not blow your ears off lol.
        var noiseStrength = Mathf.Clamp(speed * terrainImpactNoiseModifier, 0, 100f);

        owner.room.PlaySound(SoundID.Spear_Fragment_Bounce, impactChunk, false, vol, Random.Range(0.8f, 1.2f));
        owner.room.InGameNoise(new InGameNoise(impactChunk.pos, noiseStrength, owner, 1f));
        int sparkNum = (int)Random.Range(vol * 2, vol * 7);

        //lizardEffectColorGraphics.Flicker((int)Mathf.Max(speed * 0.3f, 30));
        SpawnSparks(owner, impactChunk.pos, direction * speed, sparkNum);
    }

    private static float Rand => Random.value;

    private void SpawnSparks(UpdatableAndDeletable owner, Vector2 sourcePos, Vector2 directionAndMomentum, int sparkNum)
    {
        Color sparkColor = lizardEffectColorGraphics.ShellColor(AbstractLizardShellArmor.health, ItemProperties.ArmorAccessoryProperties.MaxHealth);

        for (int k = 0; k < sparkNum; k++)
        {
            //-- MS7: Figure out how to make sparks have the lizard graphics thing where they change color, without NEEDING lizard graphics.
            Vector2 pos = sourcePos + Custom.DegToVec(Rand * 360f) * 5f * Rand;
            Vector2 vel = -directionAndMomentum * -0.1f + Custom.DegToVec(Rand * 360f) * Mathf.Lerp(0.2f, 0.4f, Rand) * directionAndMomentum.magnitude;
            owner.room.AddObject(new Spark(pos, vel, sparkColor, null, 10, 170));
        }
    }

    public void Shatter(UpdatableAndDeletable owner, Vector2 pos)
    {
        for (int k = 0; k < 5; k++)
        {
            owner.room.AddObject(new LizardShellFragment(pos, Custom.RNV() * Mathf.Lerp(5f, 15f, UnityEngine.Random.value), lizardEffectColorGraphics.ShellColor(AbstractLizardShellArmor.health, ItemProperties.ArmorAccessoryProperties.MaxHealth)));
        }
        owner.Destroy();
    }

    public void DoDeflectEffects(BodyChunk chunkHit, Vector2 sourcePos, Vector2 directionAndMomentum, float damage, float stunBonus)
    {
        var owner = chunkHit.owner;
        //-- MS7: Required visual and audio queue for deflecting a hit.
        Color sparkColor;

        float flickerTimeF = (damage * 30f + stunBonus);
        int flickerTime = (int)(Mathf.Clamp(flickerTimeF, 25f, damage * 30f));
        lizardEffectColorGraphics.WhiteFlicker(flickerTime);

        SpawnSparks(owner, sourcePos, directionAndMomentum, Random.Range(3, 8));
        owner.room.AddObject(new StationaryEffect(sourcePos, new Color(1f, 1f, 1f), null, StationaryEffect.EffectType.FlashingOrb));
        owner.room.PlaySound(SoundID.Spear_Bounce_Off_Creauture_Shell, chunkHit);
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
