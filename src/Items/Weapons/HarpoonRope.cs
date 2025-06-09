using RWCustom;
using UnityEngine;

namespace SlugCrafting.Items.Weapons;

//
// HARPOON ROPE
//

public class HarpoonRope
{
    private IntVector2[] _cachedRtList = new IntVector2[100];

    public List<PlacedObject> noSpearStickZones;

    public float onRopePos = 1f;

    public int attachedTime;

    public Vector2 pos;
    public Vector2 lastPos;
    public Vector2 vel;

    public Creature.Grasp grasp;
    public Spear spear;

    public int tongueNum;

    public Vector2 spearStuckPos;
    public BodyChunk attachedChunk;

    public float myMass = 0.1f;

    public bool returning;

    public float requestedRopeLength;
    public float idealRopeLength;
    public float baseIdealRopeLength;

    public float minRopeLength;
    public float maxRopeLength;

    public float elastic;

    public bool disableStick;

    public Rope rope;

    public Vector2 AttachedPos => spearStuckPos;

    public float totalRope => 200f;

    public HarpoonRope(Spear spear)
    {
        this.spear = spear;
        idealRopeLength = 20f;
        minRopeLength = 0f;
        maxRopeLength = 170f;
        rope = new Rope(spear.room, -new Vector2(50f, 0), pos, 1f);
        rope.Reset();
        noSpearStickZones = new List<PlacedObject>();
    }

    public void decreaseRopeLength(float amount)
    {
        idealRopeLength = Mathf.Clamp(idealRopeLength - amount, minRopeLength, maxRopeLength);
    }

    public void increaseRopeLength(float amount)
    {
        idealRopeLength = Mathf.Clamp(idealRopeLength + amount, minRopeLength, maxRopeLength);
    }

    public void resetRopeLength()
    {
        idealRopeLength = baseIdealRopeLength;
    }

    /*
    public void NewRoom(Room newRoom)
    {
        resetRopeLength();
        rope = new Rope(newRoom, pos - new Vector2(50f, 0), pos, 1f);
        noSpearStickZones.Clear();

        for (int i = 0; i < newRoom.roomSettings.placedObjects.Count; i++)
        {
            if (newRoom.roomSettings.placedObjects[i].type == PlacedObject.Type.NoSpearStickZone)
            {
                noSpearStickZones.Add(newRoom.roomSettings.placedObjects[i]);
            }
        }
    }
    */

    public void Update()
    {
        _ = pos;
        lastPos = pos;
        pos = spear.firstChunk.pos;
        if (grasp.grabber.firstChunk != null)
        {
            rope.Update(grasp.grabber.firstChunk.pos, pos);
        }
    }

    private float GetTargetZeroGVelo(float baseV, float incV)
    {
        if (baseV < 0f)
        {
            return Mathf.Max(baseV + incV, -4f);
        }
        return Mathf.Min(baseV + incV, 4f);
    }

    /*
    private void Elasticity()
    {
        float num = 1f;
        Vector2 vector = Custom.DirVec(baseChunk.pos, rope.AConnect);
        float totalLength = rope.totalLength;
        float a = 0.7f;
        if (player.tongue.Attached)
        {
            a = Custom.LerpMap(Mathf.Abs(0.5f - onRopePos), 0.5f, 0.4f, 1.1f, 0.7f);
        }
        float num2 = RequestRope() * Mathf.Lerp(a, 1f, elastic);
        float num3 = Mathf.Lerp(0.85f, 0.25f, elastic);
        if (totalLength > num2)
        {
            baseChunk.vel += vector * (totalLength - num2) * num3 * num;
            baseChunk.pos += vector * (totalLength - num2) * num3 * num * Mathf.Lerp(1f, 0.5f, elastic);
            vector = Custom.DirVec(pos, rope.BConnect);
        }
    }
    */

    /*
    // TODO: add functionality for creatures to jump off the vine.

    public SimpleSegment[] segments;

    private Spear owner;

    private BodyChunk attachmentChunk;

    private float angle;

    private float side;

    private float connectionRad = 12f;

    public int timeSinceWhiskersBeingClimbed;

    public float startRadius = 1.5f;

    public float endRadius = 0.1f;

    private Vector2 GlobalAttachmentPos
    {
        get
        {
            Vector2 rotation = attachmentChunk.Rotation;
            Vector2 vector = new Vector2(0f - rotation.y, rotation.x);
            return attachmentChunk.pos + vector * (attachmentChunk.rad - 5f) * Mathf.Sin(1 * (float)Math.PI / 2f + side * (float)Math.PI / 2f);
        }
    }

    public float BackgroundFactor => Mathf.InverseLerp(-0.1f, -0.4f, Mathf.Cos(1 * (float)Math.PI / 2f + side * (float)Math.PI / 2f));

    public bool InBackground => BackgroundFactor > 0f;

    public HarpoonRope(Spear owner, int segments, BodyChunk attachmentChunk, float angle, float side)
    {
        this.owner = owner;
        this.segments = new SimpleSegment[segments];
        this.attachmentChunk = attachmentChunk;
        this.angle = angle;
        this.side = side;
        Reset();
    }

    public bool hasCreatureAttached()
    {
        return timeSinceWhiskersBeingClimbed <= 1;
    }

    public void Reset()
    {
        Vector2 globalAttachmentPos = GlobalAttachmentPos;
        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].Reset(globalAttachmentPos);
        }
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
        if (room != owner.room)
        {
            room?.climbableVines.vines.Remove(this);
            RemoveFromRoom();
            return;
        }
        timeSinceWhiskersBeingClimbed++;
        Vector2 rotation = attachmentChunk.Rotation;
        Vector2 vector = new Vector2(0f - rotation.y, rotation.x) * Mathf.Sin(1 * (float)Math.PI / 2f + (float)Math.PI / 2f * side);
        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].vel.y -= 0.9f * room.gravity;
            float num = (float)i / (float)(segments.Length - 1);
            segments[i].vel = Vector2.Lerp(segments[i].vel, attachmentChunk.vel, num * 0.1f);
            segments[i].vel += (vector - rotation * (1f + angle)) * num;
            segments[i].lastPos = segments[i].pos;
            segments[i].pos += segments[i].vel;
            segments[i].vel *= 0.95f;
            if (i > 0 && (room.aimap == null || room.aimap.getTerrainProximity(segments[i].pos) < 3))
            {
                SharedPhysics.TerrainCollisionData cd2 = SharedPhysics.VerticalCollision(cd: new SharedPhysics.TerrainCollisionData(segments[i].pos, segments[i].lastPos, segments[i].vel, 2f, new IntVector2(0, 0), goThroughFloors: true), room: room);
                cd2 = SharedPhysics.HorizontalCollision(room, cd2);
                segments[i].pos = cd2.pos;
                segments[i].vel = cd2.vel;
                if (cd2.contactPoint.x != 0)
                {
                    segments[i].vel.y *= 0.6f;
                }
                if (cd2.contactPoint.y != 0)
                {
                    segments[i].vel.x *= 0.6f;
                }
            }
        }
        ConnectEnds();
        for (int num2 = segments.Length - 1; num2 > 0; num2--)
        {
            ConnectSegments(num2, num2 - 1);
        }
        ConnectEnds();
        for (int j = 1; j < segments.Length; j++)
        {
            ConnectSegments(j, j - 1);
        }
        ConnectEnds();
    }

    private void ConnectEnds()
    {
        segments[0].pos = GlobalAttachmentPos;
        segments[0].vel = attachmentChunk.vel;
    }

    private void ConnectSegments(int A, int B)
    {
        Vector2 normalized = (segments[A].pos - segments[B].pos).normalized;
        float num = Vector2.Distance(segments[A].pos, segments[B].pos);
        float num2 = Mathf.InverseLerp(0f, connectionRad, num);
        if (num > connectionRad)
        {
            segments[A].pos += normalized * (connectionRad - num) * 0.5f * num2;
            segments[A].vel += normalized * (connectionRad - num) * 0.5f * num2;
            segments[B].pos -= normalized * (connectionRad - num) * 0.5f * num2;
            segments[B].vel -= normalized * (connectionRad - num) * 0.5f * num2;
        }
    }

    public void BeingClimbedOn(Creature crit)
    {
        timeSinceWhiskersBeingClimbed = 0;
    }

    public bool CurrentlyClimbable()
    {
        return true;
    }

    public int TotalPositions()
    {
        return segments.Length;
    }

    public float Rad(int index)
    {
        return 0.1f;
    }

    public float Mass(int index)
    {
        return 0.04f;
    }

    public Vector2 Pos(int index)
    {
        return segments[index].pos;
    }

    /// <summary>
    /// Push the segment in movement rotated by the rotation.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="movement"></param>
    /// <param name="rotation"></param>
    public void SwingPush(int index, Vector2 movement, Vector2 rotation)
    {
        var swingVel = Custom.rotateVectorDeg(movement, rotation.GetAngle());
        segments[index].vel += swingVel;
    }

    public void Push(int index, Vector2 movement)
    {
        segments[index].vel += movement;
    }
    */
}
