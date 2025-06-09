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

    public virtual float MassModifier() => 0.3f;

    public virtual Color ShellColor() => new(0.5f, 0.5f, 0.5f);

    public virtual string HeadSprite0Jaw() => "LizardJaw0.0";
    public virtual string HeadSprite1LowerTeeth() => "LizardLowerTeeth0.0";
    public virtual string HeadSprite2UpperTeeth() => "LizardUpperTeeth0.0"; 
    public virtual string HeadSprite3Head() => "LizardHead0.0";
    public virtual string HeadSprite4Eyes() => "LizardEyes0.0";

    public virtual float HeadBodyChunkRadius() => 8f;
    public virtual float HeadBodyChunkMass() => 0.7f;

    public virtual float Health() => 2f;

    public override void Throwable(Player player, ref bool throwable)
        => throwable = true;

    public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 3;

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
    {
        grabability = Player.ObjectGrabability.BigOneHand;
    }
}