namespace SlugCrafting.Menus;

[Serializable]
public struct SerializablePerformedShelterCraft
{
    public EntityID[] materialObjects;
    public int craftID;

    public int region;
    public int room;

    public SerializablePerformedShelterCraft(EntityID[] materialObjects, ushort craftID, int region, int room)
    {
        this.materialObjects = materialObjects;
        this.craftID = craftID;
        this.region = region;
        this.room = room;
    }
}
