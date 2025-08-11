namespace SlugCrafting.Items.Weapons;

public sealed class KingVultureSpearProperties : ItemProperties
{
    public override void Throwable(Player player, ref bool throwable)
        => throwable = true;

    public override void ScavCollectScore(Scavenger scavenger, ref int score)
        => score = 5;

    public override void ScavWeaponPickupScore(Scavenger scav, ref int score) // TODO: find out what this is lol?
        => score = 5;

    public override void ScavWeaponUseScore(Scavenger scav, ref int score)
        => score = 5;

    public override void Grabability(Player player, ref Player.ObjectGrabability grabability)
    {
        grabability = Player.ObjectGrabability.BigOneHand;
    }
}