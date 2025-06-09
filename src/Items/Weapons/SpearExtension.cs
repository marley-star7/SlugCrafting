using RWCustom;
using UnityEngine;

using System.Runtime.CompilerServices;
using SlugCrafting.Crafts;
using MoreSlugcats;
using SlugCrafting.Items;

namespace SlugCrafting.Items.Weapons;

public class SpearCraftingData : PlayerCarryableItemCraftingData
{
    //
    // HARPOON DATA
    //

    public bool isHarpoon;

    public HarpoonRope harpoonRope;

    //
    // DOUBLE SIDED DATA
    //

    public enum SidedMode
    {
        SingleSided,
        DoubleSidedFront,
        DoubleSidedBack,
    }
    public SidedMode sidedMode = SidedMode.SingleSided;
    public Spear? oppositeSidedSpear = null;

    public readonly List<BodyChunk> skeweredChunks = new();
    public bool skeweringActive = false;
    public bool isControllingSpearSide = false;
    public float skewerLength = 20f;
    //public float chunkSkeweredVelocityModifier = 0.4f;
    //public float chunkMovingIntoSkewerVelocityModifier = 0.4f;

    /// <summary>
    /// The required angle the velocity of a chunk must be moving at least to enter.
    /// Based off a dot product calculation, so 1 is exactly same as the skewer direction, and 0 is perpendicular. 
    /// </summary>
    public float skewerEnterAngle = 0.8f;

    public WeakReference<Spear> spearRef;

    public SpearCraftingData(Spear spear) : base (spear)
    {
        spearRef = new WeakReference<Spear>(spear);
    }
    
    //
    // ATTACHED ITEM DATA
    //

    public SporePlant? sporePlant = null;
}

// TODO: later implement the more complex skewer functionality, for now this works ig.

public static class SpearExtension
{
    //
    // ATTACHED ITEMS UPDATE
    //

    public static void AttachSporePlant(this Spear spear, SporePlant sporePlant)
    {
        var SpearCraftingData = spear.GetSpearCraftingData();
        if (SpearCraftingData.sporePlant != null)
        {
            Plugin.Logger.LogMessage($"Spear {spear.abstractSpear.ID} already has a spore plant attached, cannot attach another one.");
            return; // Already has a spore plant attached.
        }

        SpearCraftingData.sporePlant = sporePlant;
        sporePlant.GetSporePlantCraftingData().stuckInSpear = spear;
    }

    public static void UnattachSporePlant(this Spear spear)
    {
        var SpearCraftingData = spear.GetSpearCraftingData();
        var sporePlant = SpearCraftingData.sporePlant;

        if (sporePlant == null)
            return;

        SpearCraftingData.sporePlant = null;
        sporePlant.GetSporePlantCraftingData().stuckInSpear = null;
    }

    /*
    public static void DeactivateAttachedSporePlant(this Spear spear)
    {
        var SpearCraftingData = spear.GetSpearCraftingData();
        var sporePlant = SpearCraftingData.sporePlant;

        if (sporePlant == null)
            return;

        //-- Make sure the spore plant is pacified and does not collide with the spear.
        sporePlant.ChangeCollisionLayer(2);
        sporePlant.Pacified = true;
    }

    public static void ActivateAttachedSporePlant(this Spear spear)
    {
        var SpearCraftingData = spear.GetSpearCraftingData();
        var sporePlant = SpearCraftingData.sporePlant;

        if (sporePlant == null)
            return;

        //-- Spore plant needs to be on default collision layer to detect creatures.
        sporePlant.ChangeCollisionLayer(sporePlant.DefaultCollLayer);
        sporePlant.Pacified = false;
    }
    */

    public static void AttachedSporePlantUpdate(this Spear spear)
    {
        var SpearCraftingData = spear.GetSpearCraftingData();

        if (SpearCraftingData.sporePlant == null)
            return;

        var sporePlant = SpearCraftingData.sporePlant;

        //
        // Make spore plant follow the spear.
        //

        var posAdjustmentUpSpear = spear.rotation * 8f;

        sporePlant.firstChunk.lastPos = spear.firstChunk.lastPos + posAdjustmentUpSpear;
        sporePlant.firstChunk.pos = spear.firstChunk.pos + posAdjustmentUpSpear;

        sporePlant.rotation = spear.rotation;
        sporePlant.lastRotation = spear.lastRotation;

        sporePlant.firstChunk.vel = spear.firstChunk.vel;

        //-- So player cannot pickup the spore plant from the spear.
        sporePlant.forbiddenToPlayer = 10;

        //- MR7: I threw this in the update function because for some reason otherwise the collision keeps changing on the spore plant to collide with spear.
        // I would like to do this a nicer way, but too lazy to find out how.
        sporePlant.CollideWithObjects = false;
        sporePlant.CollideWithTerrain = false;
        sporePlant.GoThroughFloors = true;

        if (spear.stuckInObject != null || spear.stuckInWall != null)
        {
            sporePlant.Pacified = false;
        }
        else
        {
            sporePlant.Pacified = true;
        }

        if (sporePlant.Pacified)
        {
            sporePlant.ChangeCollisionLayer(2);
        }
        else
        {
            sporePlant.ChangeCollisionLayer(sporePlant.DefaultCollLayer);
        }
    }
    
    //
    // DOUBLE SIDED SPEAR FUNCTIONS
    //

    public static void DoubleSidedSpearBackUpdate(this Spear backSpear, bool eu)
    {
        var backSpearCraftingData = backSpear.GetSpearCraftingData();
        var frontSpear = backSpearCraftingData.oppositeSidedSpear;

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
        backSpear.stuckRotation = frontSpear.stuckRotation * -1f;

        backSpear.lastMode = frontSpear.lastMode;
        backSpear.mode = frontSpear.mode;

        backSpear.stuckInWall = frontSpear.stuckInWall;

        //-- Always forbidden to player.
        backSpear.forbiddenToPlayer = 1;
    }

    public static void DoubleSidedSpearBackSkewerUpdate(this Spear spear, bool eu)
    {
        // TODO: give custom sprite for these kinda spear traps.

        var SpearCraftingData = spear.GetSpearCraftingData();
        // If we are not in the skewering mode, do nothing.
        if (!SpearCraftingData.skeweringActive)
            return;

        //
        // LOOK FOR NEW CHUNKS TO SKEWER
        //

        // REMEMBER ROTATION OF SPEARS ARE SAVED AS DIRECTION OF -1 to 1 UP AND DOWN FOR SOME REASON.
        // CONFUSED ME BAD TIME AT FIRST.
        Vector2 impaleDir = new Vector2(spear.rotation.x, spear.rotation.y);

        // Offset is from center chunk.
        var offsetX = impaleDir.x * SpearCraftingData.skewerLength;
        var offsetY = impaleDir.y * SpearCraftingData.skewerLength;

        var chunk = spear.firstChunk;
        // Get the hitbox area of the spear's enter pointy end.
        Vector2 lineStartPos = new Vector2(chunk.pos.x + offsetX, chunk.pos.y + offsetY);
        Vector2 lineEndPos = new Vector2(chunk.pos.x - offsetX, chunk.pos.y - offsetY);

        var collisionResult = SharedPhysics.TraceProjectileAgainstBodyChunks(null, spear.room, lineStartPos, ref lineEndPos, 1f, 1, spear, false);
        BodyChunk? intersectedBodyChunk = collisionResult.chunk;

        if (intersectedBodyChunk != null && !SpearCraftingData.skeweredChunks.Contains(intersectedBodyChunk))
        {
            // Check to see if the direction the chunk is moving is enough to count as into the spear.
            float velocityAgainstImpaleDirDot = Vector2.Dot(intersectedBodyChunk.vel, impaleDir);
            if (velocityAgainstImpaleDirDot >= SpearCraftingData.skewerEnterAngle)
                spear.SkewerInCreature(collisionResult, eu);
        }

        // TODO: make it so that it only checks for creatures, and one chunk of the creature
        for (int i = 0; i < SpearCraftingData.skeweredChunks.Count; i++)
        {
            SpearCraftingData.skeweredChunks[i].pos = lineStartPos;
            SpearCraftingData.skeweredChunks[i].setPos = lineStartPos;
            SpearCraftingData.skeweredChunks[i].vel = spear.firstChunk.vel;
        }

        //
        // Custom skewered code left behind in favor of using the built in spearing code.
        //

        /*
        for (int i = 0; i < SpearCraftingData.skeweredChunks.Count; i++)
        {
            // TODO: add sound loop for skewered chunks moving making the pole slide sound

            // Get the dot product for how much the intersected body chunk is moving in the direction of the spear.
            var velocityIntoImpaleDirDot = Vector2.Dot(-impaleDir, SpearCraftingData.skeweredChunks[i].vel);

            velocityIntoImpaleDirDot += 1; // Bring it from -1 to 1, to 0 to 2 range.
            velocityIntoImpaleDirDot /= 2; // Bring it from 0 to 2, to 0 to 1 range.

            // Force update the chunks position after it's update
            SpearCraftingData.skeweredChunks[i].MoveFromOutsideMyUpdate(eu, chunk.pos);

            // Make it so that the chunk can only move in the direction the spear is sticking.
            SpearCraftingData.skeweredChunks[i].vel *= impaleDir;

            // Make it harder to move the chunk if it is moving into the spear.
            var finalSkeweredChunkVelocityModifier = SpearCraftingData.chunkSkeweredVelocityModifier + (velocityIntoImpaleDirDot * SpearCraftingData.chunkMovingIntoSkewerVelocityModifier);
            finalSkeweredChunkVelocityModifier = Mathf.Clamp(finalSkeweredChunkVelocityModifier, 0.1f, 1f); // Can bug the hellll out if velocity starts multiplying greater than 1 lol.

            SpearCraftingData.skeweredChunks[i].vel *= finalSkeweredChunkVelocityModifier;
            // Limit the rotation chunk as well so that the chunk visually looks like it doesn't want to rotate.
            //SpearCraftingData.skeweredChunks[i].rotationChunk.vel *= finalSkeweredChunkVelocityModifier/2;

            // TODO: add functionality for scavs and scugs to hold onto the spear that is impaling them if they are impaled.
        }
        */
    }

    //
    // SKEWERING BACK SIDE SPEAR FUNCTIONALITY
    //

    /// <summary>
    /// Mostly built off of the base games LodgeInSpear function.
    /// But suited for skewering creatures.
    /// </summary>
    /// <param name="spear"></param>
    /// <param name="result"></param>
    /// <param name="eu"></param>
    public static void SkewerInCreature(this Spear spear, SharedPhysics.CollisionResult result, bool eu)
    {
        try
        {
            var SpearCraftingData = spear.GetSpearCraftingData();
            SpearCraftingData.skeweredChunks.Add(result.chunk);

            spear.stuckInObject = result.obj;
            spear.room.PlaySound(SoundID.Spear_Stick_In_Creature, result.chunk.pos);

            if (spear is ExplosiveSpear)
            {
                var explosiveSpear = spear as ExplosiveSpear;
                explosiveSpear.Ignite();
            }

            if (result.chunk != null)
            {
                var spearPosBeforeStuck = spear.firstChunk.pos;

                spear.ChangeMode(Weapon.Mode.StuckInCreature);
                spear.stuckInChunkIndex = result.chunk.index;
                spear.stuckInChunk.MoveFromOutsideMyUpdate(eu, spear.stuckInWall.Value);
                spear.stuckRotation = Custom.VecToDeg(spear.rotation);
                spear.stuckBodyPart = -1;
                spear.pinToWallCounter = 600; // Extra long for skewer traps.
                spear.firstChunk.MoveWithOtherObject(eu, spear.stuckInChunk, result.collisionPoint);

                Plugin.Logger.LogMessage("Skewered spear to creature BodyChunk");
                new AbstractPhysicalObject.AbstractSpearStick(spear.abstractPhysicalObject, (result.obj as Creature).abstractCreature, spear.stuckInChunkIndex, spear.stuckBodyPart, spear.stuckRotation);
            }
            else if (result.onAppendagePos != null)
            {
                spear.ChangeMode(Weapon.Mode.StuckInCreature);
                spear.stuckInChunkIndex = 0;
                spear.stuckInAppendage = result.onAppendagePos;
                spear.stuckRotation = Custom.VecToDeg(spear.rotation) - Custom.VecToDeg(spear.stuckInAppendage.appendage.OnAppendageDirection(spear.stuckInAppendage));

                Plugin.Logger.LogMessage("Skewered spear to creature Appendage");
                new AbstractPhysicalObject.AbstractSpearAppendageStick(spear.abstractPhysicalObject, (result.obj as Creature).abstractCreature, result.onAppendagePos.appendage.appIndex, result.onAppendagePos.prevSegment, result.onAppendagePos.distanceToNext, spear.stuckRotation);
            }
            if (spear.room.BeingViewed)
            {
                for (int i = 0; i < 8; i++)
                {
                    spear.room.AddObject(new WaterDrip(result.collisionPoint, -spear.firstChunk.vel * UnityEngine.Random.value * 0.5f + Custom.DegToVec(360f * UnityEngine.Random.value) * spear.firstChunk.vel.magnitude * UnityEngine.Random.value * 0.5f, waterColor: false));
                }
            }
        }
        catch (NullReferenceException ex)
        {
            spear.stuckInObject = null;
            Custom.LogWarning($"Spear skewer in creature failure. {ex} :: {spear.abstractSpear.pos} :: {result.obj} :: {spear.room.abstractRoom.name}");
        }
    }

    /*
    public static void FreeSkeweredChunk(this Spear spear, BodyChunk chunk)
    {
        var SpearCraftingData = spear.SlugCrafting();

        if (!SpearCraftingData.skeweredChunks.Contains(chunk))
            return;

        SpearCraftingData.skeweredChunks.Remove(chunk);
        spear.room.PlaySound(SoundID.Spear_Dislodged_From_Creature, chunk.pos);
    }
    */

    public static SpearCraftingData GetSpearCraftingData(this Spear physicalObject) => (SpearCraftingData)PlayerCarryableItemCraftingExtension.craftingDataConditionalWeakTable.GetValue(physicalObject, _ => new PlayerCarryableItemCraftingData(physicalObject));
}
