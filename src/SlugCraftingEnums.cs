using MRCustom.Animations;
using SlugCrafting.Animations;

namespace SlugCrafting;

public static class SlugCraftingEnums
{
    public static readonly SlugcatStats.Name Crafter = new(nameof(Crafter), false);

    public class AbstractObjectType
    {
        public static readonly AbstractPhysicalObject.AbstractObjectType Knife = new("Knife", true);
        
        public static readonly AbstractPhysicalObject.AbstractObjectType SpiderSilkString = new("SpiderSilkString", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType LanternMouseString = new("LanternMouseString", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType Cord = new("Cord", true);

        public static readonly AbstractPhysicalObject.AbstractObjectType LizardShell = new("LizardShell", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType GreenLizardShell = new("GreenLizardShell", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType PinkLizardShell = new("PinkLizardShell", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType BlueLizardShell = new("BlueLizardShell", true);

        public static readonly AbstractPhysicalObject.AbstractObjectType GreenLizardShellHelmet = new("GreenLizardShellHelmet", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType PinkLizardShellHelmet = new("PinkLizardShellHelmet", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType BlueLizardShellHelmet = new("BlueLizardShellHelmet", true);

        public static readonly AbstractPhysicalObject.AbstractObjectType LizardHideBackpack = new("LizardHideBackpack", true);

        public static readonly AbstractPhysicalObject.AbstractObjectType KingVultureSpear = new("KingVultureSpear", true);
    }

    public class SandboxID
    {
        public static readonly MultiplayerUnlocks.SandboxUnlockID Knife = new("Knife", true);

        public static readonly MultiplayerUnlocks.SandboxUnlockID SpiderSilkString = new("SpiderSilkString", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID LanternMouseString = new("LanternMouseString", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID Cord = new("Cord", true);

        public static readonly MultiplayerUnlocks.SandboxUnlockID GreenLizardShell = new("GreenLizardShell", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID PinkLizardShell = new("PinkLizardShell", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID BlueLizardShell = new("BlueLizardShellHelmet", true);

        public static readonly MultiplayerUnlocks.SandboxUnlockID GreenLizardShellHelmet = new("GreenLizardShellHelmet", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID PinkLizardShellHelmet = new("PinkLizardShellHelmet", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID BlueLizardShellHelmet = new("BlueLizardShellHelmet", true);

        public static readonly MultiplayerUnlocks.SandboxUnlockID LizardHideBackpack = new("LizardHideBackpack", true);

        public static readonly MultiplayerUnlocks.SandboxUnlockID KingVultureSpear = new("KingVultureSpear", true);
    }

    public class PlayerHandAnimations
    {
        public static PlayerHandAnimationPlayer.AnimationIndex DoubleSwallow = new PlayerHandAnimationPlayer.AnimationIndex("DoubleSwallow", true);
        public static PlayerHandAnimationPlayer.AnimationIndex SmashIntoCraft = new PlayerHandAnimationPlayer.AnimationIndex("SmashIntoCraft", true);
        public static PlayerHandAnimationPlayer.AnimationIndex SawBackForthScavenge = new PlayerHandAnimationPlayer.AnimationIndex("SawBackForthScavenge", true);

        public static PlayerHandAnimationPlayer.AnimationIndex BiteStruggleNutLeftHand = new PlayerHandAnimationPlayer.AnimationIndex("BiteStruggleNutLeftHand", true);
        public static PlayerHandAnimationPlayer.AnimationIndex BiteStruggleNutRightHand = new PlayerHandAnimationPlayer.AnimationIndex("BiteStruggleNutRightHand", true);

        public static PlayerHandAnimationPlayer.AnimationIndex KnapSpearFirstHit = new PlayerHandAnimationPlayer.AnimationIndex("KnapSpearFirstHit", true);
        public static PlayerHandAnimationPlayer.AnimationIndex KnapSpearLoop = new PlayerHandAnimationPlayer.AnimationIndex("KnapSpearLoop", true);
        public static PlayerHandAnimationPlayer.AnimationIndex KnapSpearBreak = new PlayerHandAnimationPlayer.AnimationIndex("KnapSpearBreak", true);
        public static PlayerHandAnimationPlayer.AnimationIndex ImpaleOnSpear = new PlayerHandAnimationPlayer.AnimationIndex("ImpaleOnSpear", true);

        internal static void RegisterValues()
        {
            PlayerHandAnimationPlayer.defaultPlayerHandAnimationLibrary.RegisterAnimation(DoubleSwallow,
                new DoubleSwallowPlayerAnimation()
                {
                    length = 105,
                }
            );

            var knapSpearFirstHitAnimation = new BashObjectPlayerAnimation()
            {
                length = 21,
                impactTime = 20f,
                sinBeatingCurveStartRad = 0.9f,
                flinchAndLookAway = false,
                loop = true,

                fullRiseHandOffsetPos = new Vector2(13f, 9f),
                fullDescentHandOffsetPos = new Vector2(-8f, -17f),
            };
            knapSpearFirstHitAnimation.AddSignalEvent(BashObjectPlayerAnimation.impactSignalEvent, AnimationSignalEvents.OnKnapSpearImpact);

            PlayerHandAnimationPlayer.defaultPlayerHandAnimationLibrary.RegisterAnimation(KnapSpearFirstHit, knapSpearFirstHitAnimation);

            var knapSpearAnimation = new BashObjectPlayerAnimation()
            {
                length = 21,
                impactTime = 20f,
                sinBeatingCurveStartRad = 0.7f,
                flinchAndLookAway = false,
                loop = true,

                fullRiseHandOffsetPos = new Vector2(13f, 9f),
                fullDescentHandOffsetPos = new Vector2(-8f, -17f),
            };
            knapSpearAnimation.AddSignalEvent(BashObjectPlayerAnimation.impactSignalEvent, AnimationSignalEvents.OnKnapSpearImpact);

            PlayerHandAnimationPlayer.defaultPlayerHandAnimationLibrary.RegisterAnimation(KnapSpearLoop, knapSpearAnimation);

            var knapSpearBreakAnimation = new BashObjectPlayerAnimation()
            {
                length = 21,
                impactTime = 20f,
                sinBeatingCurveStartRad = 0.7f,
                flinchAndLookAway = true,
                loop = true,

                fullRiseHandOffsetPos = new Vector2(13f, 9f),
                fullDescentHandOffsetPos = new Vector2(-8f, -17f),
            };
            knapSpearBreakAnimation.AddSignalEvent(BashObjectPlayerAnimation.impactSignalEvent, AnimationSignalEvents.OnKnapSpearBreakImpact);

            PlayerHandAnimationPlayer.defaultPlayerHandAnimationLibrary.RegisterAnimation(KnapSpearBreak, knapSpearBreakAnimation);

            var biteStruggleNutLeftHandAnimation = new BiteStruggleAnimation()
            {
                length = 60,
                hand = 0
            };
            biteStruggleNutLeftHandAnimation.AddSignalEvent(biteStruggleNutLeftHandAnimation.animationFinishedSignalEvent, AnimationSignalEvents.OnBiteStruggleNutFinish);
            PlayerHandAnimationPlayer.defaultPlayerHandAnimationLibrary.RegisterAnimation(BiteStruggleNutLeftHand, biteStruggleNutLeftHandAnimation);

            var biteStruggleNutRightHandAnimation = new BiteStruggleAnimation()
            {
                length = 60,
                hand = 1
            };
            biteStruggleNutRightHandAnimation.AddSignalEvent(biteStruggleNutRightHandAnimation.animationFinishedSignalEvent, AnimationSignalEvents.OnBiteStruggleNutFinish);
            PlayerHandAnimationPlayer.defaultPlayerHandAnimationLibrary.RegisterAnimation(BiteStruggleNutRightHand, biteStruggleNutRightHandAnimation);
            /*
            PlayerHandAnimationPlayer.defaultPlayerHandAnimationLibrary.RegisterAnimation(ImpaleOnSpear,
                new SmashIntoCraftPlayerHandAnimation()
                {
                    length = 100,
                    primaryHandWeaponSetRotation = new Vector2(1, 1).normalized,
                    secondaryHandWeaponSetRotation = new Vector2(1, 1).normalized
                }
            );
            */
        }
    }
}
