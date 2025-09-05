namespace SlugCrafting.Items;

public class LizardShellCuirass : LizardShellArmor
{
    public struct DrawSpritesContext
    {
        public float posX;
        public float posY;
        public float scaleX;
        public float scaleY;
        public float rotation;
        public string spriteAngle;
        public float anchorX;
        public float anchorY;

        public DrawSpritesContext(float posX, float posY, float scaleX, float scaleY, float rotation, string spriteAngle, float anchorX, float anchorY)
        {
            this.posX = posX;
            this.posY = posY;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.rotation = rotation;
            this.spriteAngle = spriteAngle;
            this.anchorX = anchorX;
            this.anchorY = anchorX;
        }
    }

    public readonly struct SpriteInfo
    {
        public readonly string name;

        public SpriteInfo(string name)
        {
            this.name = name;
        }
    }
    private SpriteInfo[] _bodySpritesInfo;
    private SpriteInfo[] _hipsSpritesInfo;

    public int totalSpritesInfoLength => _bodySpritesInfo.Length + _hipsSpritesInfo.Length;

    public AbstractLizardShellCuirass AbstractLizardShellCuirass => (AbstractLizardShellCuirass)base.AbstractLizardShellArmor;

    public new LizardShellCuirassItemProperties ItemProperties => (LizardShellCuirassItemProperties)base.ItemProperties;
    public new LizardShellCuirassAccessoryProperties AccessoryProperties => (LizardShellCuirassAccessoryProperties)base.AccessoryProperties;

    public LizardShellCuirass(AbstractLizardShellCuirass abstractLizardShellCuirass, LizardShellCuirassItemProperties itemProperties) : base(abstractLizardShellCuirass, itemProperties)
    {
        _bodySpritesInfo = AccessoryProperties.BodySpritesInfo;
        _hipsSpritesInfo = AccessoryProperties.HipsSpritesInfo;
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[totalSpritesInfoLength];

        for (int i = 0; i < _bodySpritesInfo.Length; i++)
        {
            sLeaser.sprites[i] = new FSprite(_bodySpritesInfo[i].name + "A0", true);
            sLeaser.sprites[i].color = Color.white; // Default color, can be changed later.
        }

        for (int i = 0; i < _hipsSpritesInfo.Length; i++)
        {
            int spriteIndex = _bodySpritesInfo.Length + i;
            sLeaser.sprites[spriteIndex] = new FSprite(_hipsSpritesInfo[i].name + "A0", true);
            sLeaser.sprites[spriteIndex].color = Color.white;
        }
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos, DrawSpritesContext bodyDrawContext, DrawSpritesContext hipsDrawContext)
    {
        int scaleX = bodyDrawContext.spriteAngle.StartsWith("-") ? -1 : 1;
        var spriteAngle = GraphicsModuleCCGExtensions.GetSymmetricalAngleFromAsymmetrical(bodyDrawContext.spriteAngle);

        for (int i = 0; i < _bodySpritesInfo.Length; i++)
        {
            sLeaser.sprites[i].x = bodyDrawContext.posX;
            sLeaser.sprites[i].y = bodyDrawContext.posY;
            sLeaser.sprites[i].scaleX = scaleX;
            sLeaser.sprites[i].scaleY = bodyDrawContext.scaleY;
            sLeaser.sprites[i].rotation = bodyDrawContext.rotation;
            sLeaser.sprites[i].element = Futile.atlasManager.GetElementWithName(_bodySpritesInfo[i].name + spriteAngle);
            sLeaser.sprites[i].anchorX = bodyDrawContext.anchorX;
            sLeaser.sprites[i].anchorY = bodyDrawContext.anchorY;
        }

        for (int i = 0; i < _hipsSpritesInfo.Length; i++)
        {
            int spriteIndex = _bodySpritesInfo.Length + i;
            sLeaser.sprites[spriteIndex].x = hipsDrawContext.posX;
            sLeaser.sprites[spriteIndex].y = hipsDrawContext.posY;
            sLeaser.sprites[spriteIndex].scaleX = scaleX;
            sLeaser.sprites[spriteIndex].scaleY = hipsDrawContext.scaleY;
            sLeaser.sprites[spriteIndex].rotation = hipsDrawContext.rotation;
            sLeaser.sprites[spriteIndex].element = Futile.atlasManager.GetElementWithName(_hipsSpritesInfo[i].name + spriteAngle);
            sLeaser.sprites[spriteIndex].anchorX = hipsDrawContext.anchorX;
            sLeaser.sprites[spriteIndex].anchorY = hipsDrawContext.anchorY;
        }

        ApplySpriteEffectGroupColors(sLeaser);
    }
}
