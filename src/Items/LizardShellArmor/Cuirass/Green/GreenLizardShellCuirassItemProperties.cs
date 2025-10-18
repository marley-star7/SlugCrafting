using static SlugCrafting.Items.LizardShellCuirass;

namespace SlugCrafting.Items;

public class GreenLizardShellCuirassItemProperties : LizardShellCuirassItemProperties
{
    public GreenLizardShellCuirassItemProperties() : base(new GreenLizardShellCuirassAccessoryProperties())
    {

    }

    public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 1;
}
