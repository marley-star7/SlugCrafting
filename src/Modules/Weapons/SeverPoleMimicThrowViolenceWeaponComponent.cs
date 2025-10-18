
namespace SlugCrafting.Modules.Weapons;

public class SeverPoleMimicThrowViolenceWeaponModule : ThrowViolenceWeaponModule
{
    public SeverPoleMimicThrowViolenceWeaponModule(Weapon owner) : base(owner)
    {
    }

    public override void HitCreature(BodyChunk source, Creature hitCreature, Vector2 directionAndMomentum, BodyChunk hitChunk, PhysicalObject.Appendage.Pos hitAppendage)
    {
        base.HitCreature(source, hitCreature, directionAndMomentum, hitChunk, hitAppendage);
        if (hitCreature is PoleMimic poleMimic)
        {
            poleMimic.SeverAndCordify(hitAppendage);
        }
    }
}
