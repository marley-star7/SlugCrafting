using RWCustom;
using UnityEngine;

using MarMath;

using SlugCrafting.Items;

namespace SlugCrafting.Animations.Scavenging;

public class SawBackForthScavengeAnimation : HandAnimation
{
    int animationLength;

    public SawBackForthScavengeAnimation(int animationLength)
    {
        this.animationLength = animationLength;
    }

    public override void PlaySlugcatAnimation(Player scug, int sawGraspIndex, int objectGraspIndex, int animationTime)
    {
        // TODO: spawn the sparks and stuff that occasionanly fly off corpse
        var sawSpeed = 20;
        var scugGraphics = scug.graphicsModule as PlayerGraphics;
        var scavengingChunk = scug.grasps[objectGraspIndex].grabbedChunk;
        var graspedSaw = scug.grasps[sawGraspIndex].grabbed;

        //
        // SAW MOTION X CALCULATION
        //

        var sawMotionPosX = scavengingChunk.pos.x;

        // Saw will align back and forth from center based off the scavenge timer.
        var sawAlignmentFromCenterX = MarMathf.InverseLerpNegToPos(0, sawSpeed, animationTime %= sawSpeed);

        // Saw motion X then moves back and forth the chunks rad based off timer.
        sawMotionPosX += sawAlignmentFromCenterX * scavengingChunk.rad;

        //
        // SAW MOTION Y CALCULATION
        //

        var sawMotionPosY = scavengingChunk.pos.y + scavengingChunk.rad; // Starts at top of body chunk

        var sawProgress = Mathf.InverseLerp(0, animationLength, animationTime);
        sawMotionPosY -= sawProgress * scavengingChunk.rad * 2; // Slowly moves down to bottom of body chunk.

        //
        // ACTUAL SETTAGE
        //

        var sawMotionPos = new Vector2(sawMotionPosX, sawMotionPosY);
        scugGraphics.hands[sawGraspIndex].reachingForObject = true;
        scugGraphics.hands[sawGraspIndex].absoluteHuntPos = sawMotionPos;

        //
        // SET KNIFE ROTATION
        //

        if (graspedSaw is Knife)
        {
            var graspedKnife = graspedSaw as Knife;

            // Flip if the grabber is facing left.
            float knifeAnimRotationX;
            if (graspedSaw.firstChunk.pos.x < scug.mainBodyChunk.pos.x)
                knifeAnimRotationX = 90f;
            else
                knifeAnimRotationX = -90;

            float knifeFlipDir = Mathf.Sign(MarMathf.InverseLerpNegToPos(-90, 90, knifeAnimRotationX));

            float knifeAnimRotationY = Custom.DirVec(scug.mainBodyChunk.pos, sawMotionPos).y;
            knifeAnimRotationY += 40 * knifeFlipDir; // Rotate it a bit back to make it look like a saw.

            graspedKnife.setRotation = new Vector2(knifeAnimRotationX, knifeAnimRotationY);
        }
    }
}