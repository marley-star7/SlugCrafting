using RWCustom;
using UnityEngine;
using Noise;

using MRCustom.Math;
using MRCustom.Animations;

using SlugCrafting;
using SlugCrafting.Crafts;

namespace SlugCrafting.Animations;

public class SmashIntoCraftPlayerHandAnimation : MRAnimation<Player>
{
    // TODO: base it off the chunk position rather than hand.
    // TODO: add secret functionality to hit in time with the music lol
    // TODO: it'd be fun as well to find a way to be able to hit it on command, so you can play music. xd

    /// <summary>
    /// The position the hand will be when it is fully raised.
    /// </summary>
    public Vector2 fullRiseHandOffsetPos = new Vector2(13f, 9f);
    /// <summary>
    /// The position the hand will be when it is fully down on the object.
    /// </summary>
    public Vector2 fullDescentHandOffsetPos = new Vector2(-8f, -17f);

    /// <summary>
    /// Also technically speed which you beat objects.
    /// </summary>
    public float timeBetweenBeats = 30f;
    /// <summary>
    /// How high up to start the sine curve, set a bit low for that little raise until crashing back down the hand.
    /// </summary>
    public float sinBeatingCurveStartRad = 0.7f;

    /// <summary>
    /// How long scug will close their eyes when the object breaks.
    /// </summary>
    public int eyesCloseOnBreakLength = 20;
    /// <summary>
    /// The time before eyes break.
    /// </summary>
    public int timeBeforeBreakEyesClose = 15;

    /// <summary>
    /// The sound that plays when the player beats the object.
    /// </summary>
    public SoundID beatSound;
    /// <summary>
    /// The sound that plays when the object finally breaks.
    /// </summary>
    public SoundID breakSound;

    private Queue<float> beatSoundTimes;

    private float extraBeatTime = 0f;
    private float numOfTimesToBeat = 0f;

    protected PlayerGraphics playerGraphics;
    protected PlayerCraftingData playerCraftingData;

    public SmashIntoCraftPlayerHandAnimation(float length)
    {
        this.Length = length;
    }

    public override void Start(Player player)
    {
        base.Start(player);

        this.owner = player;
        this.playerGraphics = (PlayerGraphics)player.graphicsModule;
        this.playerCraftingData = player.GetCraftingData();

        extraBeatTime = Length % timeBetweenBeats;
        numOfTimesToBeat = (Length - extraBeatTime) / timeBetweenBeats;

        // Add the times that the beat sound will play lol.
        beatSoundTimes = new Queue<float>();
        for (int i = 1; i < numOfTimesToBeat; i++)
        {
            beatSoundTimes.Enqueue(timeBetweenBeats * i);
        }
        // Add the last sound timer, which is the extra beat time.
        beatSoundTimes.Enqueue(timeBetweenBeats * numOfTimesToBeat + extraBeatTime);
    }

    public override void Stop(Player owner)
    {

    }

    public override void Update(int animTimer)
    {
        // Play da sounds when da sounds need to be played.
        if (beatSoundTimes.Peek() <= animTimer)
        {
            if (beatSoundTimes.Count > 1)
            {
                beatSoundTimes.Dequeue();
                // Play the beat sound.
                owner.room.PlaySound(beatSound, owner.firstChunk.pos);
                owner.room.InGameNoise(new InGameNoise(owner.firstChunk.pos, 2500f, owner, 1f));

                // Vibrate the spear if it is a spear for extra pizazz.
                if (owner.grasps[0].grabbed is Spear)
                    ((Spear)owner.grasps[0].grabbed).vibrate = 10;
            }
            // If we are on our last sound, play the break sound instead.
            else if (beatSoundTimes.Count > 0)
            {
                beatSoundTimes.Dequeue();
                // Final break sound which is much louder.
                owner.room.PlaySound(breakSound, owner.firstChunk.pos);
                owner.room.InGameNoise(new InGameNoise(owner.firstChunk.pos, 5000f, owner, 1f));
            }
        }

        // Look up and blink at the last break.
        if (animTimer > Length - timeBeforeBreakEyesClose)
        {
            playerGraphics.LookAtPoint(playerGraphics.drawPositions[0, 0] + fullRiseHandOffsetPos, 42069); // Hell yea
            owner.eyesClosedTime = eyesCloseOnBreakLength;
        }
        else // Look down at the thing your smacking bruh.
        {
            playerGraphics.LookAtPoint(playerGraphics.drawPositions[0, 0] + fullDescentHandOffsetPos, 0.1f); // Very low interest, look at if there is nothing else.
        }
    }

    public override void GraphicsUpdate(int animTimer)
    {
        int beatedObjectIndex = 0;
        int beatingObjectIndex = 1;

        float timeThisBeat = timeBetweenBeats;
        // If we are on the last beat, add extra time.
        bool isLastBeat = animTimer >= (Length - (timeBetweenBeats + extraBeatTime));
        if (isLastBeat)
            timeThisBeat += extraBeatTime;

        float currentBeatTimer = animTimer % timeThisBeat;
        float currentBeatProgress = Mathf.InverseLerp(0, timeThisBeat, currentBeatTimer); // 0 to 1

        //
        // BEATING HAND POS CALCULATION
        //

        float currentBeatingAngleProgress = Mathf.Clamp((currentBeatProgress * Mathf.PI), sinBeatingCurveStartRad, Mathf.PI);

        float beatingHandProgress = Math.Abs(Mathf.Sin(currentBeatingAngleProgress) - 1); // 0 to 1, 0 is hand full raised, 1 is hand full down.
        beatingHandProgress *= beatingHandProgress; // Squared for more of a curve.

        //
        // BEATED HAND POS CALCULATION
        //

        float beatedObjectKnockbackProgress = Mathf.InverseLerp(timeThisBeat / 4, 0, currentBeatTimer); // 0 to 1

        var beatingHand = playerGraphics.hands[beatingObjectIndex];
        var beatedHand = playerGraphics.hands[beatedObjectIndex];

        // For some reason this works better?
        //beatingHand.reachingForObject = true;
        //beatingHand.absoluteHuntPos = Vector2.Lerp(self.drawPositions[0, 0] + Vector2.up * 5, beatedHand.pos, currentBeatProgress);

        Vector2 directionBeating = (fullRiseHandOffsetPos - fullDescentHandOffsetPos).normalized;
        float beatingKnockback = 2.5f;

        var fullRiseHandPos = playerGraphics.drawPositions[0, 0] + fullRiseHandOffsetPos;
        var fullDescentHandPos = playerGraphics.drawPositions[0, 0] + fullDescentHandOffsetPos;

        beatingHand.pos = Vector2.Lerp(fullRiseHandPos, beatedHand.pos, beatingHandProgress);
        beatedHand.pos = Vector2.Lerp(fullDescentHandPos + directionBeating * beatingKnockback, fullDescentHandPos, beatedObjectKnockbackProgress);
    }
}