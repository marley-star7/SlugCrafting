namespace SlugCrafting.Items;

public class LizardHideBackpack
{
    public AbstractPhysicalObject owner;

    public ItemContainer itemContainer;
    public VisibleItemContainerCycler itemContainerCycler;

    public LizardHideBackpack(AbstractPhysicalObject owner)
    {
        this.owner = owner;
        itemContainer = new ItemContainer(owner, 3);
        itemContainerCycler = new VisibleItemContainerCycler(itemContainer);
    }
}
