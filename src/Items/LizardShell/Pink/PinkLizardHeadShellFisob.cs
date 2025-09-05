namespace SlugCrafting.Items;

sealed class PinkLizardHeadShellFisob : LizardHeadShellFisob
{
    private static PinkLizardHeadShellItemProperties _itemProperties = new PinkLizardHeadShellItemProperties();
    public override LizardHeadShellItemProperties ItemProperties => _itemProperties;

    public PinkLizardHeadShellFisob() : base(Enums.AbstractObjectType.PinkLizardHeadShell, CreatureTemplate.Type.PinkLizard)
    {
        Icon = new SmallLizardHeadShellIcon(Color.magenta);

        RegisterUnlock(Enums.SandboxUnlockID.PinkLizardHeadShell, parent: MultiplayerUnlocks.SandboxUnlockID.PinkLizard, data: 0);
    }
}