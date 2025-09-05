namespace SlugCrafting.Crafts;

[Serializable]
public struct ShelterCraftResultData
{
    [Serializable]
    public readonly struct MaterialResultData
    {
        public readonly EntityID entityID;
        public readonly bool consumed;

        public MaterialResultData(EntityID entityID, bool consumed)
        {
            this.entityID = entityID;
            this.consumed = consumed;
        }
    }

    public MaterialResultData[] materials;

    public ushort craftID;

    public WorldCoordinate coord;

    public ShelterCraftResultData(MaterialResultData[] materials, ushort craftID, WorldCoordinate coord)
    {
        this.materials = materials;
        this.craftID = craftID;
        this.coord = coord;
    }
}

public struct ShelterCraft
{
    public CraftRecipe recipe;

    public delegate void CraftResult(in World world, in ShelterCraftResultData shelterCraftResultData);

    public CraftResult craftResult;

    public ShelterCraft(CraftRecipe recipe, CraftResult craftResult)
    {
        this.recipe = recipe;
        this.craftResult = craftResult;
        //this.craftResult = craftResult;
    }
}

public static class ShelterCraftResultDataExtensions
{
    public static AbstractRoom GetAbstractRoomToCraftIn(this ShelterCraftResultData shelterCraftResultData, World world)
    {
        return world.GetAbstractRoom(shelterCraftResultData.coord.room);
    }
}