using MRCustom.Animations;
using SlugCrafting.Animations;

namespace SlugCrafting.Crafts
{
    public struct CraftIngredient
    {
        /// <summary>
        /// The AbstractObjectType of the ingredient.
        /// </summary>
        public AbstractPhysicalObject.AbstractObjectType type;

        public CraftIngredient(
            AbstractPhysicalObject.AbstractObjectType type
        )
        {
            this.type = type;
        }
    }

    // Structs are more performant in loops than classes,
    // And we can get away without inheriting from anything here!
    public struct Craft
    {
        /// <summary>
        /// The delegate function that must be used to make the function for returning craft results.
        /// </summary>
        /// <returns></returns>
        public delegate void CraftResult(Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject);

        /// <summary>
        /// Return true if the ingredient is valid.
        /// </summary>
        /// <param name="physicalObject"></param>
        /// <returns></returns>
        public delegate bool ValidateIngredients(in PhysicalObject primaryIngredientObject, in PhysicalObject secondaryIngredientObject);

        private static bool DefaultValidation(in PhysicalObject primaryIngredientObject, in PhysicalObject secondaryIngredientObject) { return true; }

        public Craft(CraftIngredient primaryIngredient, CraftIngredient secondaryIngredient, CraftResult craftResult)
        {
            this.primaryIngredient = primaryIngredient;
            this.secondaryIngredient = secondaryIngredient;
            this.craftResult = craftResult;
        }

        public ValidateIngredients _ingredientValidation;

        /// <summary>
        /// Should returns true if the ingredients are valid for a craft.
        /// </summary>
        public ValidateIngredients ingredientValidation
        {
            get => _ingredientValidation ?? DefaultValidation;
            set => _ingredientValidation = value;
        }

        /// <summary>
        /// The function that returns the crafted object.
        /// </summary>
        public CraftResult craftResult;

        public float craftTime;
        /// <summary>
        /// The index of the animation, set's the hand animation to this during crafts.
        /// </summary>
        public PlayerHandAnimationPlayer.HandAnimationIndex handAnimationIndex;
        /// <summary>
        /// Optional use of the player hand animation class, can always leave null if want more custom stuff.
        /// </summary>
        public MRAnimation<Player> handAnimation;

        public CraftIngredient primaryIngredient;
        public CraftIngredient secondaryIngredient;
    }
}
