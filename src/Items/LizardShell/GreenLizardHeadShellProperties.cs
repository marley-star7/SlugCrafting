using UnityEngine;

namespace SlugCrafting.Items;

// TODO: these are not right, need to get the correct default values from the game.
class GreenLizardShellProperties : LizardHeadShellProperties
{
    public GreenLizardShellProperties() : base(CreatureTemplate.Type.GreenLizard)
    {

    }

    public override Color defaultShellColor => Color.green; // Green color

    public override string headSprite0Jaw => "LizardJaw0.0";
    public override string headSprite1LowerTeeth => "LizardLowerTeeth0.0";
    public override string headSprite2UpperTeeth => "LizardUpperTeeth0.0";
    public override string headSprite3Head => "LizardHead0.0";
    public override string headSprite4Eyes => "LizardEyes0.0";

    public override float defaultHeadBodyChunkRadius => 8f;
    public override float defaultHeadBodyChunkMass => 0.7f;

    public override float maxHealth => 5f;

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
    {
        grabability = Player.ObjectGrabability.TwoHands;
    }
}
