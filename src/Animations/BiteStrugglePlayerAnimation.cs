namespace SlugCrafting.Animations;

public class BiteStruggleAnimation : RWAnimation<Player>
{
    public int hand = 1;

    public override void Start(Player player)
    {
    }

    public override void Stop(Player player)
    {

    }

    public override void Update(Player player, float animTimer)
    {

    }

    public override void GraphicsUpdate(Player player, float animTimer)
    {
        var playerGraphics = player.graphicsModule as PlayerGraphics;
        if (playerGraphics != null && player.grasps[hand] != null && player.grasps[hand].grabbed != null)
            playerGraphics.BiteStruggle(hand);
    }
}
