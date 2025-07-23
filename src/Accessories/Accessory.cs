//-- MR7: Just like base game, functionality and graphics are seperate classes.
// Cosmetics are the accessory graphics, while the gameplay function code resides in the accessory.

namespace SlugCrafting.Accessories;

public class Accessory
{
    /// <summary>
    /// The enum of areas that a cosmetic can be equipped to.
    /// Used to decide what accessories can be worn together with this.
    /// </summary>
    public enum EquipRegion
    {
        None,
        Head,
        LeftEar,
        RightEar,
        LeftEye,
        RightEye,
        Nose,
        Mouth
    }

    /// <summary>
    /// The areas the accessory counts as equipping to.
    /// Use this to decide what accessories can or cannot be worn together with this.
    /// </summary>
    public EquipRegion[] equipRegions = new[]
    {
        EquipRegion.None
    };

    public enum HideRegion
    {
        None,
        Head,
        LeftEar,
        RightEar,
        LeftEye,
        RightEye,
        Nose,
    }

    /// <summary>
    /// Regions of the body that will be hidden by this cosmetic.
    /// Hiding a body region simply makes it invisible.
    /// </summary>
    public HideRegion[] hideRegions = new[]
    {
        HideRegion.None,
    };

    public Player? wearer
    {
        get { return cosmetics[0].wearer as Player; }
    }
    /// <summary>
    /// The overlaying sprite parts that make up the total accessory.
    /// </summary>
    public DynamicCosmetic[] cosmetics;

    public AccessoryArmorStats? armorStats;

    public bool isEquipped => cosmetics[0].isEquipped;

    public Accessory(DynamicCosmetic[] cosmetics, AccessoryArmorStats? armorStats = null)
    {
        this.cosmetics = cosmetics;
        this.armorStats = armorStats;
    }

    public void Equip(Player wearer)
    {
        var wearerCraftingData = wearer.GetPlayerCraftingData();

        wearerCraftingData.accessories.Add(this);
        // Equip this accessory to all equip regions.
        for (int i = 0; i < equipRegions.Length; i++)
        {
            if (wearerCraftingData.equipRegionAccessories.ContainsKey(equipRegions[i]))
            {
                var existingAccessory = wearerCraftingData.equipRegionAccessories[equipRegions[i]];
                //TODO: add unequip functionality here, or that scug just says (no)
            }
            else
            {
                wearerCraftingData.equipRegionAccessories[equipRegions[i]] = this;
            }
        }

        wearer.UpdateMass();

        for (int i = 0; i < cosmetics.Length; i++)
        {
            cosmetics[i].Equip(wearer);
        }
    }

    public virtual bool OnSpearHitWearer(Spear spear, SharedPhysics.CollisionResult result, bool eu)
    {
        return true;
    }

    public virtual void OnWearerViolence(ViolenceContext violenceContext)
    {

    }

    public virtual void OnWearerTerrainImpact(TerrainImpactContext impactContext)
    {
        // Default implementation does nothing.
    }

    public virtual void OnWearerGrabbed(Creature.Grasp grasp)
    {
        // Default implementation does nothing.
    }
}