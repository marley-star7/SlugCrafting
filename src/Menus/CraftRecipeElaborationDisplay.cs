
namespace SlugCrafting.Menus;

public class CraftRecipeElaborationDisplay : RectangularMenuObject
{
    public new CraftRecipesSelector owner => (CraftRecipesSelector)base.owner;

    public FSprite craftObjectSymbol;

    public FLabel craftObjectTitle;

    public BigSimpleButton? craftButton;

    public CraftRecipe recipe;

    public EntityTypeSymbolProperties craftObjectSymbolProperties;

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

        this.craftObjectSymbolProperties = EntityTypeSymbolPropertiesManager.GetEntityTypeSymbolProperties(resultObject);

        AddCraftObjectSymbol();
        AddCraftObjectTitle();
    }

    protected void AddCraftObjectSymbol()
    {
        craftObjectSymbol = new FSprite(craftObjectSymbolProperties.spriteName, true)
        {
            color = craftObjectSymbolProperties.color,
            x = topLeftPosX + 40,
            y = topLeftPosY + 40
        };
        this.Container.AddChild(craftObjectSymbol);
    }

    protected void AddCraftObjectTitle()
    {
        craftObjectTitle = new FLabel(LabelTest.GetFont(true), craftObjectSymbolProperties.name)
        {
            x = topLeftPosX + 120,
            y = topLeftPosY + 40
        };
        this.Container.AddChild(craftObjectTitle);
    }

    protected void AddCraftButton()
    {
        var craftButtonPos = new Vector2(40, 40);
        var craftButtonSize = new Vector2(90, 32);
        craftButton = new BigSimpleButton(menu, this, "Craft", "CRAFT", craftButtonPos, craftButtonSize, FLabelAlignment.Center, true);

        this.subObjects.Add(craftButton);
    }


    public override void RemoveSprites()
    {
        base.RemoveSprites();

        this.Container.RemoveChild(craftObjectSymbol);
        this.Container.RemoveChild(craftObjectTitle);

        this.subObjects.Clear();
    }
}
