using UnityEngine;

using Fisobs.Properties;

namespace SlugCrafting.Items;

/// <summary>
/// All default values based off pink lizards.
/// </summary>
public class LizardHeadShellItemProperties : ItemProperties
{
    /// <summary>
    /// Dictionary of all the corresponding properties for each lizard template type.
    /// Add to this when adding a custom lizard type to the dictionary for it to be recognized.
    /// </summary>
    public readonly static Dictionary<CreatureTemplate.Type, LizardHeadShellItemProperties> PropertiesOfTemplateType = new()
    {
        // Default lizard head shell used for modded or non-registered lizard types,
        { CreatureTemplate.Type.LizardTemplate, new LizardHeadShellItemProperties() }
    };

    public LizardHeadShellItemProperties()
    {

    }

    public virtual Color DefaultShellColor { get => new(0.5f, 0.5f, 0.5f); }

    public virtual string HeadSprite0Jaw { get => "LizardJaw0.0"; }
    public virtual string HeadSprite1LowerTeeth { get => "LizardLowerTeeth0.0"; }
    public virtual string HeadSprite2UpperTeeth { get => "LizardUpperTeeth0.0"; }
    public virtual string HeadSprite3Head { get => "LizardHead0.0"; }
    public virtual string HeadSprite4Eyes { get => "LizardEyes0.0"; }

    public virtual float DefaultHeadBodyChunkRadius { get => 8f; }
    public virtual float DefaultHeadBodyChunkMass { get => 0.7f; }

    public virtual float MassModifier { get => 0.3f; }

    public virtual float MaxHealth { get => 2f; }

    public override void Throwable(Player player, ref bool throwable)
        => throwable = true;

    public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 3;

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
    {
        grabability = Player.ObjectGrabability.BigOneHand;
    }
}