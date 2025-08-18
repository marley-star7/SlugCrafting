using MRCustom.Contexts;

namespace SlugCrafting.Items;

public class LizardShellHelmet
{
    public struct DrawSpritesContext
    {
        public float posX;
        public float posY;
        public float scaleX;
        public float scaleY;
        public float rotation;
        public string spriteAngle;
        public float lookDirX;
        public float lookDirY;

        public DrawSpritesContext(float posX, float posY, float scaleX, float scaleY, float rotation, string spriteAngle, float lookDirX, float lookDirY)
        {
            this.posX = posX;
            this.posY = posY;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.rotation = rotation;
            this.spriteAngle = spriteAngle;
            this.lookDirX = lookDirX;
            this.lookDirY = lookDirY;
        }
    }

    public struct SpriteInfo
    {
        public string name;
        public float distanceFromHeadModifier = 1f;

        public SpriteInfo(string name)
        {
            this.name = name;
        }
    }
    private SpriteInfo[] spritesInfo;

    public readonly SpriteLayerGroup[] spriteLayerGroups;

    public SpriteEffectGroup[] spriteEffectGroups;
    public SpriteEffectGroup effectColorGroup => spriteEffectGroups[0]; // The first group is the color group, the second is the dark group.
    public SpriteEffectGroup blackColorGroup => spriteEffectGroups[1]; // The second group is the dark group, the first is the color group.
    public SpriteEffectGroup darkEffectColorGroup => spriteEffectGroups[2];

    public AbstractLizardShellHelmet abstractLizardShellHelmet;
    public LizardShellHelmetProperties properties;

    public LizardEffectColorGraphics lizardEffectColorGraphics;

    public float terrainImpactNoiseModifier = 3;

    Color blackColor = Color.black;

    private static float Rand => Random.value;

    public LizardShellHelmet(AbstractLizardShellHelmet abstractLizardShellHelmet, LizardShellHelmetProperties properties)
    {
        this.abstractLizardShellHelmet = abstractLizardShellHelmet;
        this.properties = properties;

        lizardEffectColorGraphics = new LizardEffectColorGraphics(abstractLizardShellHelmet.shellColor);

        spritesInfo = properties.spritesInfo;
        spriteLayerGroups = properties.spriteLayerGroups;
        spriteEffectGroups = properties.spriteEffectGroups;
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        lizardEffectColorGraphics.ApplyPalette(palette);
        blackColor = palette.blackColor;
    }

    public void Update(bool eu)
    {
        lizardEffectColorGraphics.Update();
    }

    private void SpawnSparks(UpdatableAndDeletable owner, Vector2 sourcePos, Vector2 directionAndMomentum, int sparkNum)
    {
        Color sparkColor = lizardEffectColorGraphics.ShellColor(abstractLizardShellHelmet.health, properties.maxHealth);

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
            owner.room.AddObject(new LizardShellFragment(pos, Custom.RNV() * Mathf.Lerp(5f, 15f, UnityEngine.Random.value), lizardEffectColorGraphics.ShellColor(abstractLizardShellHelmet.health, properties.maxHealth)));
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

    public void OnWearerCollide(Player player, PhysicalObject otherObject, int myChunk, int otherChunk)
    {

    }

    public void DoTerrainImpactEffects(BodyChunk impactChunk, Vector2 direction, float speed, bool firstContact)
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

    //
    // IDrawable and IDynamicCosmetic
    //

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[spritesInfo.Length];

        for (int i = 0; i < spritesInfo.Length; i++)
        {
            sLeaser.sprites[i] = new FSprite(spritesInfo[i].name + "A0", true);
            sLeaser.sprites[i].color = Color.white; // Default color, can be changed later.
        }
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos, DrawSpritesContext drawContext)
    {
        if (sLeaser == null)
            return;

        int scaleX = drawContext.spriteAngle.StartsWith("-") ? -1 : 1;
        var spriteAngle = GraphicsModuleCCGExtensions.GetSymmetricalAngleFromAsymmetrical(drawContext.spriteAngle);

        //-- Loop through and update all sprites behind the head + in front of face match the face sprites sprite.
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].x = drawContext.posX + drawContext.lookDirX * spritesInfo[i].distanceFromHeadModifier;
            sLeaser.sprites[i].y = drawContext.posY + drawContext.lookDirY * spritesInfo[i].distanceFromHeadModifier;
            sLeaser.sprites[i].scaleX = scaleX;
            sLeaser.sprites[i].scaleY = drawContext.scaleY;
            sLeaser.sprites[i].rotation = drawContext.rotation;
            sLeaser.sprites[i].element = Futile.atlasManager.GetElementWithName(spritesInfo[i].name + spriteAngle);
        }

        var effectColor = lizardEffectColorGraphics.ShellColor(abstractLizardShellHelmet.health, properties.maxHealth);
        for (int i = 0; i < effectColorGroup.sprites.Length; i++)
        {
            sLeaser.sprites[effectColorGroup.sprites[i]].color = effectColor;
        }

        /*
        var darkEffectColor = Color.Lerp(effectColor, blackColor, 0.1f);
        for (int i = 0; i < darkEffectColorGroup.sprites.Length; i++)
        {
            sLeaser.sprites[darkEffectColorGroup.sprites[i]].color = darkEffectColor;
        }
        */

        for (int i = 0; i < blackColorGroup.sprites.Length; i++)
        {
            sLeaser.sprites[blackColorGroup.sprites[i]].color = blackColor;
        }

    }
}
