namespace SlugCrafting.Items;

sealed class GreenLizardShellHelmetFisob : LizardShellHelmetFisob
{
    public override LizardShellHelmetItemProperties ItemProperties => new GreenLizardShellHelmetItemProperties();

    public GreenLizardShellHelmetFisob() : base(Enums.AbstractObjectType.GreenLizardShellHelmet)
    {
        RegisterItemPropertiesType(Enums.AbstractObjectType.GreenLizardShellHelmet, ItemProperties);
        RegisterUnlock(Enums.SandboxUnlockID.GreenLizardShellHelmet, parent: MultiplayerUnlocks.SandboxUnlockID.GreenLizard, data: 0);
    }
}