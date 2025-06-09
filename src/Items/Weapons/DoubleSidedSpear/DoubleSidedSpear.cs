/*
using RWCustom;
using UnityEngine;

namespace SlugCrafting.Weapons;

public class DoubleSidedSpear : Spear
{
    public AbstractDoubleSidedSpear abstractDoubleSidedSpear;

    public Spear backSpear;

    public float skewerLength = 20f;
    public float skewerEnterAngle = 0.7f;

    public bool skeweringActive = false;
    public readonly List<BodyChunk> skeweredChunks = new();

    public DoubleSidedSpear(AbstractPhysicalObject abstractPhysicalObject, World world)
        : base(abstractPhysicalObject, world)
    {

    }

    public override void Update(bool eu)
    {
        base.Update(eu);

        DoubleSidedSpearBackUpdate(backSpear, eu);
        DoubleSidedSpearBackSkewerUpdate(backSpear, eu);
    }

    public void DoubleSidedSpearBackSkewerUpdate(Spear spear, bool eu)
    {
        // TODO: give custom sprite for these kinda spear traps.

        // If we are not in the skewering mode, do nothing.
        if (!skeweringActive)
            return;

        //
        // LOOK FOR NEW CHUNKS TO SKEWER
        //

        // REMEMBER ROTATION OF SPEARS ARE SAVED AS DIRECTION OF -1 to 1 UP AND DOWN FOR SOME REASON.
        // CONFUSED ME BAD TIME AT FIRST.
        Vector2 impaleDir = new Vector2(spear.rotation.x, spear.rotation.y);

        // Offset is from center chunk.
        var offsetX = impaleDir.x * skewerLength;
        var offsetY = impaleDir.y * skewerLength;

        var chunk = spear.firstChunk;
        // Get the hitbox area of the spear's enter pointy end.
        Vector2 lineStartPos = new Vector2(chunk.pos.x + offsetX, chunk.pos.y + offsetY);
        Vector2 lineEndPos = new Vector2(chunk.pos.x - offsetX, chunk.pos.y - offsetY);

        var collisionResult = SharedPhysics.TraceProjectileAgainstBodyChunks(null, spear.room, lineStartPos, ref lineEndPos, 1f, 1, spear, false);
        BodyChunk? intersectedBodyChunk = collisionResult.chunk;

        if (intersectedBodyChunk != null && !skeweredChunks.Contains(intersectedBodyChunk))
        {
            // Check to see if the direction the chunk is moving is enough to count as into the spear.
            float velocityAgainstImpaleDirDot = Vector2.Dot(intersectedBodyChunk.vel, impaleDir);
            if (velocityAgainstImpaleDirDot >= skewerEnterAngle)
                spear.SkewerInCreature(collisionResult, eu);
        }

        //
        // Custom skewered code left behind in favor of using the built in spearing code.
        //

        /*
        for (int i = 0; i < spearData.skeweredChunks.Count; i++)
        {
            // TODO: add sound loop for skewered chunks moving making the pole slide sound

            // Get the dot product for how much the intersected body chunk is moving in the direction of the spear.
            var velocityIntoImpaleDirDot = Vector2.Dot(-impaleDir, spearData.skeweredChunks[i].vel);

            velocityIntoImpaleDirDot += 1; // Bring it from -1 to 1, to 0 to 2 range.
            velocityIntoImpaleDirDot /= 2; // Bring it from 0 to 2, to 0 to 1 range.

            // Force update the chunks position after it's update
            spearData.skeweredChunks[i].MoveFromOutsideMyUpdate(eu, chunk.pos);

            // Make it so that the chunk can only move in the direction the spear is sticking.
            spearData.skeweredChunks[i].vel *= impaleDir;

            // Make it harder to move the chunk if it is moving into the spear.
            var finalSkeweredChunkVelocityModifier = spearData.chunkSkeweredVelocityModifier + (velocityIntoImpaleDirDot * spearData.chunkMovingIntoSkewerVelocityModifier);
            finalSkeweredChunkVelocityModifier = Mathf.Clamp(finalSkeweredChunkVelocityModifier, 0.1f, 1f); // Can bug the hellll out if velocity starts multiplying greater than 1 lol.

            spearData.skeweredChunks[i].vel *= finalSkeweredChunkVelocityModifier;
            // Limit the rotation chunk as well so that the chunk visually looks like it doesn't want to rotate.
            //spearData.skeweredChunks[i].rotationChunk.vel *= finalSkeweredChunkVelocityModifier/2;

            // TODO: add functionality for scavs and scugs to hold onto the spear that is impaling them if they are impaled.
        }
        */
/*
    }

    private static void DoubleSidedSpearBackUpdate(Spear backSpear, bool eu)
    {
        var backSpearData = backSpear.GetSlugCraftingData();
        var frontSpear = backSpearData.oppositeSidedSpear;

        //-- Basically just copy allll the stuff of the front spear.

        for (int i = 0; i < backSpear.bodyChunks.Count(); i++)
        {
            backSpear.bodyChunks[i].pos = frontSpear.bodyChunks[i].pos;
            backSpear.bodyChunks[i].lastPos = frontSpear.bodyChunks[i].lastPos;
        }

        backSpear.lastPivotAtTip = frontSpear.lastPivotAtTip;
        backSpear.pivotAtTip = frontSpear.pivotAtTip;

        backSpear.lastRotation = frontSpear.lastRotation * -1f;
        backSpear.rotation = frontSpear.rotation * -1f;

        backSpear.lastMode = frontSpear.lastMode;
        backSpear.mode = frontSpear.mode;

        backSpear.stuckInWall = frontSpear.stuckInWall;

        //-- Always forbidden to player.
        backSpear.forbiddenToPlayer = 1;
    }

    //
    // SPRITE DRAWING
    //

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        // base.InitiateSprites(sLeaser, rCam);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        //-- Only run if not the back of a double sided spear,
        // as double sided spear front will override updates if it is enabled instead.
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        
    }
}
*/