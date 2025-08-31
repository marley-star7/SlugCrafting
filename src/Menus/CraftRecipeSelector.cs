using Menu;
using MRCustom.UI;
using SlugCrafting.Crafts;
using System;
using System.Linq;
using static SlugCrafting.Menus.CraftRecipesSelector;

namespace SlugCrafting.Menus;

// TODO: will need an inventory record struct or something to store the items change after crafting to be removed after wakeup.
// Checking the inventory before and after will not do, just less efficient.

public class CraftRecipesSelector : ScrollableItemList<CraftRecipeItem>
{
    /// <summary>
    /// An individual visual representation of a craft recipe in the scrolling list.
    /// </summary>
    public class CraftRecipeItem : ScrollableItemList<CraftRecipeItem>.Item
    {
        /// <summary>
        /// The button that covers the entire item.
        /// </summary>
        public class Button : ButtonTemplate
        {
            public Button(Menu.Menu menu, CraftRecipeItem owner) : base(menu, owner, new Vector2(0, -owner.size.y / 2), owner.size)
            {
            }

            public override void Clicked()
            {
                Singal(this, "CRAFTRESULTCLICKED");
            }
        }

        public new CraftRecipesSelector owner => (CraftRecipesSelector)base.owner;

        public Button button;

        public FSprite[] ingredientsSymbolSprites;

        public FSprite resultSymbolSprite;

        public FSprite[] bodyModeRequirementSymbols;

        public FLabel label;

        public CraftRecipe recipe;

        private Color symbolColor;

        public const float SymbolsGapDistance = 6;
        public const float FirstSymbolGapExtraGapDistance = 3;
        public const float MaxIngredientSymbolPixelSize = 36;

        public const float FirstSymbolStartDistance = SideLinesPixelSizeX + SymbolsGapDistance + FirstSymbolGapExtraGapDistance + MaxIngredientSymbolPixelSize / 2;
        public const float LabelStartDistance = FirstSymbolStartDistance + MaxIngredientSymbolPixelSize / 2 + SymbolsGapDistance;

        public const float MaxBodyModeRequirementSymbolPixelSize = 24;
        public const float BodyModeRequirementSymbolStartDistance = SideLinesPixelSizeX + SymbolsGapDistance + FirstSymbolGapExtraGapDistance + MaxBodyModeRequirementSymbolPixelSize / 2;

        public const int LayeredSymbolsOffsetDistance = 6;

        public float fadeAway = 0f;

        private float fade;
        private float lastFade;

        public CraftRecipeItem(CraftRecipesSelector owner, int index, CraftRecipe recipe) : base(owner, index)
        {
            this.recipe = recipe;

            var ingredientsIconSymbolProperties = recipe.ingredients.Select(ingredient => ObjectIconSymbolPropertiesManager.GetObjectIconSymbolProperties(ingredient.material.objectType)).ToArray();
            var resultIconSymbolProperties = ObjectIconSymbolPropertiesManager.GetObjectIconSymbolProperties(recipe.resultedObjects[0].objectType);

            this.symbolColor = resultIconSymbolProperties.color;

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
            };
            this.Container.AddChild(this.resultSymbolSprite);

            this.label = new FLabel(LabelTest.GetFont(false), resultIconSymbolProperties.name)
            {
                alignment = FLabelAlignment.Left,
            };
            this.Container.AddChild(this.label);

            this.button = new Button(menu, this);
            this.subObjects.Add(this.button);

            //AddBodyModeRequirementSymbols();
        }

        protected virtual void AddBodyModeRequirementSymbols()
        {
            var halfOwnerSizeX = ((CraftRecipesSelector)owner).size.x / 2f;

            switch (recipe.bodyModeRequirement)
            {
                case CraftRecipe.BodyModeRequirement.Any:
                    bodyModeRequirementSymbols = new FSprite[]
                    {
                        new FSprite("craftBodyModeRequirementSymbol_Stand", true)
                        {
                            color = Color.white
                        },
                        new FSprite("craftBodyModeRequirementSymbol_Sneak", true)
                        {
                            color = Color.gray
                        },
                    };
                    break;

                case CraftRecipe.BodyModeRequirement.Stand:
                    bodyModeRequirementSymbols = new FSprite[]
                    {
                        new FSprite("craftBodyModeRequirementSymbol_Stand", true)
                        {
                            color = Color.white
                        },
                    };
                    break;

                case CraftRecipe.BodyModeRequirement.Sneak:
                    bodyModeRequirementSymbols = new FSprite[]
                    {
                        new FSprite("craftBodyModeRequirementSymbol_Sneak", true)
                        {
                            color = Color.white
                        },
                    };
                    break;
            }

            for (int i = 0; i < bodyModeRequirementSymbols.Length; i++)
            {
                bodyModeRequirementSymbols[i].x = pos.x + halfOwnerSizeX - BodyModeRequirementSymbolStartDistance - LayeredSymbolsOffsetDistance * i;
                bodyModeRequirementSymbols[i].y = pos.y + LayeredSymbolsOffsetDistance * i;
                this.Container.AddChild(bodyModeRequirementSymbols[i]);
            }
        }

        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);

            float baseResultSymbolSpriteAlpha;
            Menu.Menu.MenuColors baseLabelColor;

            //-- Ms7: Icons being "Darker" is just them being half transparent.
            if (button.buttonBehav.greyedOut)
            {
                baseResultSymbolSpriteAlpha = 0.5f;
                baseLabelColor = Menu.Menu.MenuColors.DarkGrey;
            }
            else
            {
                baseResultSymbolSpriteAlpha = 1f;
                baseLabelColor = Menu.Menu.MenuColors.MediumGrey;
            }

            label.color = button.InterpColor(timeStacker, Menu.Menu.MenuColor(Menu.Menu.MenuColors.MediumGrey)); // InterpColor() is some fancy schmancy default function used for most buttons flashing and color lerping in this game.

            // Set pos
            var halfOwnerSizeX = owner.size.x / 2f;

            // -- Ms7: As subobjects copy the pos of the owner for select things, you may use this to get the true global position of an item on screen,
            // Which drawPos uses screenPos for global position, as well as uses timeStacker for smooothness.
            var drawPos = DrawPos(timeStacker);
            var alpha = Mathf.Lerp(lastFade, fade, timeStacker);

            for (int i = 0; i < ingredientsSymbolSprites.Length; i++)
            {
                ingredientsSymbolSprites[i].x = drawPos.x - FirstSymbolStartDistance - ((MaxIngredientSymbolPixelSize + SymbolsGapDistance) * i);
                ingredientsSymbolSprites[i].y = drawPos.y;
                ingredientsSymbolSprites[i].alpha = alpha;
            }

            resultSymbolSprite.x = drawPos.x + FirstSymbolStartDistance;
            resultSymbolSprite.y = drawPos.y;
            resultSymbolSprite.alpha = baseResultSymbolSpriteAlpha * alpha;

            label.x = drawPos.x + LabelStartDistance;
            label.y = drawPos.y;
            label.alpha = alpha;
        }

        public override void RemoveSprites()
        {
            base.RemoveSprites();

            for (int i = 0; i < ingredientsSymbolSprites.Length; i++)
            {
                this.Container.RemoveChild(ingredientsSymbolSprites[i]);
            }
            ingredientsSymbolSprites = new FSprite[0];

            for (int i = 0; i < bodyModeRequirementSymbols.Length; i++)
            {
                this.Container.RemoveChild(bodyModeRequirementSymbols[i]);
            }
            bodyModeRequirementSymbols = new FSprite[0];

            this.Container.RemoveChild(resultSymbolSprite);
            resultSymbolSprite = null;
            this.Container.RemoveChild(label);
            label = null;
        }
    }

    private Inventory _playerInventory;

    public CraftRecipeElaborationDisplay? craftRecipeElaborationDisplay;

    private readonly FSprite _separator;

    public static float GetConstrainedSizeY(float singleItemHeight, float sizeY)
    {
        return sizeY - (sizeY % singleItemHeight);
    }

    public static Vector2 GetDefaultSize => new Vector2(200, GetConstrainedSizeY(SingleCraftRecipeItemHeight, 600));

    public const float SingleCraftRecipeItemHeight = 32 + 6; // 32 is for size of symbol, 6 is for some extra space between.

    public override float GetSingleItemHeight => SingleCraftRecipeItemHeight;

    public CraftRecipesSelector(Menu.Menu menu, MenuObject owner, Vector2 pos, Vector2 size) : base(menu, owner, pos, size, Enums.SliderID.CraftRecipesSelectorScroll, true)
    {
        this.menu = menu;

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
    }

    public override void Singal(MenuObject sender, string message)
    {
        base.Singal(sender, message);
        if (message == "CRAFTRESULTCLICKED")
        {
            ShowCraftRecipeInformation((sender as CraftRecipesSelector.CraftRecipeItem).recipe);
            return;
        }
    }

    //--- Ms7: Cache of craft recipe availability to avoid recalculating every time resort, since uses dictionary can lookup fast (thank you hashing).
    private HashSet<CraftRecipe> _cachedCraftableRecipes = new();

    public void SortCraftRecipeItemsByAvaliability()
    {
        var newOrder = Content.CraftRecipes
            .OrderByDescending(recipe => recipe.IsShelterCraft())
            .ThenBy(recipe => _cachedCraftableRecipes.Contains(recipe))
            .ThenBy(recipe => ObjectIconSymbolPropertiesManager.GetObjectIconSymbolProperties(
                recipe.resultedObjects[0]).name)
            .ToList();

        ReorderCraftRecipeItems(newOrder);
    }

    public void ReorderCraftRecipeItems(List<CraftRecipe> newOrder)
    {
        ClearItems();

        // Re-add items in the new order
        for (int i = 0; i < newOrder.Count; i++)
        {
            AddCraftRecipe(newOrder[i]);
        }

        scrollableBehav.ResetScrollPos();
    }

    public void SetPlayerInventory(Inventory inventory)
    {
        this._playerInventory = inventory;
        UpdateAvailabilityCache();
        SortCraftRecipeItemsByAvaliability();
    }

    protected virtual void AddCraftRecipe(CraftRecipe recipe)
    {
        var craftRecipeItem = new CraftRecipeItem
        (
            this,
            items.Count,
            recipe
        );
        AddItem(craftRecipeItem);
    }

    protected override void PostAddItem(CraftRecipeItem craftRecipeItem)
    {
        craftRecipeItem.button.buttonBehav.greyedOut = !_cachedCraftableRecipes.Contains(craftRecipeItem.recipe);
    }

    protected void ShowCraftRecipeInformation(in CraftRecipe recipe)
    {
        craftRecipeElaborationDisplay = new CraftRecipeElaborationDisplay(recipe, menu, this, this.pos + new Vector2(200, 0), new Vector2(600, 900));
        this.subObjects.Add(craftRecipeElaborationDisplay);
    }

    public virtual bool IsRecipeCraftable(CraftRecipe recipe)
    {
        return _playerInventory.CanCraftRecipe(recipe);
    }

    protected void UpdateAvailabilityCache()
    {
        Plugin.LogDebug($"Updating avaliability cache in CraftRecipeSelector");

        for (int i = 0; i < Content.CraftRecipes.Count; i++)
        {
            var recipe = Content.CraftRecipes[i];

            if (IsRecipeCraftable(recipe))
                _cachedCraftableRecipes.Add(recipe);
            else
                _cachedCraftableRecipes.Remove(recipe);
        }
    }
}