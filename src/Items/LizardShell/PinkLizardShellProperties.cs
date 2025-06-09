using UnityEngine;

namespace SlugCrafting.Items;

class PinkLizardShellProperties : LizardShellProperties
{
    public override Color ShellColor() => Color.magenta;

    public override string HeadSprite0Jaw() => "LizardJaw0.0";
    public override string HeadSprite1LowerTeeth() => "LizardLowerTeeth0.0";
    public override string HeadSprite2UpperTeeth() => "LizardUpperTeeth0.0"; 
    public override string HeadSprite3Head() => "LizardHead0.0";
    public override string HeadSprite4Eyes() => "LizardEyes0.0";

    public override float HeadBodyChunkRadius() => 8f;
    public override float HeadBodyChunkMass() => 0.7f;

    public override float Health() => 2f;
}