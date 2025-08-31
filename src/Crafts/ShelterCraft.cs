namespace SlugCrafting.Crafts;

public struct ShelterCraft
{
    public struct ShelterCraftResultDataPackage
    {
        public AbstractRoom abstractRoom;
        public AbstractPhysicalObject[] materialObjects;
        public WorldCoordinate pos;

        public ShelterCraftResultDataPackage(AbstractRoom abstractRoom, AbstractPhysicalObject[] materialObjects)
        {
            this.abstractRoom = abstractRoom;
            this.materialObjects = materialObjects;
        }
    }

    public CraftRecipe recipe;

    public delegate void CraftResult(in ShelterCraftResultDataPackage shelterCraftResultDataPackage);

    public CraftResult craftResult;

    public ShelterCraft(CraftRecipe recipe, CraftResult craftResult)
    {
        this.recipe = recipe;
        this.craftResult = craftResult;
        //this.craftResult = craftResult;
    }
}
