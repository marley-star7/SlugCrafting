using RWCustom;
using UnityEngine;

using SlugCrafting.Items;

namespace SlugCrafting;

/// <summary>
/// A class for a hitbox that has functionality to skewer body chunks.
/// </summary>
public class Skewer
{
    /*
    /// <summary>
    /// The list of body chunks that are currently skewered.
    /// </summary>
    public List<BodyChunk> skeweredChunks = new();

    //
    // TODO: later add more directions for skewers, all 360.
    // Current problem is I don't know how to make optimal math checking a rectangle rotated

    /// <summary>
    /// The start position of the skewer's detection area.
    /// </summary>
    public Vector2 startPos;
    /// <summary>
    /// The end position of the skewer's detection area.
    /// </summary>
    public Vector2 endPos;

    /// <summary>
    /// The thickness of the skewer detection area.
    /// </summary>
    private float _skewerRad = 1f;

    public float chunkVelocityModifier = 0.7f;
    public float chunkMovingIntoSkewerVelocityModifier = 0.3f;

    public void UpdateDetection(Room room, Vector2 startPos, )
    {
        var collisionResult = SharedPhysics.TraceProjectileAgainstBodyChunks(null, self.room, lineStartPos, ref lineEndPos, 1f, 1, self, false);
        var intersectedBodyChunk = collisionResult.chunk;

        // If found new body chunk, and it is not already skewered by the spear, add it.
        if (intersectedBodyChunk != null && !skeweredChunks.Contains(intersectedBodyChunk))
        {
            AddSkeweredChunk(intersectedBodyChunk);
        }

        for (int i = 0; i < skeweredChunks.Count; i++)
        {
            // Get the dot product for how much the intersected body chunk is moving in the direction of the spear.
            var velocityAgainstImpaleDirDot = Vector2.Dot(rotation, intersectedBodyChunk.vel);

            velocityAgainstImpaleDirDot += 1; // Bring it from -1 to 1, to 0 to 2 range.
            velocityAgainstImpaleDirDot /= 2; // Bring it from 0 to 2, to 0 to 1 range.

            // Force update the chunks position after it's update
            intersectedBodyChunk.MoveFromOutsideMyUpdate(eu, hitboxPos);

            // Make it so that the chunk can only move in the direction the spear is sticking.
            intersectedBodyChunk.vel *= rotation;

            // Make it harder to move the chunk if it is moving into the spear.
            intersectedBodyChunk.vel *= chunkVelocityModifier + (velocityAgainstImpaleDirDot * chunkMovingIntoSkewerVelocityModifier);
            //intersectedBodyChunk.rotationChunk.vel *= velocityResistanceImpaledMod * impaledChunkVelocityHinderance/2;
            //intersectedBodyChunk.owner.firstChunk.vel *= impaleDir * impaledChunkVelocityHinderance;

            // TODO: add functionality for scavs and scugs to hold onto the spear that is impaling them if they are impaled.
        }
    }

    public void AddSkeweredChunk(BodyChunk chunk)
    {
        if (skeweredChunks.Contains(chunk))
            return;

        skeweredChunks.Add(chunk);
    }

    //
    // TODO: maybe move these functions to MRCustom.
    //
    */
}