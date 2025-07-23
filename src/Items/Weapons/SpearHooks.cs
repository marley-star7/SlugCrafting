
namespace SlugCrafting;

internal static class SpearHooks
{
    internal static bool Spear_HitSomething(On.Spear.orig_HitSomething orig, Spear self, SharedPhysics.CollisionResult result, bool eu)
    {
        if (result.obj == null)
        {
            return false;
        }
        var didHit = true;
        if (result.obj is Player)
        {
            var playerCraftingData = ((Player)result.obj).GetPlayerCraftingData();
            for (int i = 0; i < playerCraftingData.accessories.Count; i++)
            {
                didHit = playerCraftingData.accessories[i].OnSpearHitWearer(self, result, eu);
            }
        }
        if (!didHit)
        {
            return false;
        }
        return orig(self, result, eu);
    }

    internal static void Spear_Update(On.Spear.orig_Update orig, Spear selfSpear, bool eu)
    {
        var spearData = selfSpear.GetSpearCraftingData();

        //-- Only run if not the back of a double sided spear,
        // as double sided spear front will override updates if it is enabled instead.
        switch (spearData.sidedMode)
        {
            case SpearCraftingData.SidedMode.SingleSided:
                NormalUpdate();
                break;

            case SpearCraftingData.SidedMode.DoubleSidedFront:
                NormalUpdate();

                var backSpear = spearData.oppositeSidedSpear;
                backSpear.DoubleSidedSpearBackUpdate(eu);
                backSpear.DoubleSidedSpearBackSkewerUpdate(eu);

                backSpear.AttachedSporePlantUpdate();

                break;

            case SpearCraftingData.SidedMode.DoubleSidedBack:
                // Do nothing, front spear controls these updates.
                break;
        }

        void NormalUpdate()
        {
            orig(selfSpear, eu);
            selfSpear.AttachedSporePlantUpdate();
        }
    }

    internal static void Spear_Thrown(On.Spear.orig_Thrown orig, Spear self, Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);

        var harpoonRope = new HarpoonRope(self)
        {
            grasp = thrownBy.grasps[0],
        };
    }

    internal static void Spear_LodgeInCreature(On.Spear.orig_LodgeInCreature_CollisionResult_bool orig, Spear selfSpear, SharedPhysics.CollisionResult result, bool eu)
    {
        orig(selfSpear, result, eu);
        var selfSpearCraftingData = selfSpear.GetSpearCraftingData();

        //- MR7: If player manages to lodge the spear in a creature we do NOT trigger the spore plant immediately,
        // so they have some time to get away, rewarding them with the spore plant spear actually working in stopping the threat.
        // Ideally, we somehow change the spore plant's to focus the specific impaled creature, but couldn't figure out how to do that good yet sooo.
        // TODO: make it so that the spore plant focuses the area closer around the creature impaled, or avoids the player.
        /*
        if (selfSpearCraftingData.sporePlant != null)
            selfSpearCraftingData.sporePlant.BeeTrigger();
        */
    }

    internal static void Spear_Destroy(On.UpdatableAndDeletable.orig_Destroy orig, UpdatableAndDeletable self)
    {
        orig(self);

        try
        {
            // Destroy the opposite side spear if it exists first.
            if (self is Spear)
            {
                var selfSpear = (Spear)self;
                var selfSpearCraftingData = selfSpear.GetSpearCraftingData();

                if (selfSpearCraftingData.oppositeSidedSpear != null && selfSpearCraftingData.oppositeSidedSpear.slatedForDeletetion == false)
                    selfSpearCraftingData.oppositeSidedSpear?.Destroy();
            }
            orig(self);
        }
        catch (System.Exception e)
        {
            Custom.Logger.Error($"Error destroying spear: {e.Message}\n{e.StackTrace}");
        }
    }

    internal static void Spear_NewRoom(On.PhysicalObject.orig_NewRoom orig, PhysicalObject self, Room newRoom)
    {
        orig(self, newRoom);

        if (self is not Spear)
            return;

        var spear = self as Spear;
    }

    internal static void Spear_ChangeMode(On.Spear.orig_ChangeMode orig, Spear selfSpear, Weapon.Mode newMode)
    {
        orig(selfSpear, newMode);

        var spearData = selfSpear.GetSpearCraftingData();
        if (spearData.sidedMode == SpearCraftingData.SidedMode.DoubleSidedFront)
        {
            var backSpearCraftingData = spearData.oppositeSidedSpear.GetSpearCraftingData();

            if (newMode == Weapon.Mode.StuckInWall)
            {
                spearData.isControllingSpearSide = false;
                backSpearCraftingData.isControllingSpearSide = true;

                backSpearCraftingData.skeweringActive = true;
            }
            else if (newMode == Weapon.Mode.Carried)
            {
                spearData.isControllingSpearSide = true;
                backSpearCraftingData.isControllingSpearSide = false;
            }
            else
            {
                backSpearCraftingData.skeweringActive = false;
                // Free all the skewered chunks.
                foreach (var chunk in spearData.skeweredChunks)
                {
                    backSpearCraftingData.skeweredChunks.Remove(chunk);
                }
            }
        }
    }

    //
    // GRAPHICS FUNCTIONS
    //

    internal static void Spear_InitiateSprites(On.Spear.orig_InitiateSprites orig, Spear self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        orig(self, sLeaser, rCam);
    }

    internal static void Spear_DrawSprites(On.Spear.orig_DrawSprites orig, Spear self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        var spearData = self.GetSpearCraftingData();

        //-- Only run if not the back of a double sided spear,
        // as double sided spear front will override updates if it is enabled instead.
        switch (spearData.sidedMode)
        {
            case SpearCraftingData.SidedMode.SingleSided:
                NormalUpdate();
                break;

            case SpearCraftingData.SidedMode.DoubleSidedFront:
                break;

            case SpearCraftingData.SidedMode.DoubleSidedBack:
                NormalUpdate();
                break;
        }

        void NormalUpdate()
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            if (spearData.sporePlant != null)
            {
                //rCam.MoveObjectToInternalContainer(spearData.sporePlant, self, -1);
            }
        }
    }
}