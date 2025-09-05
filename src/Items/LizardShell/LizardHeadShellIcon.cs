namespace SlugCrafting.Items;

public class LizardHeadShellIcon : Icon
{
    private Color _spriteColor;

    public LizardHeadShellIcon(Color spriteColor)
    {
        _spriteColor = spriteColor;
    }

    public override string SpriteName(int data) => "icon_LizardHeadShell";

    public override Color SpriteColor(int data) => _spriteColor;

    public override int Data(AbstractPhysicalObject apo) => 0;
}
