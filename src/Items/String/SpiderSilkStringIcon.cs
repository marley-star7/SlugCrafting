namespace SlugCrafting.Items;

public class SpiderSilkStringIcon : Icon
{
    public override int Data(AbstractPhysicalObject apo)
        => 0;

    public override Color SpriteColor(int data)
        => Consts.IconColors.MushroomWhite;

    public override string SpriteName(int data)
        => "Symbol_String";
}