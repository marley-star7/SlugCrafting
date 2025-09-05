namespace SlugCrafting.Items;

// TODO: these are not right, need to get the correct default values from the game.
class GreenLizardHeadShellProperties : LizardHeadShellItemProperties
{
    public override Color DefaultShellColor => Color.green; // Green color

    public override string HeadSprite0Jaw => "LizardJaw0.0";
    public override string HeadSprite1LowerTeeth => "LizardLowerTeeth0.0";
    public override string HeadSprite2UpperTeeth => "LizardUpperTeeth0.0";
    public override string HeadSprite3Head => "LizardHead0.0";
    public override string HeadSprite4Eyes => "LizardEyes0.0";

    public override float DefaultHeadBodyChunkRadius => 8f;
    public override float DefaultHeadBodyChunkMass => 0.7f;

    public override float MaxHealth => 5f;

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
    {
        grabability = Player.ObjectGrabability.TwoHands;
    }
}
