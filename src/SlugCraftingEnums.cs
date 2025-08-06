using MRCustom.Animations;
using SlugCrafting.Animations;

namespace SlugCrafting;

public static class SlugCraftingEnums
{
    public static readonly SlugcatStats.Name Crafter = new(nameof(Crafter), false);

    public class AbstractObjectType
    {
        public static readonly AbstractPhysicalObject.AbstractObjectType Knife = new("Knife", true);

        public static readonly AbstractPhysicalObject.AbstractObjectType GreenLizardShell = new("GreenLizardShell", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType PinkLizardShell = new("PinkLizardShell", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType BlueLizardShell = new("BlueLizardShell", true);

        public static readonly AbstractPhysicalObject.AbstractObjectType GreenLizardShellHelmet = new("GreenLizardShellHelmet", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType PinkLizardShellHelmet = new("PinkLizardShellHelmet", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType BlueLizardShellHelmet = new("BlueLizardShellHelmet", true);
    }

    public class SandboxID
    {
        public static readonly MultiplayerUnlocks.SandboxUnlockID Knife = new("Knife", true);

        public static readonly MultiplayerUnlocks.SandboxUnlockID GreenLizardShell = new("GreenLizardShell", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID PinkLizardShell = new("PinkLizardShell", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID BlueLizardShell = new("BlueLizardShellHelmet", true);

        public static readonly MultiplayerUnlocks.SandboxUnlockID GreenLizardShellHelmet = new("GreenLizardShellHelmet", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID PinkLizardShellHelmet = new("PinkLizardShellHelmet", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID BlueLizardShellHelmet = new("BlueLizardShellHelmet", true);
    }

    public class PlayerHandAnimations
    {
        public static PlayerHandAnimationPlayer.AnimationIndex DoubleSwallowCraft = new PlayerHandAnimationPlayer.AnimationIndex("DoubleSwallowCraft", true);
        public static PlayerHandAnimationPlayer.AnimationIndex SmashIntoCraft = new PlayerHandAnimationPlayer.AnimationIndex("SmashIntoCraft", true);
        public static PlayerHandAnimationPlayer.AnimationIndex SawBackForthScavenge = new PlayerHandAnimationPlayer.AnimationIndex("SawBackForthScavenge", true);

        public static PlayerHandAnimationPlayer.AnimationIndex KnapSpear = new PlayerHandAnimationPlayer.AnimationIndex("KnapSpear", true);
        public static PlayerHandAnimationPlayer.AnimationIndex ImpaleOnSpear = new PlayerHandAnimationPlayer.AnimationIndex("ImpaleOnSpear", true);

        internal static void RegisterValues()
        {
            PlayerHandAnimationPlayer.defaultPlayerHandAnimationLibrary.RegisterAnimation(DoubleSwallowCraft,
                new SwallowCraftPlayerHandAnimation()
                {
                    length = 50,
                }
            );

            PlayerHandAnimationPlayer.defaultPlayerHandAnimationLibrary.RegisterAnimation(KnapSpear,
                new SmashIntoCraftPlayerHandAnimation()
                {
                    length = 200,
                    timeBetweenBeats = 20f,
                    sinBeatingCurveStartRad = 0.7f,

                    beatSound = SoundID.Spear_Bounce_Off_Wall,
                    breakSound = SoundID.Spear_Fragment_Bounce,

                    fullRiseHandOffsetPos = new Vector2(13f, 9f),
                    fullDescentHandOffsetPos = new Vector2(-8f, -17f),
                }
            );

            PlayerHandAnimationPlayer.defaultPlayerHandAnimationLibrary.RegisterAnimation(ImpaleOnSpear,
                new SmashIntoCraftPlayerHandAnimation()
                {
                    length = 100,
                    primaryHandWeaponSetRotation = new Vector2(1, 1).normalized,
                    secondaryHandWeaponSetRotation = new Vector2(1, 1).normalized
                }
            );
        }
    }
}
