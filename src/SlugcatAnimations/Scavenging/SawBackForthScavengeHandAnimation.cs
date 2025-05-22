/*
using RWCustom;
using UnityEngine;

using MRCustom.Animations;
using MRCustom.Math;

using SlugCrafting.Items;

namespace SlugCrafting.Animations.Slugcat;

public class SawBackForthScavengeHandAnimation : MRAnimation<Player>
{
    public int sawGraspIndex = 0;
    public int creatureGraspIndex = 1;

    public SawBackForthScavengeHandAnimation(string name)
    {
        _name = name;
    }

    string _name;
    string MRAnimation<Player>.Name
    {
        get { return _name; }
        set { }
    }

    public float GetLength(Player player)
    {
        return player.GetCraftingData().currentTargetedScavenge.scavengeTime;
    }

    public void Start(Player player)
    {
    }

    public void Stop(Player owner)
    {

    }

    public void Update(Player player, int animationTime)
    {
        // TODO: spawn the sparks and stuff that occasionanly fly off corpse
        var sawSpeed = 20;
        var playerGraphics = player.graphicsModule as PlayerGraphics;
        var scavengingChunk = player.grasps[creatureGraspIndex].grabbedChunk;
        var graspedSaw = player.grasps[sawGraspIndex].grabbed;

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

        var sawProgress = Mathf.InverseLerp(0, GetLength(player), animationTime);
        sawMotionPosY -= sawProgress * scavengingChunk.rad * 2; // Slowly moves down to bottom of body chunk.

        //
        // ACTUAL SETTAGE
        //

        var sawMotionPos = new Vector2(sawMotionPosX, sawMotionPosY);
        playerGraphics.hands[sawGraspIndex].reachingForObject = true;
        playerGraphics.hands[sawGraspIndex].absoluteHuntPos = sawMotionPos;

        //
        // SET KNIFE ROTATION
        //

        if (graspedSaw is Knife)
        {
            var graspedKnife = graspedSaw as Knife;

            // Flip if the grabber is facing left.
            float knifeAnimRotationX;
            if (graspedSaw.firstChunk.pos.x < player.mainBodyChunk.pos.x)
                knifeAnimRotationX = 90f;
            else
                knifeAnimRotationX = -90;

            float knifeFlipDir = Mathf.Sign(MarMathf.InverseLerpNegToPos(-90, 90, knifeAnimRotationX));

            float knifeAnimRotationY = Custom.DirVec(player.mainBodyChunk.pos, sawMotionPos).y;
            knifeAnimRotationY += 40 * knifeFlipDir; // Rotate it a bit back to make it look like a saw.

            graspedKnife.setRotation = new Vector2(knifeAnimRotationX, knifeAnimRotationY);
        }
    }

    public void UpdateGraphics(Player player, int animationTime)
    {

    }
}
*/