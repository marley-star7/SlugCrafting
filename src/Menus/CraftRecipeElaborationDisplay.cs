
namespace SlugCrafting.Menus;

public class CraftRecipeElaborationDisplay : RectangularMenuObject
{
    public CraftRecipesSelector owner => (CraftRecipesSelector)base.owner;

    public FSprite craftObjectSymbol;

    public FLabel craftObjectTitle;

    public BigSimpleButton? craftButton;

    public CraftRecipe recipe;

    public ObjectIconSymbolProperties craftObjectSymbolProperties;

    private float topLeftPosX => pos.x - size.x / 2;
    private float topLeftPosY => pos.y - size.y / 2;

    /// <summary>
    /// Creates information for a craft recipe.
    /// </summary>
    /// <param name="recipe"></param>
    /// <param name="menu"></param>
    /// <param name="owner"></param>
    /// <param name="pos"></param>
    /// <param name="size"></param>
    public CraftRecipeElaborationDisplay(CraftRecipe recipe, Menu.Menu menu, CraftRecipesSelector owner, Vector2 pos, Vector2 size) : base(menu, owner, pos, size)
    {
        this.recipe = recipe;
        var resultObject = recipe.resultedObjects[0];

        this.craftObjectSymbolProperties = ObjectIconSymbolPropertiesManager.GetObjectIconSymbolProperties(resultObject);

        AddCraftObjectSymbol();
        AddCraftObjectTitle();
    }

    private void AddCraftObjectSymbol()
    {
        craftObjectSymbol = new FSprite(craftObjectSymbolProperties.spriteName, true)
        {
            color = craftObjectSymbolProperties.color,
            x = topLeftPosX + 40,
            y = topLeftPosY + 40
        };
        this.Container.AddChild(craftObjectSymbol);
    }

    private void AddCraftObjectTitle()
    {
        craftObjectTitle = new FLabel(LabelTest.GetFont(true), craftObjectSymbolProperties.name)
        {
            x = topLeftPosX + 120,
            y = topLeftPosY + 40
        };
        this.Container.AddChild(craftObjectTitle);
    }

    public override void RemoveSprites()
    {
        base.RemoveSprites();

        this.Container.RemoveChild(craftObjectSymbol);
        this.Container.RemoveChild(craftObjectTitle);

        this.subObjects.Clear();
    }
}
