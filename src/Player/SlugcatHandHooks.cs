namespace SlugCrafting;

internal static class SlugcatHandHooks
{
    internal static bool SlugcatHand_EngageInMovement(On.SlugcatHand.orig_EngageInMovement orig, SlugcatHand self)
    {
        Player scug = self.owner.owner as Player;

        if (scug.privSneak > 0.5f && scug.grasps[self.limbNumber] != null && scug.grasps[self.limbNumber].grabbed is LizardHeadShell)
        {
            self.huntSpeed = 12f;
            self.quickness = 0.7f;
            return true;
        }

        return orig(self);
    }

    internal static void SlugcatHand_Update(On.SlugcatHand.orig_Update orig, SlugcatHand self)
    {
        orig(self);

        Player scug = self.owner.owner as Player;

        if (scug.privSneak > 0.5f && scug.grasps[self.limbNumber] != null)
        {
            if (scug.grasps[self.limbNumber].grabbed is LizardHeadShell)
            {
                self.relativeHuntPos *= 1f - (scug.grasps[self.limbNumber].grabbed as LizardHeadShell).donned;
            }
        }
    }
}
