using UnityEngine;

namespace SlugCrafting.Items;

// TODO: these are not right, need to get the correct default values from the game.
class GreenLizardShellProperties : LizardShellProperties
{
    public override Color ShellColor() => new(0.5f, 1f, 0.5f); // Green color

    public override string HeadSprite0Jaw() => "LizardJaw0.0";
    public override string HeadSprite1LowerTeeth() => "LizardLowerTeeth0.0";
    public override string HeadSprite2UpperTeeth() => "LizardUpperTeeth0.0";
    public override string HeadSprite3Head() => "LizardHead0.0";
    public override string HeadSprite4Eyes() => "LizardHead0.0";

    public override float HeadBodyChunkRadius() => 8f;
    public override float HeadBodyChunkMass() => 0.7f;

    public override float Health() => 5f;
}
