using static SlugCrafting.Items.LizardShellHelmet;

namespace SlugCrafting.Items;

public class GreenLizardShellHelmetItemProperties : LizardShellHelmetItemProperties
{
    public GreenLizardShellHelmetItemProperties() : base(new GreenLizardShellHelmetAccessoryProperties())
    {

    }

    public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 1;
}
