namespace SlugCrafting.Menus;

/// <summary>
/// In order to differentiate different items in the menu, and store information about them during craft time to reference in-game later.
/// We store that information in this class, which is saved and stuffs.
/// 
/// Most importantly is entityID which is used to reference the original object for removal.
/// </summary>
public class EntityMenuData
{
    public EntityID entityID;
    public EntityTypeDefinition typeDefinition;

    public EntityMenuData(EntityTypeDefinition typeDefinition, EntityID entityID)
    {
        this.entityID = entityID;
        this.typeDefinition = typeDefinition;
    }

    public EntityMenuData(AbstractPhysicalObject abstractPhysicalObject)
    {
        this.entityID = abstractPhysicalObject.ID;
        this.typeDefinition = new EntityTypeDefinition(abstractPhysicalObject);
    }
}