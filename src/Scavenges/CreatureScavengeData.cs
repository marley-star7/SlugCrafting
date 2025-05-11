using UnityEngine;

using System.Runtime.CompilerServices;

namespace SlugCrafting.Scavenges;

/// <summary>
/// A struct that contains the position of a scavenge spot.
/// Based off of body chunk, and accompanying position values.
/// </summary>
public struct ScavengeSpot
{
    public int bodyChunkIndex;
    public Vector2 pos;

    public ScavengeSpot(int bodyChunkIndex, int posX, int posY)
    {
        this.bodyChunkIndex = bodyChunkIndex;
        this.pos = new Vector2(posX, posY);
    }

    public ScavengeSpot(int bodyChunkIndex, Vector2 pos)
    {
        this.bodyChunkIndex = bodyChunkIndex;
        this.pos = pos;
    }

    /// <summary>
    /// If pos and index are the same, then they are considered equal.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(ScavengeSpot a, ScavengeSpot b)
    {
        if (a.bodyChunkIndex != b.bodyChunkIndex)
            return false;
        if (a.pos != b.pos)
            return false;

        return true;
    }

    public static bool operator !=(ScavengeSpot a, ScavengeSpot b)
    {
        if (a.bodyChunkIndex == b.bodyChunkIndex)
            return false;
        if (a.pos == b.pos)
            return false;

        return true;
    }
}

public abstract class CreatureScavengeData
{
    /// <summary>
    /// The key value is the location to reach that scavenge spot via player's directional presses.
    /// 0,0 is the default starting spot.
    /// </summary>
    public Dictionary<ScavengeSpot, AbstractPhysicalObjectScavenge> ScavengeSpots;

    /// <summary>
    /// The creature refrence to this scavenge data.
    /// </summary>
    public Creature creature;

    /// <summary>
    /// Currently will not work if overriden, this is due to the way the factory process works.
    /// If I knew how to fix it I would, but I don't.
    /// </summary>
    /// <param name="creature"></param>
    public CreatureScavengeData(Creature creature)
    {
        this.creature = creature;
        RealizeScavengeData();
    }

    /// <summary>
    /// Runs on inital construction of object.
    /// Set scavenge data here.
    /// </summary>
    protected virtual void RealizeScavengeData()
    {

    }

    public AbstractPhysicalObjectScavenge GetScavenge(ScavengeSpot ScavengeSpot)
    {
        return ScavengeSpots.TryGetValue(ScavengeSpot, out var spot) ? spot : null;
    }

    public ScavengeSpot GetNearestValidScavengeSpot(ScavengeSpot fromSpot)
    {
        ScavengeSpot closestSpot = fromSpot;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < ScavengeSpots.Count; i++)
        {
            var checkingSpot = ScavengeSpots.Keys.ElementAt(i);
            var checkingSpotDistance = Vector2.Distance(
                creature.bodyChunks[fromSpot.bodyChunkIndex].pos + fromSpot.pos, 
                creature.bodyChunks[checkingSpot.bodyChunkIndex].pos + checkingSpot.pos
            );

            if (checkingSpotDistance < bestDistance && ScavengeSpots[checkingSpot].canScavenge)
            {
                closestSpot = checkingSpot;
                bestDistance = checkingSpotDistance;
            }
        }
        return closestSpot;
    }

    public AbstractPhysicalObjectScavenge GetNearestValidScavenge(ScavengeSpot fromSpot)
    {
        var closestSpot = GetNearestValidScavengeSpot(fromSpot);
        if (closestSpot == fromSpot)
            return GetScavenge(fromSpot);
        else
            return GetScavenge(closestSpot);
    }

    public virtual AbstractPhysicalObject Scavenge(ScavengeSpot scavengeSpot)
    {
        return null;
    }
}

public static class CreatureExtension
{
    private static readonly ConditionalWeakTable<Creature, CreatureScavengeData> conditionalWeakTable = new();

    public static CreatureScavengeData GetCreatureScavengeData(this Creature creature)
    {
        return conditionalWeakTable.GetValue(creature, _ => SlugCrafting.Core.Content.CreateScavengeData(creature, creature.Template.type));
    }
}
