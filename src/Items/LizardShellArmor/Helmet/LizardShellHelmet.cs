namespace SlugCrafting.Items;

public class LizardShellHelmet : LizardShellArmor
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

    public readonly struct SpriteInfo
    {
        public readonly string name;
        public readonly float distanceFromHeadModifier = 1f;

        public SpriteInfo(string name, float distanceFromHeadModifier = 1f)
        {
            this.name = name;
            this.distanceFromHeadModifier = distanceFromHeadModifier;
        }
    }
    private SpriteInfo[] _spritesInfo;

    public AbstractLizardShellHelmet AbstractLizardShellHelmet => (AbstractLizardShellHelmet)base.AbstractLizardShellArmor;

    public new LizardShellHelmetItemProperties ItemProperties => (LizardShellHelmetItemProperties)base.ItemProperties;
    public new LizardShellHelmetAccessoryProperties AccessoryProperties => (LizardShellHelmetAccessoryProperties)base.AccessoryProperties;

    public LizardShellHelmet(AbstractLizardShellHelmet abstractLizardShellHelmet, LizardShellHelmetItemProperties itemProperties) : base(abstractLizardShellHelmet, itemProperties)
    {
        _spritesInfo = AccessoryProperties.SpritesInfo;
    }

    //
    // IDrawable and IDynamicCosmetic
    //

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[_spritesInfo.Length];

        for (int i = 0; i < _spritesInfo.Length; i++)
        {
            sLeaser.sprites[i] = new FSprite(_spritesInfo[i].name + "A0", true);
            sLeaser.sprites[i].color = Color.white; // Default color, can be changed later.
        }
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos, DrawSpritesContext drawContext)
    {
        int scaleX = drawContext.spriteAngle.StartsWith("-") ? -1 : 1;
        var spriteAngle = GraphicsModuleCCGExtensions.GetSymmetricalAngleFromAsymmetrical(drawContext.spriteAngle);

        //-- Loop through and update all sprites behind the head + in front of face match the face sprites sprite.
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].x = drawContext.posX + drawContext.lookDirX * _spritesInfo[i].distanceFromHeadModifier;
            sLeaser.sprites[i].y = drawContext.posY + drawContext.lookDirY * _spritesInfo[i].distanceFromHeadModifier;
            sLeaser.sprites[i].scaleX = scaleX;
            sLeaser.sprites[i].scaleY = drawContext.scaleY;
            sLeaser.sprites[i].rotation = drawContext.rotation;
            sLeaser.sprites[i].element = Futile.atlasManager.GetElementWithName(_spritesInfo[i].name + spriteAngle);
        }

        ApplySpriteEffectGroupColors(sLeaser);
    }
}
