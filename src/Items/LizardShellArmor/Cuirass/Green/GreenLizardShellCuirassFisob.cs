using MRCustom.Json;

namespace SlugCrafting.Items;

public class GreenLizardShellCuirassFisob : LizardShellCuirassFisob
{
    public override LizardShellCuirassItemProperties ItemProperties => new GreenLizardShellCuirassItemProperties();

    public GreenLizardShellCuirassFisob() : base(Enums.AbstractObjectType.GreenLizardShellCuirass)
    {
        RegisterItemPropertiesType(Enums.AbstractObjectType.GreenLizardShellCuirass, ItemProperties);
        RegisterUnlock(Enums.SandboxUnlockID.GreenLizardShellCuirass, parent: MultiplayerUnlocks.SandboxUnlockID.GreenLizard, data: 0);
    }
}