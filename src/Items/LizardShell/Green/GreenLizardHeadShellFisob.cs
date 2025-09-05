namespace SlugCrafting.Items;

sealed class GreenLizardHeadShellFisob : LizardHeadShellFisob
{
    private static GreenLizardHeadShellProperties _itemProperties = new GreenLizardHeadShellProperties();
    public override LizardHeadShellItemProperties ItemProperties => _itemProperties;

    public GreenLizardHeadShellFisob() : base(Enums.AbstractObjectType.GreenLizardHeadShell, CreatureTemplate.Type.GreenLizard)
    {
        Icon = new BigLizardHeadShellIcon(Color.green);

        RegisterUnlock(Enums.SandboxUnlockID.GreenLizardHeadShell, parent: MultiplayerUnlocks.SandboxUnlockID.GreenLizard, data: 0);
    }
}