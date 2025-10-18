using Noise;

namespace SlugCrafting.Animations;

public class BashObjectPlayerAnimation : RWAnimation<Player>
{
    // TODO: base it off the chunk position rather than hand.
    // TODO: add secret functionality to hit in time with the music lol
    // TODO: it'd be fun as well to find a way to be able to hit it on command, so you can play music. xd

    /// <summary>
    /// The offset of rotation during the animation.
    /// </summary>
    public Vector2? primaryHandWeaponSetRotation = null;
    /// <summary>
    /// The offset of rotation during the animation.
    /// </summary>
    public Vector2? secondaryHandWeaponSetRotation = null;

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
    public float impactTime = 30f;
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

    public static StringName impactSignalEvent = new StringName("Impact");

    public bool flinchAndLookAway = false;

    private float extraBeatTime = 0f;
    private float numOfTimesToBeat = 0f;

    protected PlayerGraphics playerGraphics;
    protected PlayerCraftingData playerCraftingData;

    public override void Start(Player player)
    {
        this.playerGraphics = (PlayerGraphics)player.graphicsModule;
        this.playerCraftingData = player.GetPlayerCraftingData();

        extraBeatTime = length % impactTime;
        numOfTimesToBeat = (length - extraBeatTime) / impactTime;
    }

    public override void Stop(Player player)
    {

    }

    public override void Update(Player player, float animTimer)
    {
		var playerCraftingData = player.GetPlayerCraftingData();
		var playerGraphics = player.graphicsModule as PlayerGraphics;

		if (playerGraphics == null)
			return;

		if (impactTime == animTimer)
            EmitSignal(impactSignalEvent, player);

        // Look down at the thing your smacking bruh.
        if (flinchAndLookAway)
        {
			playerGraphics.LookAtPoint(playerGraphics.drawPositions[0, 0] + fullRiseHandOffsetPos, 42069); // Hell yea
			player.eyesClosedTime = eyesCloseOnBreakLength;
		}
        else
            playerGraphics.LookAtPoint(playerGraphics.drawPositions[0, 0] + fullDescentHandOffsetPos, 0.1f); // Very low interest, look at if there is nothing else.
    }

    public override void GraphicsUpdate(Player player, float animTimer)
    {
		var playerCraftingData = player.GetPlayerCraftingData();
		var playerGraphics = player.graphicsModule as PlayerGraphics;

		if (playerGraphics == null)
			return;

		int beatedObjectIndex = 0;
        int beatingObjectIndex = 1;

        float timeThisBeat = impactTime;
        // If we are on the last beat, add extra time.
        bool isLastBeat = animTimer >= (length - (impactTime + extraBeatTime));
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

        Vector2 directionBeating = (fullRiseHandOffsetPos - fullDescentHandOffsetPos).normalized;
        float beatingKnockback = 2.5f;

        var fullRiseHandPos = playerGraphics.drawPositions[0, 0] + fullRiseHandOffsetPos;
        var fullDescentHandPos = playerGraphics.drawPositions[0, 0] + fullDescentHandOffsetPos;

        beatingHand.pos = Vector2.Lerp(fullRiseHandPos, beatedHand.pos, beatingHandProgress);
        beatedHand.pos = Vector2.Lerp(fullDescentHandPos + directionBeating * beatingKnockback, fullDescentHandPos, beatedObjectKnockbackProgress);

        //-- MS7 TODO: just move this stuff down here to a different animation? that copies the data from smashIntoCraft, to remove the miniscule overhead (but moreso set a standard).
        if (playerGraphics.player.grasps[0].grabbed is Weapon && primaryHandWeaponSetRotation != null)
        {
            var primaryHandWeapon = (Weapon)playerGraphics.player.grasps[0].grabbed;
            primaryHandWeapon.rotation = primaryHandWeaponSetRotation.Value;
        }

        if (playerGraphics.player.grasps[1].grabbed is Weapon && secondaryHandWeaponSetRotation != null)
        {
            var secondaryHandWeapon = (Weapon)playerGraphics.player.grasps[1].grabbed;
            secondaryHandWeapon.rotation = secondaryHandWeaponSetRotation.Value;
        }
    }
}