namespace SlugCrafting.Items;

public class CordIcon : Icon
{
    public override int Data(AbstractPhysicalObject apo)
        => 0;

    public override Color SpriteColor(int data)
        => Consts.IconColors.MediumGrey;

    public override string SpriteName(int data)
        => "Symbol_Cord";
}