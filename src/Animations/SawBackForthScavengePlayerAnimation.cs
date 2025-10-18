
namespace SlugCrafting.Animations;

public class SawBackForthScavengePlayerHandAnimation : RWAnimation<Player>
{
    /// <summary>
    /// How fast the saw moves back and forth.
    /// Smaller num = faster.
    /// </summary>
    public float timeBetweenSaws = 20f;

    public int knifeHand = 0;

    public SawBackForthScavengePlayerHandAnimation(int knifeHand)
    {
        this.knifeHand = knifeHand;
    }

    public override void Start(Player player)
    {

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
        var scavengingChunk = player.grasps[MarPlayerExtensions.GetOtherGrasp(knifeHand)].grabbedChunk;
        var graspedSaw = player.grasps[knifeHand].grabbed;

        //
        // SAW MOTION X CALCULATION
        //

        var sawMotionPosX = scavengingChunk.pos.x;

        // Saw will align back and forth from center based off the scavenge timer.
        var sawAlignmentFromCenterX = MarMathf.InverseLerpNegToPos(0, timeBetweenSaws, animationTimer %= timeBetweenSaws);

        // Saw motion X then moves back and forth the chunks rad based off timer.
        sawMotionPosX += sawAlignmentFromCenterX * scavengingChunk.rad;

        //
        // SAW MOTION Y CALCULATION
        //

        var sawMotionPosY = scavengingChunk.pos.y + scavengingChunk.rad; // Starts at top of body chunk

        var sawProgress = Mathf.InverseLerp(0, length, animationTimer);
        sawMotionPosY -= sawProgress * scavengingChunk.rad * 2; // Slowly moves down to bottom of body chunk.

        //
        // ACTUAL SETTAGE
        //

        var sawMotionPos = new Vector2(sawMotionPosX, sawMotionPosY);
        playerGraphics.hands[knifeHand].reachingForObject = true;
        playerGraphics.hands[knifeHand].absoluteHuntPos = sawMotionPos;

        //
        // SET KNIFE ROTATION
        //

        if (graspedSaw is Weapon graspedWeapon)
        {
            // Flip if the grabber is facing left.
            float knifeAnimRotationX;
            if (graspedSaw.firstChunk.pos.x < playerGraphics.player.mainBodyChunk.pos.x)
                knifeAnimRotationX = 90f;
            else
                knifeAnimRotationX = -90;

            float knifeFlipDir = Mathf.Sign(MarMathf.InverseLerpNegToPos(-90, 90, knifeAnimRotationX));

            float knifeAnimRotationY = Custom.DirVec(playerGraphics.player.mainBodyChunk.pos, sawMotionPos).y;
            knifeAnimRotationY += 40 * knifeFlipDir; // Rotate it a bit back to make it look like a saw.

            graspedWeapon.setRotation = new Vector2(knifeAnimRotationX, knifeAnimRotationY);
        }
    }
}