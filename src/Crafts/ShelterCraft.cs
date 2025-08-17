using MRCustom.Animations;

namespace SlugCrafting.Crafts;

// Structs are more performant in loops than classes,
// And we can get away without inheriting from anything here!
public struct ShelterCraft
{
    /// <summary>
    /// The delegate function that must be used to make the function for returning craft results.
    /// </summary>
    /// <returns></returns>
    public delegate void CraftResult(in ShelterCraft shelterCraft, Creature crafter);

    public ShelterCraft(CraftIngredient[] ingredients, AbstractPhysicalObject.AbstractObjectType craftedObject, CraftResult craftResult)
    {
        this.ingredients = ingredients;
        this.craftedObject = craftedObject;
        this.craftResult = craftResult;
    }

    public CraftIngredient[] ingredients;
    /// <summary>
    /// The type
    /// </summary>
    public AbstractPhysicalObject.AbstractObjectType craftedObject;
    /// <summary>
    /// The function that returns the crafted object.
    /// </summary>
    public CraftResult craftResult;
}
