using MRCustom.Animations;
using SlugCrafting.Animations;

namespace SlugCrafting;

public static class SlugCraftingEnums
{
    public static readonly SlugcatStats.Name Crafter = new(nameof(Crafter), false);

    public static class ProcessID
    {
        public static readonly ProcessManager.ProcessID ShelterCraft = new("ShelterCraft", true);
    }

    public static class AbstractObjectType
    {
        public static readonly AbstractPhysicalObject.AbstractObjectType Knife = new("Knife", true);
        
        public static readonly AbstractPhysicalObject.AbstractObjectType SpiderSilkString = new("SpiderSilkString", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType LanternMouseString = new("LanternMouseString", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType Cord = new("Cord", true);

        public static readonly AbstractPhysicalObject.AbstractObjectType LizardHeadShell = new("LizardHeadShell", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType GreenLizardHeadShell = new("GreenLizardHeadShell", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType PinkLizardHeadShell = new("PinkLizardHeadShell", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType BlueLizardHeadShell = new("BlueLizardHeadShell", true);

        public static readonly AbstractPhysicalObject.AbstractObjectType GreenLizardShellHelmet = new("GreenLizardShellHelmet", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType PinkLizardShellHelmet = new("PinkLizardShellHelmet", true);
        public static readonly AbstractPhysicalObject.AbstractObjectType BlueLizardShellHelmet = new("BlueLizardShellHelmet", true);

        public static readonly AbstractPhysicalObject.AbstractObjectType LizardHideBackpack = new("LizardHideBackpack", true);

        public static readonly AbstractPhysicalObject.AbstractObjectType KingVultureSpear = new("KingVultureSpear", true);
    }

    public static class SandboxUnlockID
    {
        public static readonly MultiplayerUnlocks.SandboxUnlockID Knife = new("Knife", true);

        public static readonly MultiplayerUnlocks.SandboxUnlockID SpiderSilkString = new("SpiderSilkString", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID LanternMouseString = new("LanternMouseString", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID Cord = new("Cord", true);

        public static readonly MultiplayerUnlocks.SandboxUnlockID GreenLizardHeadShell = new("GreenLizardHeadShell", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID PinkLizardHeadShell = new("PinkLizardHeadShell", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID BlueLizardHeadShell = new("BlueLizardShellHelmet", true);

        public static readonly MultiplayerUnlocks.SandboxUnlockID GreenLizardShellHelmet = new("GreenLizardShellHelmet", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID PinkLizardShellHelmet = new("PinkLizardShellHelmet", true);
        public static readonly MultiplayerUnlocks.SandboxUnlockID BlueLizardShellHelmet = new("BlueLizardShellHelmet", true);

        public static readonly MultiplayerUnlocks.SandboxUnlockID LizardHideBackpack = new("LizardHideBackpack", true);

        public static readonly MultiplayerUnlocks.SandboxUnlockID KingVultureSpear = new("KingVultureSpear", true);
    }

    public static class PlayerHandAnimations
    {
        public static PlayerHandAnimationPlayer.AnimationIndex DoubleSwallow = new PlayerHandAnimationPlayer.AnimationIndex("DoubleSwallow", true);
        public static PlayerHandAnimationPlayer.AnimationIndex SmashIntoCraft = new PlayerHandAnimationPlayer.AnimationIndex("SmashIntoCraft", true);

        public static PlayerHandAnimationPlayer.AnimationIndex SawBackForthUsingLeftHand = new PlayerHandAnimationPlayer.AnimationIndex("SawBackForthUsingLeftHand", true);
        public static PlayerHandAnimationPlayer.AnimationIndex SawBackForthUsingRightHand = new PlayerHandAnimationPlayer.AnimationIndex("SawBackForthUsingRightHand", true);

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

            // --- Knap Spear Animations --- //
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

            // --- Bite Struggle --- //

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

            // --- Saw Animations --- //

            var sawBackForthUsingLeftHandAnimation = new SawBackForthScavengePlayerHandAnimation(1)
            {
                length = 60,
            };
            PlayerHandAnimationPlayer.defaultPlayerHandAnimationLibrary.RegisterAnimation(SawBackForthUsingLeftHand, sawBackForthUsingLeftHandAnimation);

            var sawBackForthUsingRightHandAnimation = new SawBackForthScavengePlayerHandAnimation(0)
            {
                length = 60,
            };
            PlayerHandAnimationPlayer.defaultPlayerHandAnimationLibrary.RegisterAnimation(SawBackForthUsingRightHand, sawBackForthUsingRightHandAnimation);

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

    public static class CraftAnimationSets
    {
        public static Craft.Animation[] DefaultSawBackForthUsingLeftHandAnimations = new Craft.Animation[]
        {
            new Craft.Animation(1, SlugCraftingEnums.PlayerHandAnimations.SawBackForthUsingLeftHand)
        };
        public static Craft.Animation[] DefaultSawBackForthUsingRightHandAnimations = new Craft.Animation[]
        {
            new Craft.Animation(1, SlugCraftingEnums.PlayerHandAnimations.SawBackForthUsingRightHand)
        };
    }

    public static class CraftIngredients
    {
        public static CraftIngredient Knife = new CraftIngredient(SlugCraftingEnums.AbstractObjectType.Knife);

        public static CraftIngredient GreenLizardHeadShell = new CraftIngredient(SlugCraftingEnums.AbstractObjectType.GreenLizardHeadShell);
        public static CraftIngredient PinkLizardHeadShell = new CraftIngredient(SlugCraftingEnums.AbstractObjectType.PinkLizardHeadShell);

        public static CraftIngredient GreenLizardHead = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Head, CreatureTemplate.Type.GreenLizard);
        public static CraftIngredient GreenLizardBody = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Body, CreatureTemplate.Type.GreenLizard);

        public static CraftIngredient PinkLizardHead = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Head, CreatureTemplate.Type.PinkLizard);
        public static CraftIngredient PinkLizardBody = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Body, CreatureTemplate.Type.PinkLizard);

        public static CraftIngredient BlueLizardHead = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Head, CreatureTemplate.Type.BlueLizard);
        public static CraftIngredient BlueLizardBody = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Body, CreatureTemplate.Type.BlueLizard);

        public static CraftIngredient WhiteLizardHead = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Head, CreatureTemplate.Type.WhiteLizard);
        public static CraftIngredient WhiteLizardBody = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Body, CreatureTemplate.Type.WhiteLizard);

        public static CraftIngredient YellowLizardHead = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Head, CreatureTemplate.Type.YellowLizard);
        public static CraftIngredient YellowLizardBody = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Body, CreatureTemplate.Type.YellowLizard);

        public static CraftIngredient BlackLizardHead = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Head, CreatureTemplate.Type.BlackLizard);
        public static CraftIngredient BlackLizardBody = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Body, CreatureTemplate.Type.BlackLizard);

        public static CraftIngredient RedLizardHead = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Head, CreatureTemplate.Type.RedLizard);
        public static CraftIngredient RedLizardBody = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Body, CreatureTemplate.Type.RedLizard);

        public static CraftIngredient CyanLizardHead = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Head, CreatureTemplate.Type.CyanLizard);
        public static CraftIngredient CyanLizardBody = new CraftIngredient(AbstractPhysicalObject.AbstractObjectType.Creature, CreatureBodyChunkIndex.Lizard.Body, CreatureTemplate.Type.CyanLizard);
    }
}
