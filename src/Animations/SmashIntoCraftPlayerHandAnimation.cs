using SlugCrafting;
using UnityEngine;

using MRCustom.Math;
using MRCustom.Animations;

namespace SlugCrafting.Animations;

public class SmashIntoCraftPlayerHandAnimation : PlayerHandAnimation
{

    // TODO: definitely probably need to do classes for this sort of thing, that way can store data inbetween instead of re-calculating every time.
    // TODO: set up a system so that can play sounds and count down the sounds, probably do this alongside the craft junk.
    // TODO: base it off the chunk position rather than hand.
    // TODO: add real noise to this, so you can sit there like a caveman with a noisemaker.

    public Vector2 fullRiseHandOffsetPos = new Vector2(13f, 9f);
    public Vector2 fullDescentHandOffsetPos = new Vector2(-8f, -17f);

    /// <summary>
    /// Also technically speed which you beat objects.
    /// </summary>
    public float timeBetweenBeats = 20f;
    public float sinBeatingCurveStartRad = 0.7f; // How high up to start the sine curve, set a bit low for that little raise until crashing back down the hand.

    private float extraBeatTime = 0f;
    private float numOfTimesToBeat = 0f;

    protected Player player;
    protected PlayerGraphics playerGraphics;
    protected PlayerCraftingData playerCraftingData;

    public SmashIntoCraftPlayerHandAnimation(float length) 
        : base(length)
    {
        float extraBeatTime = Length % timeBetweenBeats;
        float numOfTimesToBeat = (Length - extraBeatTime) / timeBetweenBeats;
    }

    public override void Play(Player player)
    {
        this.player = player;
        this.playerGraphics = (PlayerGraphics)player.graphicsModule;
        this.playerCraftingData = player.GetCraftingData();
    }

    public override void Update(Player player)
    {

    }

    public override void GraphicsUpdate(PlayerGraphics playerGraphics)
    {
        float animLength = playerCraftingData.currentPossibleCraft.Value.craftTime;
        float animTimer = playerCraftingData.craftTimer;

        int beatedObjectIndex = 0;
        int beatingObjectIndex = 1;

        float timeThisBeat = timeBetweenBeats;
        // If we are on the last beat, add extra time.
        bool isLastBeat = animTimer >= (animLength - (timeBetweenBeats + extraBeatTime));
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

        beatingHand.pos = Vector2.Lerp(playerGraphics.drawPositions[0, 0] + fullRiseHandOffsetPos, beatedHand.pos, beatingHandProgress);
        beatedHand.pos = Vector2.Lerp(playerGraphics.drawPositions[0, 0] + fullDescentHandOffsetPos + directionBeating * beatingKnockback, playerGraphics.drawPositions[0, 0] + fullDescentHandOffsetPos, beatedObjectKnockbackProgress);
    }
}