using MRCustom.Animations;

namespace SlugCrafting.Crafts
{
    // Structs are more performant in loops than classes,
    // And we can get away without inheriting from anything here!
    public struct ShelterCraft
    {
        /// <summary>
        /// The delegate function that must be used to make the function for returning craft results.
        /// </summary>
        /// <returns></returns>
        public delegate AbstractPhysicalObject CraftResult(Creature crafter);

        public ShelterCraft(CraftIngredient[] ingredients, CraftResult craftResult, int craftTime)
        {
            this.ingredients = ingredients;
            this.craftResult = craftResult;
            this.craftTime = craftTime;
        }

        /// <summary>
        /// The function that returns the crafted object.
        /// </summary>
        public CraftResult craftResult;

        public string animation;

        public CraftIngredient[] ingredients;

        public int craftTime;
    }
}
