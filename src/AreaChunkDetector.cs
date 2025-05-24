using RWCustom;
using UnityEngine;

namespace SlugCrafting;

/// <summary>
/// Used to detect body chunks entering a rectangular area.
/// Functionally similar to like a hitbox of sorts.
/// </summary>
public class AreaChunkDetector : UpdatableAndDeletable
{
    /*
    /// <summary>
    /// Wether or not the skewer is active, and will skewer things / update.
    /// </summary>
    public bool active = false;

    /// <summary>
    /// The top left corner of the 2d area.
    /// </summary>
    Vector2 pos;
    /// <summary>
    /// The bottom right corner of the 2d area.
    /// </summary>
    Vector2 end;

    // TODO: to get proper rotation, probably base it off velocity of things passing through.
    // TODO: use hitbox functionality for some updator thing for good optimization.
    // TODO: check that one spear function for how it does the hitbox
    // TODO: seems like shared phyiscs TraceProjectileagainstBodyChunk is what we are looking for.
    // TODO: might not need to implement hitbox functionallity rn afterall.

    public override void Update(bool eu)
    {
        if (!active)
            return;

        var intersectedBodyChunk = RoomBodyChunkInRange(room, pos, end);

        // If found new body chunk, and it is not already skewered by the spear, add it.
        if (intersectedBodyChunk != null && !skeweredChunks.Contains(intersectedBodyChunk))
        {
            AddSkeweredChunk(intersectedBodyChunk);
        }
    }

    private static BodyChunk RoomBodyChunkInRange(in Room room, in Vector2 topLeft, in Vector2 bottomRight)
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
