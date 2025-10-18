namespace SlugCrafting.Animations;

public class SharpenCraftPlayerHandAnimation : RWAnimation<Player>
{
    /// <summary>
    /// How fast the saw moves back and forth.
    /// Smaller num = faster.
    /// </summary>
    public float timeBetweenSaws = 20f;

    protected PlayerGraphics playerGraphics;
    protected PlayerCraftingData playerCraftingData;

    public int knifeGraspIndex;

    public override void Start(Player player)
    {
        this.playerGraphics = (PlayerGraphics)player.graphicsModule;
        this.playerCraftingData = player.GetPlayerCraftingData();
    }

    public override void Stop(Player player)
    {

    }

    public override void Update(Player player, float animationTimer)
    {

    }

    public override void GraphicsUpdate(Player player, float animationTimer)
    {
        var playerCraftingData = player.GetPlayerCraftingData();
        var playerGraphics = player.graphicsModule as PlayerGraphics;

        if (playerGraphics == null)
            return;

        // TODO: spawn the sparks and stuff that occasionanly fly off corpse
        var sharpeningChunk = player.grasps[0].grabbedChunk;
        var graspedSharpener = player.grasps[1].grabbed;

        //
        // SHARPEN MOTION X CALCULATION
        //

        var sawMotionPosX = sharpeningChunk.pos.x;

        // Saw will align back and forth from center based off the scavenge timer.
        var sawAlignmentFromCenterX = MarMathf.InverseLerpNegToPos(0, timeBetweenSaws, playerCraftingData.scavengeTimer %= timeBetweenSaws);

        // Saw motion X then moves back and forth the chunks rad based off timer.
        sawMotionPosX += sawAlignmentFromCenterX * sharpeningChunk.rad;

        //
        // SHARPEN MOTION Y CALCULATION
        //

        var sawMotionPosY = sharpeningChunk.pos.y + sharpeningChunk.rad; // Starts at top of body chunk

        var sawProgress = Mathf.InverseLerp(0, animationTimer, length);
        sawMotionPosY -= sawProgress * sharpeningChunk.rad * 2; // Slowly moves down to bottom of body chunk.

        //
        // ACTUAL SETTAGE
        //

        var sawMotionPos = new Vector2(sawMotionPosX, sawMotionPosY);
        playerGraphics.hands[knifeGraspIndex].reachingForObject = true;
        playerGraphics.hands[knifeGraspIndex].absoluteHuntPos = sawMotionPos;

        //
        // SET KNIFE ROTATION
        //

        if (graspedSharpener is Knife)
        {
            var graspedKnife = graspedSharpener as Knife;

            // Flip if the grabber is facing left.
            float knifeAnimRotationX;
            if (graspedSharpener.firstChunk.pos.x < playerGraphics.player.mainBodyChunk.pos.x)
                knifeAnimRotationX = 90f;
            else
                knifeAnimRotationX = -90;

            float knifeFlipDir = Mathf.Sign(MarMathf.InverseLerpNegToPos(-90, 90, knifeAnimRotationX));

            float knifeAnimRotationY = Custom.DirVec(playerGraphics.player.mainBodyChunk.pos, sawMotionPos).y;
            knifeAnimRotationY += 40 * knifeFlipDir; // Rotate it a bit back to make it look like a saw.

            graspedKnife.setRotation = new Vector2(knifeAnimRotationX, knifeAnimRotationY);
        }
    }
}