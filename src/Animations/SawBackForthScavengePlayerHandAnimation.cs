using RWCustom;
using UnityEngine;

using MRCustom.Math;
using MRCustom.Animations;

using SlugCrafting.Items.Weapons;

namespace SlugCrafting.Animations;

public class SawBackForthScavengePlayerHandAnimation : MRAnimation<Player>
{
    /// <summary>
    /// How fast the saw moves back and forth.
    /// Smaller num = faster.
    /// </summary>
    public float timeBetweenSaws = 20f;

    protected PlayerGraphics playerGraphics;
    protected PlayerCraftingData playerCraftingData;

    public SawBackForthScavengePlayerHandAnimation(float length)
    {
        this.Length = length;
    }

    public override void Start(Player player)
    {
        base.Start(player);

        this.owner = player;
        this.playerGraphics = (PlayerGraphics)player.graphicsModule;
        this.playerCraftingData = player.GetPlayerCraftingData();
    }

    public override void Stop(Player player)
    {

    }

    public override void Update(int animationTimer)
    {

    }

    public override void GraphicsUpdate(int animationTimer)
    {
        var player = playerGraphics.player;
        var playerCraftingData = owner.GetPlayerCraftingData();
        // TODO: spawn the sparks and stuff that occasionanly fly off corpse
        var scavengingChunk = player.grasps[playerCraftingData.creatureGraspUsed].grabbedChunk;
        var graspedSaw = player.grasps[playerCraftingData.knifeGraspUsed].grabbed;

        //
        // SAW MOTION X CALCULATION
        //

        var sawMotionPosX = scavengingChunk.pos.x;

        // Saw will align back and forth from center based off the scavenge timer.
        var sawAlignmentFromCenterX = MarMathf.InverseLerpNegToPos(0, timeBetweenSaws, playerCraftingData.scavengeTimer %= timeBetweenSaws);

        // Saw motion X then moves back and forth the chunks rad based off timer.
        sawMotionPosX += sawAlignmentFromCenterX * scavengingChunk.rad;

        //
        // SAW MOTION Y CALCULATION
        //

        var sawMotionPosY = scavengingChunk.pos.y + scavengingChunk.rad; // Starts at top of body chunk

        var sawProgress = Mathf.InverseLerp(0, playerCraftingData.currentTargetedScavenge.scavengeTime, playerCraftingData.scavengeTimer);
        sawMotionPosY -= sawProgress * scavengingChunk.rad * 2; // Slowly moves down to bottom of body chunk.

        //
        // ACTUAL SETTAGE
        //

        var sawMotionPos = new Vector2(sawMotionPosX, sawMotionPosY);
        playerGraphics.hands[playerCraftingData.knifeGraspUsed].reachingForObject = true;
        playerGraphics.hands[playerCraftingData.knifeGraspUsed].absoluteHuntPos = sawMotionPos;

        //
        // SET KNIFE ROTATION
        //

        if (graspedSaw is Knife)
        {
            var graspedKnife = graspedSaw as Knife;

            // Flip if the grabber is facing left.
            float knifeAnimRotationX;
            if (graspedSaw.firstChunk.pos.x < playerGraphics.player.mainBodyChunk.pos.x)
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