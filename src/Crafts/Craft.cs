using MRCustom.Animations;
using SlugCrafting.Animations;

namespace SlugCrafting.Crafts
{
    public struct CraftIngredient
    {
        private static bool DefaultValidation(in PhysicalObject physicalObject) { return true; }
        /// <summary>
        /// Return true if the ingredient is valid.
        /// </summary>
        /// <param name="physicalObject"></param>
        /// <returns></returns>
        public delegate bool IngredientValidation(in PhysicalObject physicalObject);

        /// <summary>
        /// The AbstractObjectType of the ingredient.
        /// </summary>
        public AbstractPhysicalObject.AbstractObjectType type;

        private IngredientValidation _validation;
        /// <summary>
        /// Returns true if the ingredient is valid.
        /// </summary>
        public IngredientValidation validation
        {
            get => _validation ?? DefaultValidation;
            set => _validation = value;
        }

        /// <summary>
        /// Wether the ingredient is consumed on craft or not.
        /// </summary>
        public bool consume;

        public CraftIngredient(
            AbstractPhysicalObject.AbstractObjectType type,
            IngredientValidation validation = null,
            bool consume = true
        )
        {
            this.type = type;
            this._validation = validation;
            this.consume = consume;
        }
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
        public delegate AbstractPhysicalObject CraftResult(Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject);

        public Craft(CraftIngredient primaryIngredient, CraftIngredient secondaryIngredient, CraftResult craftResult)
        {
            this.primaryIngredient = primaryIngredient;
            this.secondaryIngredient = secondaryIngredient;
            this.craftResult = craftResult;
        }

        /// <summary>
        /// The function that returns the crafted object.
        /// </summary>
        public CraftResult craftResult;

        public float craftTime;
        /// <summary>
        /// The index of the animation, set's the hand animation to this during crafts.
        /// </summary>
        public PlayerHandAnimationData.HandAnimationIndex handAnimationIndex;
        /// <summary>
        /// Optional use of the player hand animation class, can always leave null if want more custom stuff.
        /// </summary>
        public PlayerHandAnimation handAnimation;

        public CraftIngredient primaryIngredient;
        public CraftIngredient secondaryIngredient;
    }
}
