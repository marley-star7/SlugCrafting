using RWCustom;
using UnityEngine;

using SlugCrafting.Items;

namespace SlugCrafting;

/// <summary>
/// A class for a hitbox that has functionality to skewer body chunks.
/// </summary>
public class Skewer : UpdatableAndDeletable
{
    /*
    /// <summary>
    /// The list of body chunks that are currently skewered.
    /// </summary>
    public List<BodyChunk> skeweredChunks = new();

    //
    // TODO: later add more directions for skewers, all 360.
    // Current problem is I don't know how to make optimal math checking a rectangle rotated

    public Vector2 pos;
    /// <summary>
    /// Currently only horizontal or vertical.
    ///
    /// REMEMBER ROTATION OF SPEARS AND THEREFORE THIS ARE SAVED AS DIRECTION OF -1 to 1 UP AND DOWN FOR SOME REASON.
    /// CONFUSED ME BAD TIME AT FIRST.
    ///
    /// </summary>
    private Vector2 rotation;

    public bool horizontal
    {
        get { return rotation.x != 0; }
        set
        {
            if (value)
                rotation = new Vector2(1, 0);
            else
                rotation = new Vector2(0, 1);
        }
    }

    /// <summary>
    /// Wether or not the skewer is active, and will skewer things / update.
    /// </summary>
    public bool active = false;

    private float _skewerLengthHalf = 10f;
    /// <summary>
    /// How long the skewer is in direction pointing.
    /// Middle of the skewer will be half of this value in total length.
    /// Exists secretly internally as halfed for micro-optimization purposes.
    /// </summary>
    public float skewerLength
    {
        get { return _skewerLengthHalf * 2; }
        set { _skewerLengthHalf = value / 2; }
    }

    private float _skewerWidthHalf = 5f;
    /// <summary>
    /// How wide the hitbox for the skewer is.
    /// Exists secretly internally as halfed for micro-optimization purposes.
    /// </summary>
    public float skewerWidth
    {
        get { return _skewerWidthHalf * 2; }
        set { _skewerWidthHalf = value / 2; }
    }

    public float chunkVelocityModifier = 0.7f;
    public float chunkMovingIntoSkewerVelocityModifier = 0.3f;

    public Skewer(Room room, Vector2 pos)
    {
        this.room = room;
        this.pos = pos;
    }

    public override void Update(bool eu)
    {
        // TODO: add sound for the impale
        // TODO: give custom sprite for these kinda spear traps.

        if (!active)
            return;

        // TODO: there is probably a more optimal way to do this, without recalculating the pos ever update, do that later.
        var skewerWidthHalf = skewerWidth / 2;
        var skewerLengthHalf = skewerLength / 2;

        // Bottom Left
        Vector2 hitboxPos;
        // Top Right
        Vector2 hitBoxEnd;
        if (horizontal)
        {
            hitboxPos = new Vector2(
                pos.x - skewerWidthHalf, // Bottom left X
                pos.y - skewerLengthHalf // Bottom left Y
            );
            hitBoxEnd = new Vector2(
                pos.x + skewerWidthHalf, // Top right X
                pos.y + skewerLengthHalf // Top right Y
            );
        }
        else
        {
            hitboxPos = new Vector2(
                pos.x - skewerLengthHalf, // Bottom left X
                pos.y - skewerWidthHalf // Bottom left Y
            );
            hitBoxEnd = new Vector2(
                pos.x + skewerLengthHalf, // Top right X
                pos.y + skewerWidthHalf // Top right Y
            );
        }

        var intersectedBodyChunk = RoomBodyChunkInRange(room, hitboxPos, hitBoxEnd);

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

    private static BodyChunk RoomBodyChunkInRange(in Room room, in Vector2 bottomLeft, in Vector2 topRight)
    {
        return RoomBodyChunkInRange(
            room,
            bottomLeft.x,
            topRight.x,
            bottomLeft.y,
            topRight.y
        );
    }

    // TODO: maybe move this function to MRCustom.
    private static BodyChunk RoomBodyChunkInRange(in Room room, in float leftX, in float rightX, in float bottomY, in float topY)
    {
        BodyChunk bodyChunkIntersected = null;

        for (int i = 0; i < room.abstractRoom.creatures.Count; i++)
        {
            if (room.abstractRoom.creatures[i].realizedCreature == null || room.abstractRoom.creatures[i].realizedCreature.room != room)
            {
                continue;
            }

            // Don't you just love having to custom check for hitboxes?
            // Check all of the creatures in the room, and their body chunks for being in the area of the spear trap.
            for (int j = 0; j < room.abstractRoom.creatures[i].realizedCreature.bodyChunks.Length; j++)
            {
                // Skip if body chunks pos is not in range.
                if (!Custom.InRange(
                    room.abstractRoom.creatures[i].realizedCreature.bodyChunks[j].pos.x,
                    leftX,
                    rightX
                    ) || !Custom.InRange(
                    room.abstractRoom.creatures[i].realizedCreature.bodyChunks[j].pos.y,
                    bottomY,
                    topY
                    )
                )
                {
                    continue;
                }

                return bodyChunkIntersected = room.abstractRoom.creatures[i].realizedCreature.bodyChunks[j];
            }
        }
        return bodyChunkIntersected;
    }
    */
}