namespace SlugCrafting.Menus;

public class CraftRecipesSelector : PositionedMenuObject
{
    public class CraftRecipeItem : RectangularMenuObject
    {
        public new CraftRecipesSelector owner;

        public const float symbolStartX = 25f;
        public const float labelStartX = 50f;

        public FSprite[] ingredientsSymbolSprites;

        public FSprite resultSymbolSprite;

        public FLabel label;

        public CraftRecipeItem(ObjectIconSymbolProperties[] ingredientsIconSymbolProperties, ObjectIconSymbolProperties resultIconSymbolProperties, Menu.Menu menu, CraftRecipesSelector owner, Vector2 pos) : base(menu, owner, pos, new Vector2(120f,20f))
        {
            var halfOwnerSizeX = owner.size.x / 2f;

            ingredientsSymbolSprites = new FSprite[ingredientsIconSymbolProperties.Length];
            for (int i = 0; i < ingredientsSymbolSprites.Length; i++)
            {
                ingredientsSymbolSprites[i] = new FSprite(ingredientsIconSymbolProperties[i].spriteName, true)
                {
                    color = ingredientsIconSymbolProperties[i].color,
                };

                this.Container.AddChild(ingredientsSymbolSprites[i]);
            }

            this.resultSymbolSprite = new FSprite(resultIconSymbolProperties.spriteName, true)
            {
                color = resultIconSymbolProperties.color,
                x = pos.x - halfOwnerSizeX + symbolStartX,
                y = pos.y,
            };
            this.Container.AddChild(this.resultSymbolSprite);

            this.label = new FLabel(LabelTest.GetFont(false), resultIconSymbolProperties.name)
            {
                alignment = FLabelAlignment.Left,
                color = MenuColorEffect.rgbMediumGrey,
                x = pos.x - halfOwnerSizeX + labelStartX,
                y = pos.y,
            };
            this.Container.AddChild(this.label);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);
        }

        public override void RemoveSprites()
        {
            base.RemoveSprites();
        }
    }

    public List<CraftRecipeItem> craftRecipesItemsList = new();

    public VerticalSlider scrollSlider;

    public ScrollButton scrollUpButton;
    public ScrollButton scrollDownButton;

    public const float distanceFromScreenBorder = 1; // RW ui position starts at the bottom left.

    public float leftLinesPosX => this.pos.x - (size.x / 2);
    public float rightLinesPosX => this.pos.x + (size.x / 2);

    public float craftRecipeItemHeight = 30;

    public float linesCenterY => 0;

    public Vector2 size = new Vector2(250, 500);

    public float allCraftRecipeItemsTotalHeight => craftRecipesItemsList.Count * craftRecipeItemHeight;

    private readonly FSprite[] _sideLines;
    private readonly FSprite _separator;

    public CraftRecipesSelector(Menu.Menu menu, MenuObject owner, Vector2 pos) : base(menu, owner, pos)
    {
        this.menu = menu;

        // -- Ms7: The side lines are just really stretched pixels lol.
        this._sideLines = new FSprite[2];
        this._sideLines[0] = new FSprite("pixel", true)
        {
            anchorX = 0f,
            anchorY = 0f,
            scaleX = 2f,
            x = leftLinesPosX,
            y = pos.y - size.y / 2
        };
        this._sideLines[1] = new FSprite("pixel", true)
        {
            anchorX = 0f,
            anchorY = 0f,
            scaleX = 2f,
            x = leftLinesPosX,
            y = pos.y - size.y / 2
        };

        for (int i = 0; i < _sideLines.Length; i++)
        {
            _sideLines[i].color = MenuColorEffect.rgbMediumGrey;
            this.Container.AddChild(_sideLines[i]);
        }

        this._separator = new FSprite("listDivider", true)
        {
            color = MenuColorEffect.rgbDarkGrey,
            anchorX = 0.5f,
            anchorY = 0.5f,
            scaleX = 1.75f,
            x = pos.x,
            y = 0f,
        };
        this.Container.AddChild(this._separator);

        scrollSlider = new VerticalSlider(
            this.menu,
            this,
            "SCROLL",
            new Vector2(size.x / 2, -size.y / 2),
            new Vector2(0, size.y),
            SlugCraftingEnums.SliderID.CraftRecipesSelectorScroll,
            true
        );
        this.subObjects.Add(scrollSlider);

        scrollUpButton = new ScrollButton(
            this.menu,
            this,
            "UP",
            new Vector2(0, size.y / 2),
            ScrollButton.Direction.Up
        );
        scrollDownButton = new ScrollButton(
            this.menu,
            this,
            "DOWN",
            new Vector2(0, - size.y / 2),
            ScrollButton.Direction.Down
        );

        this.subObjects.Add(scrollUpButton);
        this.subObjects.Add(scrollDownButton);

        AddCraftRecipeItem(AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant);
        AddCraftRecipeItem(AbstractPhysicalObject.AbstractObjectType.DangleFruit);

        // -- Ms7: If there are not enough items to scroll, then disable the scroll buttons and slider.
        if (allCraftRecipeItemsTotalHeight < size.y)
        {
            scrollUpButton.inactive = true;
            scrollDownButton.inactive = true;
            scrollSlider.inactive = true;
        }
    }

    public override void Update()
    {
        // TODO: adding these to subObjetcs might cause them to update, do not need to put them in the updates here then.

        base.Update();
    }

    public override void GrafUpdate(float timeStacker)
    {
        base.GrafUpdate(timeStacker);
        for (int i = 0; i < _sideLines.Length; i++)
        {
            _sideLines[i].scaleY = size.y;
        }
    }

    public override void RemoveSprites()
    {
        base.RemoveSprites();
    }

    public void AddShelterCraft(ShelterCraft shelterCraft)
    {
        AddCraftRecipeItem(shelterCraft.craftedObject);
    }

    private void AddCraftRecipeItem(AbstractPhysicalObject.AbstractObjectType objectType)
    {
        var newCraftRecipeItem = new CraftRecipeItem(new ObjectIconSymbolProperties[0],
            ObjectIconSymbolPropertiesManager.GetObjectIconSymbolProperties(objectType),
            menu,
            this,
            new Vector2(pos.x, GetIdealYPosForItem(craftRecipesItemsList.Count))
        );

        this.craftRecipesItemsList.Add(newCraftRecipeItem);
        this.subObjects.Add(newCraftRecipeItem);
    }

    public float GetIdealYPosForItem(int itemIndex)
    {
        float posY = pos.y + (size.y / 2) - 20;
        posY -= itemIndex * craftRecipeItemHeight;
        return posY;
    }
}