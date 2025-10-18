namespace SlugCrafting.Modules.Weapons;

public class SeverPoleMimicWeaponStabModule : StabWeaponModule
{
    public SeverPoleMimicWeaponStabModule(Weapon owner) : base(owner)
    {
    }

    public override void DoStabViolence(Creature creatureStabbed, BodyChunk chunkStabbed)
    {
        base.DoStabViolence(creatureStabbed, chunkStabbed);

        /*
        if (creatureStabbed is PoleMimic poleMimic)
        {
            poleMimic.SeverAndCordify();
        }
        */
    }
}
