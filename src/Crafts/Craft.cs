using SlugCrafting.Animations;

namespace SlugCrafting.Crafts
{
    public struct CraftIngredient
    {
        public AbstractPhysicalObject.AbstractObjectType type;
        public bool consume;
    }

    // Structs are more performant in loops than classes,
    // And we can get away without inheriting from anything here!
    public struct Craft
    {
        // TODO: move this delegate to a seperate file, "crafting" or something?
        /// <summary>
        /// The delegate function that must be used to make the function for returning craft results.
        /// </summary>
        /// <returns></returns>
        public delegate AbstractPhysicalObject CraftResult(Creature crafter);

        public Craft(CraftIngredient primaryIngredient, CraftIngredient secondaryIngredient, CraftResult craftResult, int craftTime)
        {
            this.primaryIngredient = primaryIngredient;
            this.secondaryIngredient = secondaryIngredient;
            this.craftResult = craftResult;
            this.craftTime = craftTime;
        }

        /// <summary>
        /// The function that returns the crafted object.
        /// </summary>
        public CraftResult craftResult;

        public HandAnimation animation;

        public CraftIngredient primaryIngredient;
        public CraftIngredient secondaryIngredient;

        public int craftTime;
    }
}
