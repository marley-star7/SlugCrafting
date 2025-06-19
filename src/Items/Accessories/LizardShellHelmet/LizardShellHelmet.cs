using SlugCrafting.Accessories;
using UnityEngine;

using CompartmentalizedCreatureGraphics.SlugcatCosmetics;
using CompartmentalizedCreatureGraphics;
using SlugCrafting.Cosmetics;

namespace SlugCrafting.Items;

public class LizardShellHelmet : SimpleFullHeadAccessoryItem
{
    public string spriteName = "greenLizardShellHelmet";

    public LizardShellHelmet(AbstractLizardShellHelmet abstractHeadAccessory) : base(abstractHeadAccessory)
    {
        var shellHelmetCosmetic = new DynamicLizardShellHelmetCosmetic()
        {
            behindHeadSprites = new DynamicSlugcatFullHeadAttachedCosmetic.SpriteLayer[]
            {
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteLayer()
                {
                    name = spriteName + "_HeadBackShell1_",
                    color = Color.green,
                    distanceFromHeadModifier = 0.2f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteLayer()
                {
                    name = spriteName + "_HeadBackShell0_",
                    color = Color.green,
                    distanceFromHeadModifier = 0f
                }
            },
            inFrontOfHeadSprites = new DynamicSlugcatFullHeadAttachedCosmetic.SpriteLayer[]
            {
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteLayer()
                {
                    name = spriteName + "_HeadFrontShell0_",
                    color = Color.green,
                    distanceFromHeadModifier = 0f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteLayer()
                {
                    name = spriteName + "_HeadFrontDark0_",
                    color = new Color(0,0,0.01f),
                    distanceFromHeadModifier = 0f
                },
            },
            inFrontOfFaceSprites = new DynamicSlugcatFullHeadAttachedCosmetic.SpriteLayer[]
            {
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteLayer()
                {
                    name = spriteName + "_FaceFrontShell0_",
                    color = Color.green,
                    distanceFromHeadModifier = 0.3f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteLayer()
                {
                    name = spriteName + "_FaceFrontShell1_",
                    color = Color.green,
                    distanceFromHeadModifier = 0.6f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteLayer()
                {
                    name = spriteName + "_FaceFrontShell2_",
                    color = Color.green,
                    distanceFromHeadModifier = 1f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteLayer()
                {
                    name = spriteName + "_FaceFrontDark2_",
                    color = new Color(0,0,0.01f),
                    distanceFromHeadModifier = 1f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteLayer()
                {
                    name = spriteName + "_FaceFrontShell3_",
                    color = Color.black,
                    distanceFromHeadModifier = 1.1f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteLayer()
                {
                    name = spriteName + "_FaceFrontDark3_",
                    color = new Color(0,0,0.01f),
                    distanceFromHeadModifier = 1.1f
                }
            },
        };

        accessory = new Accessory( 
            new DynamicCosmetic[] 
            {
                shellHelmetCosmetic,
            }
        )
        {
            equipRegions = new Accessory.EquipRegion[] {
                Accessory.EquipRegion.Head,
            }
        };
        accessory.runSpeedModifier = -0.2f;

        var pos = abstractPhysicalObject.Room.realizedRoom.MiddleOfTile(abstractPhysicalObject.pos.Tile);

        base.bodyChunks = new[] {
            new BodyChunk(this, 0, pos, 1, 0.1f),
        };

        base.bodyChunkConnections = new BodyChunkConnection[0];

        base.airFriction = 0.97f;
        base.gravity = 0.9f;
        base.bounce = 0.1f;
        base.surfaceFriction = 0.45f;
        base.collisionLayer = 1;
        base.waterFriction = 0.92f;
        base.buoyancy = 0.75f;
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[]
        {
            new FSprite(spriteName + "_FaceFrontShell0_A0", true)
            {
                color = Color.green
            }
        };

        AddToContainer(sLeaser, rCam, null);
    }
}
