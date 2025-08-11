using Fisobs.Core;
using UnityEngine;

namespace SlugCrafting.Items;

public sealed class AbstractCord : AbstractPhysicalObject
{
    public AbstractPhysicalObject?[] tiedObjects = new AbstractPhysicalObject?[] { null, null};
    public int[] tiedObjectBodyChunkIndexes = new int[] {-1, -1};

    public Color color;

    public CordProperties GetPropertiesForType(AbstractObjectType type)
    {
        if (CordProperties.typesProperties.TryGetValue(type, out var properties))
            return properties;
        else
            return new CordProperties();
    }

    public AbstractCord(AbstractPhysicalObject.AbstractObjectType abstractCordType, World world, WorldCoordinate pos, EntityID ID)
        : base(world, abstractCordType, null, pos, ID)
    {

    }

    public override void Realize()
    {
        var properties = GetPropertiesForType(type);
        
        base.Realize();
        if (realizedObject == null)
            realizedObject = new CordItem(this, properties);
    }

    //public override string ToString()
    //{
    //    return this.SaveToString();
    //}
}