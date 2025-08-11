namespace SlugCrafting.Items;

public class CordProperties : ItemProperties
{
    public static Dictionary<AbstractPhysicalObject.AbstractObjectType, CordProperties> typesProperties = new();

    public virtual bool isPaletteBlackColor => true;

    public virtual Color color => Color.white;

    public virtual float thickness => 1f;

    public override void Throwable(Player player, ref bool throwable)
        => throwable = true;

    public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 1;

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
        => grabability = Player.ObjectGrabability.OneHand;
}
