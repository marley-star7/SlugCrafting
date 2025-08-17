using UnityEngine;

namespace SlugCrafting.Items;

class PinkLizardShellProperties : LizardShellProperties
{
    public PinkLizardShellProperties() : base(CreatureTemplate.Type.PinkLizard)
    {

    }

    public override Color defaultShellColor => Color.magenta;

    public override string headSprite0Jaw => "LizardJaw0.0";
    public override string headSprite1LowerTeeth => "LizardLowerTeeth0.0";
    public override string headSprite2UpperTeeth => "LizardUpperTeeth0.0"; 
    public override string headSprite3Head => "LizardHead0.0";
    public override string headSprite4Eyes => "LizardEyes0.0";

    public override float defaultHeadBodyChunkRadius => 8f;
    public override float defaultHeadBodyChunkMass => 0.7f;

    public override float maxHealth => 2f;
}