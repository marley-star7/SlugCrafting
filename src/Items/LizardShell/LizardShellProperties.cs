using UnityEngine;

using Fisobs.Properties;

namespace SlugCrafting.Items;

/// <summary>
/// All default values based off pink lizards.
/// </summary>
public class LizardShellProperties : ItemProperties
{
    /// <summary>
    /// Dictionary of all the corresponding properties for each lizard template type.
    /// Add to this when adding a custom lizard type to the dictionary for it to be recognized.
    /// </summary>
    public static Dictionary<CreatureTemplate.Type, LizardShellProperties> PropertiesOfTemplateType = new();

    public virtual Color defaultShellColor { get => new(0.5f, 0.5f, 0.5f); }

    public virtual string headSprite0Jaw { get => "LizardJaw0.0"; }
    public virtual string headSprite1LowerTeeth { get => "LizardLowerTeeth0.0"; }
    public virtual string headSprite2UpperTeeth { get => "LizardUpperTeeth0.0"; }
    public virtual string headSprite3Head { get => "LizardHead0.0"; }
    public virtual string headSprite4Eyes { get => "LizardEyes0.0"; }

    public virtual float defaultHeadBodyChunkRadius { get => 8f; }
    public virtual float defaultHeadBodyChunkMass { get => 0.7f; }

    public virtual float massModifier { get => 0.3f; }

    public virtual float maxHealth { get => 2f; }

    public override void Throwable(Player player, ref bool throwable)
        => throwable = true;

    public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 3;

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
    {
        grabability = Player.ObjectGrabability.BigOneHand;
    }
}