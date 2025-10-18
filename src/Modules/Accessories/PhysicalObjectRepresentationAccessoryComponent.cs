namespace SlugCrafting.Modules.Accessories;

/// <summary>
/// This is used to represent the physical object version of the accessory in the world so that other creatures may interact with the accessory as if it was it's object form.
/// Without this, scavengers get crashes trying to target realized objects in a room that aren't there.
/// </summary>
public class AccessoryPhysicalObjectRepresentation : PhysicalObject
{
    public AccessoryPhysicalObjectRepresentation(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject)
    {
        // Empty data
        bodyChunks = new BodyChunk[0];
        bodyChunkConnections = new BodyChunkConnection[0];
    }
}

public class PhysicalObjectRepresentationAccessoryModule : RWModule
{
    public PhysicalObjectRepresentationAccessoryModule(Accessory accessory, AbstractPhysicalObject abstractPhysicalObject) : base(accessory, typeof(PhysicalObjectRepresentationAccessoryModule))
    {
        abstractPhysicalObject.realizedObject = new AccessoryPhysicalObjectRepresentation(abstractPhysicalObject);
    }
}
