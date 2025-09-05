using UnityEngine;

namespace SlugCrafting.Items;

class PinkLizardHeadShellItemProperties : LizardHeadShellItemProperties
{
    public override Color DefaultShellColor => Color.magenta;

    public override string HeadSprite0Jaw => "LizardJaw0.0";
    public override string HeadSprite1LowerTeeth => "LizardLowerTeeth0.0";
    public override string HeadSprite2UpperTeeth => "LizardUpperTeeth0.0"; 
    public override string HeadSprite3Head => "LizardHead0.0";
    public override string HeadSprite4Eyes => "LizardEyes0.0";

    public override float DefaultHeadBodyChunkRadius => 8f;
    public override float DefaultHeadBodyChunkMass => 0.7f;

    public override float MaxHealth => 2f;
}