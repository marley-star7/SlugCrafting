namespace SlugCrafting.Items;

sealed class BlueLizardShellHelmetFisob : LizardShellHelmetFisob
{
    public override LizardShellHelmetItemProperties ItemProperties => new BlueLizardShellHelmetItemProperties();

    public BlueLizardShellHelmetFisob() : base(Enums.AbstractObjectType.BlueLizardShellHelmet)
    {
        RegisterItemPropertiesType(Enums.AbstractObjectType.BlueLizardShellHelmet, ItemProperties);
        RegisterUnlock(Enums.SandboxUnlockID.BlueLizardShellHelmet, parent: MultiplayerUnlocks.SandboxUnlockID.BlueLizard, data: 0);
    }
}