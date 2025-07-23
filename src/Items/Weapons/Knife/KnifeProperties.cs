using Fisobs.Properties;

namespace SlugCrafting.Items.Weapons;

sealed class KnifeProperties : ItemProperties
{
    public override void Throwable(Player player, ref bool throwable)
        => throwable = true;

    public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 3;

    public override void ScavWeaponPickupScore(Scavenger scav, ref int score) // TODO: find out what this is lol?
        => score = 3;

    // Don't throw the knife.
    public override void ScavWeaponUseScore(Scavenger scav, ref int score)
        => score = 2;

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
    {
        grabability = Player.ObjectGrabability.OneHand;
    }
}