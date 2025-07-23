using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugCrafting.Accessories;

/// <summary>
/// Stats for an armor accessory.
/// </summary>
public class AccessoryArmorStats
{
    /// <summary>
    /// The index of the body chunk many accessory modifications such as mass will be applied to.
    /// </summary>
    public int wearingBodyChunkIndex;
    public float mass = 0;
    public float health = 0;
    public float maxHealth = 0;

    /// <summary>
    /// Determines wether not an accessory will protect it's wearing body chunk from damage.
    /// </summary>
    public bool isArmor;

    /// <summary>
    /// The chance this accessory will be grabbed instead of the player.
    /// </summary>
    public float grabProtectionChance;

    public float runSpeedMultiplier = 1f;
    public float poleClimbSpeedMultiplier = 1f;
    public float corridorClimbSpeedMultiplier = 1f;

    public float swimForceMultiplier = 1f;
    public float swimBoostMultiplier = 1f;

    public float generalVisibilityBonusMultiplier = 1f;
    public float loudnessMultiplier = 1f;
}
