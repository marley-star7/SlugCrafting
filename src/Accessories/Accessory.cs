using SlugCrafting.Accessories;
using UnityEngine;

using CompartmentalizedCreatureGraphics;

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

    public Player? wearer;
    /// <summary>
    /// The overlaying sprite parts that make up the total accessory.
    /// </summary>
    public DynamicCosmetic[] cosmetics;

    public float runSpeedModifier;

    public Accessory(DynamicCosmetic[] cosmetics)
    {
        this.cosmetics = cosmetics;
    }

    public void Equip(Player wearer)
    {
        wearer.GetPlayerCraftingData().accessories.Add(this);
        this.wearer = wearer;

        for (int i = 0; i < cosmetics.Length; i++)
        {
            cosmetics[i].Equip(wearer);
        }
    }
}