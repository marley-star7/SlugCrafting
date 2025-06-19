using RWCustom;
using UnityEngine;

using MRCustom;

using SlugCrafting.Items;
using SlugCrafting.Items.Weapons;
using SlugCrafting.Animations;
using SlugCrafting.Crafts;
using SlugCrafting.Core;
using SlugCrafting.Scavenges;

namespace SlugCrafting.Core;

//-- MR7: I would move this somewhere else if I could think of a better spot.
public static partial class Content
{
    //
    //-- FISOBS
    //

    internal static void RegisterSlugCraftingFisobs()
    {
        Fisobs.Core.Content.Register(new KnifeFisob());
        Fisobs.Core.Content.Register(new LizardHideFisob());
        Fisobs.Core.Content.Register(new StringFisob());

        Fisobs.Core.Content.Register(new LizardShellFisob());
        Fisobs.Core.Content.Register(new GreenLizardShellFisob());
        Fisobs.Core.Content.Register(new PinkLizardShellFisob());

        Fisobs.Core.Content.Register(new GreenLizardShellHelmetFisob());

        //Fisobs.Core.Content.Register(new DoubleSidedSpearFisob());
    }

    //
    //-- BUNDLES
    //

    internal static void RegisterSlugCraftingItemBundlesProperties()
    {
        SlugCrafting.Core.Content.RegisterItemBundleProperties(
            StringFisob.abstractObjectType,
            new ItemBundleProperties
            (
                3
            )
        );
    }

    //
    //-- SCAVENGES
    //

    internal static void RegisterSlugCraftingScavenges()
    {
        //-- SCAVENGE DATA
        SlugCrafting.Core.Content.RegisterScavengeData(CreatureTemplate.Type.GreenLizard, typeof(GreenLizardScavengeData));
        SlugCrafting.Core.Content.RegisterScavengeData(CreatureTemplate.Type.PinkLizard, typeof(PinkLizardScavengeData));
        SlugCrafting.Core.Content.RegisterScavengeData(CreatureTemplate.Type.LanternMouse, typeof(LanternMouseScavengeData));
    }

    //
    //-- CRAFTS
    //

    internal static void RegisterSlugCraftingCrafts()
    {
        //
        //-MR7 TODO: can probably remove the "CraftIngredient" type and just search by abstract object?
        // Use craft result instead to decide wether something is consumed via an easy function, would definitely fit nicer.
        //

        //-- CRAFT DATA
        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = new CraftIngredient()
                {
                    type = AbstractPhysicalObject.AbstractObjectType.WaterNut,
                },

                secondaryIngredient = new CraftIngredient()
                {
                    type = AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant,
                },

                ingredientValidation = (in PhysicalObject primaryIngredientObject, in PhysicalObject secondaryIngredientObject) =>
                {
                    var waterNut = (WaterNut)primaryIngredientObject;
                    // Water nut has to be not swollen
                    if (waterNut.AbstrNut.swollen == false)
                        return true;
                    else
                        return false;
                },

                craftResult = (Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject) =>
                {
                    crafter.RemoveGrabbedObject(0);
                    crafter.RemoveGrabbedObject(1);

                    var player = (crafter as Player);
                    player.RealizeAndGrab(new AbstractPhysicalObject(
                         crafter.room.world,
                         AbstractPhysicalObject.AbstractObjectType.ScavengerBomb,
                         null,
                         crafter.coord,
                         crafter.room.game.GetNewID()
                         ));
                },

                craftTime = 40,
                handAnimationIndex = Enums.HandAnimationIndex.DoubleSwallowCraft,
                handAnimation = new SwallowCraftPlayerHandAnimation(40)
                {
                }
            }
        );

        //
        // SPEAR PRIMARY CRAFTS
        //

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = new CraftIngredient()
                {
                    type = AbstractPhysicalObject.AbstractObjectType.Spear,
                },

                secondaryIngredient = new CraftIngredient()
                {
                    type = AbstractPhysicalObject.AbstractObjectType.Rock,
                },

                craftResult = (Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject) =>
                {
                    var spear = primaryIngredientObject as Spear;
                    //-- Spawn two break parts for all spears.
                    for (int i = 0; i < 2; i++)
                    {
                        crafter.room.AddObject(new ExplosiveSpear.SpearFragment(spear.firstChunk.pos, Custom.RNV() * Mathf.Lerp(3f, 6f, UnityEngine.Random.value), spear));
                    }
                    //-- Add a puff ball to fall off if the spear is explosive.
                    if (spear is ExplosiveSpear)
                    {
                        var explosiveSpear = spear as ExplosiveSpear;
                        crafter.room.AddObject(new PuffBallSkin(spear.firstChunk.pos + spear.rotation * (spear.pivotAtTip ? 0f : 10f), Custom.RNV() * Mathf.Lerp(3f, 6f, UnityEngine.Random.value), explosiveSpear.redColor, Color.Lerp(explosiveSpear.redColor, new Color(0f, 0f, 0f), 0.3f)));
                    }

                    //-- Delete the spear.
                    crafter.RemoveGrabbedObject(0);

                    var player = (crafter as Player);
                    player.RealizeAndGrab(
                        new AbstractKnife(
                             crafter.room.world,
                             KnifeFisob.abstractObjectType,
                             crafter.coord,
                             crafter.room.game.GetNewID()
                         )
                    );
                },

                craftTime = 200,
                handAnimationIndex = Enums.HandAnimationIndex.SmashIntoCraft,
                handAnimation = new SmashIntoCraftPlayerHandAnimation(200)
                {
                    timeBetweenBeats = 20f,
                    sinBeatingCurveStartRad = 0.7f,

                    beatSound = SoundID.Spear_Bounce_Off_Wall,
                    breakSound = SoundID.Spear_Fragment_Bounce,

                    fullRiseHandOffsetPos = new Vector2(13f, 9f),
                    fullDescentHandOffsetPos = new Vector2(-8f, -17f),
                }
            }
        );

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = new CraftIngredient()
                {
                    type = AbstractPhysicalObject.AbstractObjectType.Spear,
                },

                secondaryIngredient = new CraftIngredient()
                {
                    type = KnifeFisob.abstractObjectType,
                },

                ingredientValidation = (in PhysicalObject primaryIngredientObject, in PhysicalObject secondaryIngredientObject) =>
                {
                    // TODO: for some reason this validation isn't working / updating.
                    // Only can craft if the spear is not already sharpened.
                    var spear = (Spear)primaryIngredientObject;
                    if (spear.GetSpearCraftingData().sidedMode == SpearCraftingData.SidedMode.SingleSided)
                        return true;
                    else
                        return false;
                },

                craftResult = (Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject) =>
                {
                    //- MR7 was desperately trying to do a better method of making double sided spears, and failed.

                    /*
                    var origSpear = primaryIngredientObject as Spear;

                    var abstractDoubleSidedSpear = new AbstractDoubleSidedSpear(
                        origSpear.abstractPhysicalObject,
                        crafter.room.world,
                        null,
                        crafter.coord,
                        crafter.room.game.GetNewID()
                    );

                    var player = crafter as Player;

                    player.RemoveGrabbedObject(0);

                    //-- Realize it.
                    crafter.room.abstractRoom.AddEntity(abstractDoubleSidedSpear);
                    abstractDoubleSidedSpear.RealizeInRoom();

                    //-- Grab it
                    var newDoubleSidedSpear = abstractDoubleSidedSpear.realizedObject;
                    player.SlugcatGrab(newDoubleSidedSpear, player.FreeHand());

                    return null;
                    */

                    //-- Drop the original spear.
                    crafter.ReleaseGrasp(0);

                    var origSpear = primaryIngredientObject as Spear;
                    var origSpearCraftingData = origSpear.GetSpearCraftingData();

                    //-- Create the new spear front to hold.
                    var newAbstractFrontSpear = new AbstractSpear(
                        crafter.room.world,
                        null,
                        crafter.coord,
                        crafter.room.game.GetNewID(),
                        false // Not explosive
                    );

                    //-- Realize it.
                    crafter.room.abstractRoom.AddEntity(newAbstractFrontSpear);
                    newAbstractFrontSpear.RealizeInRoom();

                    //-- Make sure the spears are connected.
                    var newSpear = newAbstractFrontSpear.realizedObject as Spear;
                    var newSpearCraftingData = newSpear.GetSpearCraftingData();

                    origSpearCraftingData.sidedMode = SpearCraftingData.SidedMode.DoubleSidedBack;
                    origSpearCraftingData.oppositeSidedSpear = newSpear;

                    newSpearCraftingData.sidedMode = SpearCraftingData.SidedMode.DoubleSidedFront;
                    newSpearCraftingData.oppositeSidedSpear = origSpear;

                    //-- Grab the new spear front.
                    var player = crafter as Player;
                    player.SlugcatGrab(newSpear, player.FreeHand());
                },

                craftTime = 100,
                handAnimationIndex = Enums.HandAnimationIndex.SmashIntoCraft, // TODO: CHANGE THIS TO BE VALID
                handAnimation = new SharpenCraftPlayerHandAnimation(100)
            }
        );

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = new CraftIngredient()
                {
                    type = AbstractPhysicalObject.AbstractObjectType.Spear,
                },
                secondaryIngredient = new CraftIngredient()
                {
                    type = AbstractPhysicalObject.AbstractObjectType.SporePlant,
                },

                ingredientValidation = (in PhysicalObject _, in PhysicalObject secondaryIngredientObject) =>
                {
                    var sporePlant = (SporePlant)secondaryIngredientObject;
                    // Spore plant has to be pacified first.
                    if (sporePlant.AbstrSporePlant.pacified == true)
                        return true;
                    else
                        return false;
                },

                craftResult = (Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject) =>
                {
                    var spear = (Spear)primaryIngredientObject;
                    var sporePlant = (SporePlant)secondaryIngredientObject;

                    spear.AttachSporePlant(sporePlant);

                    //-- Drop the spore plant.
                    crafter.ReleaseGrasp(1);
                },

                craftTime = 20,
                handAnimationIndex = Enums.HandAnimationIndex.SmashIntoCraft,
                handAnimation = new SmashIntoCraftPlayerHandAnimation(20)
                {
                    primaryHandWeaponSetRotation = new Vector2(1, 1).normalized,
                    secondaryHandWeaponSetRotation = new Vector2(1, 1).normalized
                }
            }
        );

        SlugCrafting.Core.Content.RegisterCraft(
            new Craft()
            {
                primaryIngredient = new CraftIngredient()
                {
                    type = AbstractPhysicalObject.AbstractObjectType.Lantern,
                },
                secondaryIngredient = new CraftIngredient()
                {
                    type = StringFisob.abstractObjectType,
                },

                craftResult = (Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject) =>
                {
                    crafter.RemoveGrabbedObject(1); // Remove the string object.
                    var objectToTie = (PhysicalObject)primaryIngredientObject;

                    //-- Create the new object tie.
                    var abstractStringTier = new AbstractStringTiedItem(
                        crafter.room.world,
                        crafter.coord,
                        crafter.room.game.GetNewID()
                    );

                    //-- Realize it.
                    crafter.room.abstractRoom.AddEntity(abstractStringTier);
                    abstractStringTier.RealizeInRoom();

                    var stringTier = (StringTiedItem)abstractStringTier.realizedObject;
                    stringTier.AttachToObject(objectToTie.abstractPhysicalObject, 0);

                    var player = (Player)crafter;
                    player.SlugcatGrab(abstractStringTier.realizedObject, player.FreeHand());
                },

                craftTime = 20,
                handAnimationIndex = Enums.HandAnimationIndex.SmashIntoCraft,
                handAnimation = new SmashIntoCraftPlayerHandAnimation(20)
            }
        );
    }
}