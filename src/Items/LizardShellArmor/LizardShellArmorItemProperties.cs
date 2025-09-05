namespace SlugCrafting.Items;

public abstract class LizardShellArmorItemProperties : ItemProperties, IAccessoryItemProperties
{
    protected LizardShellArmorAccessoryProperties _accessoryProperties;
    public LizardShellArmorAccessoryProperties ArmorAccessoryProperties
    {
        get => _accessoryProperties;
    }

    public AccessoryProperties AccessoryProperties => _accessoryProperties;

    public LizardShellArmorItemProperties(LizardShellArmorAccessoryProperties lizardShellArmorAccessoryProperties)
    {
        this._accessoryProperties = lizardShellArmorAccessoryProperties;
    }

    public override void Throwable(Player player, ref bool throwable)
        => throwable = true;

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
        => grabability = Player.ObjectGrabability.OneHand;
}
