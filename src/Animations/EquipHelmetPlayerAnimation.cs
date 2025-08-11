/*
namespace SlugCrafting.Animations;

public class EquipHelmetPlayerAnimation : SwallowPlayerAnimation
{
    public int helmetHand = 0;

    public int reachTopTime = 0;

    public override void Update(Player player, float animationTimer)
    {
        base.Update(player, animationTimer);
    }

    public override void GraphicsUpdate(Player player, float animationTimer)
    {
        var headChunk = player.firstChunk;
        var playerGraphics = player.graphicsModule as PlayerGraphics;
        var helmetChunk = player.grasps[helmetHand].grabbedChunk;

        var maxUpPos = headChunk.pos + new Vector2(0, helmetChunk.rad);
        Vector2 helmetShouldBePos;

        if (animationTimer > reachTopTime)
        {
            helmetShouldBePos = Mathf.Lerp(maxUpPos, headChunk.pos, animationTimer / length);
        }

        foreach (SlugcatHand hand in playerGraphics.hands)
        {
            hand.pos = Vector2.Lerp(hand.pos, playerGraphics.drawPositions[0, 0], animationTimer / 25f);
        }
    }
}
*/