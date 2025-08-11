namespace SlugCrafting.Core;

// Item bundle contains reference to physical object container, and sets its leaderobject to be the first item in the bundle.
// Objects themselves have a reference to what container they might be in, an item bundle class is different from a storagecontainer class, but both reference an object container
// Storage containers work by checking if an object itself has a container, and adding that object alongside every object in it's container.

// 1 make a base container class that has functionality to hide all the objects collision within it.

// Maybe the slots for a backpack simply hold the data for the primary object, that or item bundles themselves hold data of wether they own a container or not?

// okay so, seperate functionality here, containers should hold just the abstract, and then rebuild when grasping out based of item bundle data?
// Since the priamry item in a bundle is what is stored first anyways, and you always pull out from the first, this should not be a problem I believe.

// So object containers should store data of what objects are inside them, using their abstract data, realizing from that, and then the normal loading for an abstract to be removed.

// But somehow pulling an object out of the container needs to use bundle priority

// Fuck all this noise, make a physical object container class you know you will need using sporeplant as a reference, and maybe that will teach you, abstract the stuff later.

public abstract class PhysicalObjectContainer
{
    public abstract AbstractPhysicalObject leaderObject { get; }

    private List<AbstractPhysicalObject> items;

    public void AddItem(AbstractPhysicalObject item)
    {
        items.Add(item);
        //item.AddConnected
    }
}
