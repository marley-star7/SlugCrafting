namespace SlugCrafting.Items;

public class BlueLizardShellHelmetItemProperties : LizardShellHelmetItemProperties
{
    public BlueLizardShellHelmetItemProperties() : base(new BlueLizardShellHelmetAccessoryProperties())
    {

    }

    public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 1;
}
