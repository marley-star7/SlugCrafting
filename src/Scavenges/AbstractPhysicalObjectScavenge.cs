
using System.Numerics;
using MRCustom.Animations;

namespace SlugCrafting;

// TODO: probably change this to be a struct for optimization purposes, since the only thing that changes is the function?
public abstract class AbstractPhysicalObjectScavenge
{
    /// <summary>
    /// The object that owns this scavenge spot.
    /// </summary>
    public PhysicalObject owner;

    /*
    /// <summary>
    /// The index of the body chunk that this scavenge spot is located on.
    /// </summary>
    public int bodyChunkIndex;
    /// <summary>
    /// The relative position of the center of the scavenge spot.
    /// Used in animations.
    /// </summary>
    public Vector2 posOffset;
    /// <summary>
    /// The size of the scavenge spot's area.
    /// Used in animations to decide how far to animate certain things.
    /// </summary>
    public Vector2 size;
    */

    /// <summary>
    /// The default time it takes to scavenge this spot.
    /// </summary>
    public int scavengeTime;

    public bool canScavenge;

    public PlayerHandAnimationData.HandAnimationIndex handAnimation;

    public AbstractPhysicalObjectScavenge(PhysicalObject owner)
    {
        this.owner = owner;

        scavengeTime = 1;
        canScavenge = true;
    }

    /// <summary>
    /// Returns a prepared abstractPhysicalObject scavenged from this spot to be spawned in the world.
    /// </summary>
    /// <returns></returns>
    public abstract AbstractPhysicalObject Scavenge();
}
