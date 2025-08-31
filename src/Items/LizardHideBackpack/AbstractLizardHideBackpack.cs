namespace SlugCrafting.Items;

public class AbstractLizardHideBackpack : AbstractPhysicalObject
{
    public AbstractLizardHideBackpack(World world, WorldCoordinate pos, EntityID ID)
    : base(world, Enums.AbstractObjectType.LizardHideBackpack, null, pos, ID)
    {

    }

    public override void Realize()
    {
        base.Realize();
        if (realizedObject == null)
            realizedObject = new LizardHideBackpackItem(this, new LizardHideBackpack(this));
    }

    //public override string ToString()
    //{
    //    return this.SaveToString();
    //}
}
