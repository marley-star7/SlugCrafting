namespace SlugCrafting.Animations;

public class SwallowPlayerAnimation : RWAnimation<Player>
{
    public override void Start(Player player)
    {

    }

    public override void Stop(Player player)
    {
        //-- MS7: Do this or else will swallow cuz holding button.

        player.swallowAndRegurgitateCounter = 0;
    }

    public override void Update(Player player, float animationTimer)
    {
        //-- MS7: Cannot go over 89 or will swallow!

        if (animationTimer < 89)
            player.swallowAndRegurgitateCounter = (int)animationTimer;
        else
            player.swallowAndRegurgitateCounter = 89;
    }

    public override void GraphicsUpdate(Player player, float animationTimer)
    {

    }
}
