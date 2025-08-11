namespace SlugCrafting.Animations;

public class DoubleSwallowPlayerAnimation : SwallowPlayerAnimation
{
    public override void Update(Player player, float animationTimer)
    {
        base.Update(player, animationTimer);
    }

    public override void GraphicsUpdate(Player player, float animationTimer)
    {

        var playerGraphics = player.graphicsModule as PlayerGraphics;
        foreach (SlugcatHand hand in playerGraphics.hands)
        {
            hand.pos = Vector2.Lerp(hand.pos, playerGraphics.drawPositions[0, 0], animationTimer / 25f);
        }

        if (animationTimer > 30)
        {
            playerGraphics.blink = 5;
        }

        float animationProgress = Mathf.InverseLerp(0f, 110f, animationTimer);
        float num11 = animationTimer / Mathf.Lerp(30f, 15f, animationProgress);
        if (playerGraphics.player.standing)
        {
            playerGraphics.drawPositions[0, 0].y += Mathf.Sin(num11 * Mathf.PI * 2f) * animationProgress * 2f;
            playerGraphics.drawPositions[1, 0].y += -Mathf.Sin((num11 + 0.2f) * 3.1415927f * 2f) * animationProgress * 3f;
        }
        else
        {
            playerGraphics.drawPositions[0, 0].y += Mathf.Sin(num11 * Mathf.PI * 2f) * animationProgress * 3f;
            playerGraphics.drawPositions[0, 0].x += Mathf.Cos(num11 * Mathf.PI * 2f) * animationProgress * 1f;
            playerGraphics.drawPositions[1, 0].y += Mathf.Sin((num11 + 0.2f) * Mathf.PI * 2f) * animationProgress * 2f;
            playerGraphics.drawPositions[1, 0].x += -Mathf.Cos(num11 * 3.1415927f * 2f) * animationProgress * 3f;
        }
    }
}
