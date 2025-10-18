namespace SlugCrafting.Items.String;

public class SpiderSilkStringItem : CordItem
{
    public SpiderSilkStringItem(AbstractCord abstractCord, CordProperties properties) : base(abstractCord, properties)
    {
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        cordColor = abstractCord.color;
        UpdateColor(sLeaser, false);
    }
}
