namespace SlugCrafting.Crafts;

// Structs are more performant in loops than classes,
// And we can get away without inheriting from anything here!
public struct HandCraft
{
    /// <summary>
    /// The delegate function that must be used to make the function for returning craft results.
    /// </summary>
    /// <returns></returns>
    public delegate void CraftResult(Creature crafter, PhysicalObject primaryIngredientObject, PhysicalObject secondaryIngredientObject);

    /// <summary>
    /// The function that returns the crafted object.
    /// </summary>
    public CraftResult craftResult;

    private int _totalAnimationLoops;
    public int totalAnimationLoops
    {
        get => _totalAnimationLoops;
    }

    public struct Animation
    {
        public readonly int loopsInAnimation;
        public readonly PlayerHandAnimationPlayer.AnimationIndex animationIndex;

        public Animation(int loopsInAnimation, PlayerHandAnimationPlayer.AnimationIndex animationIndex)
        {
            this.loopsInAnimation = loopsInAnimation;
            this.animationIndex = animationIndex;
        }
    }

    private Animation[] _animations;
    public Animation[] animations
    {
        get => _animations;
        set
        {
            _animations = value;

            _totalAnimationLoops = 0;
            for (int i = 0; i < _animations.Length; i++)
                _totalAnimationLoops += _animations[i].loopsInAnimation;
        }
    }

    public CraftRecipe recipe;

    public CraftRecipe.Ingredient primaryIngredient => recipe.ingredients[0];
    public CraftRecipe.Ingredient secondaryIngredient => recipe.ingredients[1];

    /// <summary>
    /// Wether or not a craft requires the crafter to be standing still or not.
    /// </summary>
    public bool canCraftWhileMoving = true;
    /// <summary>
    /// Wether or not you can craft while doing activities such as climbing, which requires at least one hand holding the pole.
    /// </summary>
    public bool needBothHandsFree = false;

    public HandCraft(CraftRecipe craftRecipe, CraftResult craftResult)
    {
        this.recipe = craftRecipe;
        this.craftResult = craftResult;
    }

    private static bool DefaultValidation(in HandCraft craft, in Creature crafter, in PhysicalObject primaryIngredientObject, in PhysicalObject secondaryIngredientObject) { return true; }

    /// <summary>
    /// Return true if the ingredient is valid.
    /// </summary>
    /// <param name="physicalObject"></param>
    /// <returns></returns>
    public delegate bool ValidateIngredients(in HandCraft craft, in Creature crafter, in PhysicalObject primaryIngredientObject, in PhysicalObject secondaryIngredientObject);

    public ValidateIngredients _ingredientValidation;
    /// <summary>
    /// Should returns true if the ingredients are valid for a craft.
    /// </summary>
    public ValidateIngredients ingredientValidation
    {
        get => _ingredientValidation ?? DefaultValidation;
        set => _ingredientValidation = value;
    }
}
