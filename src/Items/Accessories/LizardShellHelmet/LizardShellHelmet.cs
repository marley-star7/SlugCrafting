using SlugCrafting.Items.Accessories;
using SlugCrafting.Items.Accessories.LizardShellHelmet;

namespace SlugCrafting.Items;

public class LizardShellHelmet : SimpleFullHeadAccessoryItem
{
    public LizardShellHelmetArmorAccessory armorAccessory => (LizardShellHelmetArmorAccessory)accessory;

    public string spriteName = "greenLizardShellHelmet";

    public LizardShellHelmet(AbstractLizardShellHelmet abstractHeadAccessory) : base(abstractHeadAccessory)
    {
        var accessoryStats = new AccessoryArmorStats
        {
            health = 5f,
            maxHealth = 5f,
            runSpeedMultiplier = 0.9f,
            grabProtectionChance = 1f,
        };

        var shellHelmetCosmetic = new DynamicLizardShellHelmetCosmetic
        (
            accessoryStats,
            new DynamicSlugcatFullHeadAttachedCosmetic.SpriteInfo[]
            {
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteInfo()
                {
                    name = spriteName + "_HeadBackShell1_",
                    distanceFromHeadModifier = -0.2f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteInfo()
                {
                    name = spriteName + "_HeadBackShell0_",
                    distanceFromHeadModifier = 0f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteInfo()
                {
                    name = spriteName + "_HeadFrontShell0_",
                    distanceFromHeadModifier = 0f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteInfo()
                {
                    name = spriteName + "_HeadFrontDark0_",
                    distanceFromHeadModifier = 0f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteInfo()
                {
                    name = spriteName + "_FaceFrontShell0_",
                    distanceFromHeadModifier = 0.3f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteInfo()
                {
                    name = spriteName + "_FaceFrontShell1_",
                    distanceFromHeadModifier = 0.6f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteInfo()
                {
                    name = spriteName + "_FaceFrontShell2_",
                    distanceFromHeadModifier = 1f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteInfo()
                {
                    name = spriteName + "_FaceFrontDark2_",
                    distanceFromHeadModifier = 1f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteInfo()
                {
                    name = spriteName + "_FaceFrontShell3_",
                    distanceFromHeadModifier = 1.1f
                },
                new DynamicSlugcatFullHeadAttachedCosmetic.SpriteInfo()
                {
                    name = spriteName + "_FaceFrontDark3_",
                    distanceFromHeadModifier = 1.1f
                }
            },
            new SpriteLayerGroup[]
            {
                new SpriteLayerGroup((int)CCGEnums.SlugcatCosmeticLayer.BaseHead, 0, 1),
                new SpriteLayerGroup((int)CCGEnums.SlugcatCosmeticLayer.FaceMask, 2, 10),
            }
        )
        {
            spriteEffectGroups = new SpriteEffectGroup[]
            {
                new SpriteEffectGroup(0,1,2,4,5,6,8),
                new SpriteEffectGroup(3,7,9)
            }
        };

        accessory = new LizardShellHelmetArmorAccessory(shellHelmetCosmetic, accessoryStats)
        {
            equipRegions = new Accessory.EquipRegion[] {
                Accessory.EquipRegion.Head,
            },
        };

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

    public override void HitByWeapon(Weapon weapon)
    {
        base.HitByWeapon(weapon);

        /*
        AddDamage(weapon.HeavyWeapon ? 0.5f : 0.2f);
        WhiteFlicker(20);
        Flicker(30);
        */

        firstChunk.vel = Vector2.zero;

        //HitEffect(weapon.firstChunk.vel);
    }

    public override void Update(bool eu)
    {
        base.Update(eu);

        BodyChunk chunk;
        var radius = 3f;

        if (wearer == null)
        {
            chunk = bodyChunks[0];
        }
        else
        {
            chunk = wearer.firstChunk;
            chunk.collideWithObjects = false;
            radius = chunk.rad + 0.1f;
        }

        //-- MR7: Lowkey I stole this from Fisobs CentiShields lol...
        // It seems to be responsible for the ACTUAL collision causing the spear to bounce off, but haven't deciphered it well enough yet.
        if (!Custom.DistLess(chunk.lastPos, chunk.pos, 3f) && room.GetTile(chunk.pos).Solid && !room.GetTile(chunk.lastPos).Solid)
        {
            var firstSolid = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(room, room.GetTilePosition(chunk.lastPos), room.GetTilePosition(chunk.pos));
            if (firstSolid != null)
            {
                FloatRect floatRect = Custom.RectCollision(chunk.pos, chunk.lastPos, room.TileRect(firstSolid.Value).Grow(15F));
                chunk.pos = floatRect.GetCorner(FloatRect.CornerLabel.D);

                if (floatRect.GetCorner(FloatRect.CornerLabel.B).x < 0f)
                {
                    chunk.vel.x = Mathf.Abs(chunk.vel.x) * 0.15f;
                }
                else if (floatRect.GetCorner(FloatRect.CornerLabel.B).x > 0f)
                {
                    chunk.vel.x = -Mathf.Abs(chunk.vel.x) * 0.15f;
                }
                else if (floatRect.GetCorner(FloatRect.CornerLabel.B).y < 0f)
                {
                    chunk.vel.y = Mathf.Abs(chunk.vel.y) * 0.15f;
                }
                else if (floatRect.GetCorner(FloatRect.CornerLabel.B).y > 0f)
                {
                    chunk.vel.y = -Mathf.Abs(chunk.vel.y) * 0.15f;
                }
            }
        }
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
