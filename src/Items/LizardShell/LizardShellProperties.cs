using UnityEngine;

using Fisobs.Properties;

namespace SlugCrafting.Items;

abstract class LizardShellProperties : ItemProperties
{
    public virtual float MassModifier() => 0.4f;

    public virtual Color ShellColor() => new(0.5f, 0.5f, 0.5f);

    public virtual string HeadSprite0Jaw() => "LizardJaw0.0";
    public virtual string HeadSprite1LowerTeeth() => "LizardLowerTeeth0.0";
    public virtual string HeadSprite2UpperTeeth() => "LizardUpperTeeth0.0"; 
    public virtual string HeadSprite3Head() => "LizardHead0.0";
    public virtual string HeadSprite4Eyes() => "LizardEyes0.0";

    public virtual float HeadBodyChunkRadius() => 8f; // Default for Pink Lizards
    public virtual float HeadBodyChunkMass() => 0.7f; // Default for Pink Lizards

    public override void Throwable(Player player, ref bool throwable)
        => throwable = true;

    public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 3;

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
    {
        grabability = Player.ObjectGrabability.BigOneHand;
    }
}