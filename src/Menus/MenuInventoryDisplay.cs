namespace SlugCrafting.Menus;

public class MenuInventoryDisplay : PositionedMenuObject
{
    public class ItemDisplay : ButtonTemplate
    {
        public new MenuInventoryDisplay owner => (MenuInventoryDisplay)base.owner;

        public new Vector2 pos
        {
            get => base.pos;
            set => SetPosition(value);
        }

        public static readonly Vector2 ButtonSize = new Vector2(32, 32);

        public FSprite iconSymbolSprite;

        public FLabel amountLabel;

        public int amount;

        public ItemDisplay(EntityTypeSymbolProperties entityTypeSymbolProperties, int amount, Menu.Menu menu, MenuInventoryDisplay owner, Vector2 pos) : base(menu, owner, pos, ButtonSize)
        {
            this.amount = amount;
            iconSymbolSprite = new FSprite(entityTypeSymbolProperties.spriteName, true)
            {
                color = entityTypeSymbolProperties.color,
                x = pos.x,
                y = pos.y
            };

            amountLabel = new FLabel(LabelTest.GetFont(false), amount.ToString())
            {
                color = entityTypeSymbolProperties.color,
                scale = 1f,
                x = pos.x,
                y = pos.y
            };

            this.Container.AddChild(iconSymbolSprite);
            this.Container.AddChild(amountLabel);
        }

        private void SetPosition(Vector2 newPos)
        {
            base.pos = newPos;
            iconSymbolSprite.x = newPos.x;
            iconSymbolSprite.y = newPos.y;
            amountLabel.x = newPos.x + 16; // Offset a bit to the right
            amountLabel.y = newPos.y - 16; // Offset a bit down
        }
    }

    private MenuInventory _inventory;

    private List<ItemDisplay> _itemDisplays = new List<ItemDisplay>();

    public MenuInventoryDisplay(Menu.Menu menu, MenuObject owner, Vector2 pos) : base(menu, owner, pos)
    {

    }

    public override void RemoveSprites()
    {
        base.RemoveSprites();
        for (int i = 0; i < _itemDisplays.Count; i++)
        {
            _itemDisplays[i].RemoveSprites();
        }
    }

    private void CreateItemDisplaysForInventory()
    {
        foreach (KeyValuePair<EntityTypeDefinition, List<EntityMenuData>> itemEntry in _inventory.Items)
        {
            EntityTypeSymbolProperties entityTypeSymbolProperties = EntityTypeSymbolPropertiesManager.GetEntityTypeSymbolProperties(itemEntry.Key);

            ItemDisplay itemDisplay = new ItemDisplay(entityTypeSymbolProperties, itemEntry.Value.Count, menu, this, new Vector2(0, 0));
            AddItemDisplay(itemDisplay);
        }
    }

    public void RecreateItemDisplays()
    {
        ClearItemDisplays();
        CreateItemDisplaysForInventory();
    }

    public void SetInventory(MenuInventory inventory)
    {
        _inventory = inventory;
        _inventory.PostItemRemoved += OnInventoryPostItemRemoved;

        RecreateItemDisplays();
    }

    private void OnInventoryPostItemRemoved(EntityMenuData obj)
    {
        RecreateItemDisplays();
    }

    private Vector2 GetIdealPosForItemIndex(int index)
    {
        float spacingX = 40f; // Space between items

        float x = pos.x + spacingX * index;
        float y = pos.y;

        return new Vector2(x, y);
    }

    private void AddItemDisplay(ItemDisplay itemDisplay)
    {
        _itemDisplays.Add(itemDisplay);
        itemDisplay.pos = GetIdealPosForItemIndex(_itemDisplays.Count - 1);

        this.subObjects.Add(itemDisplay);
    }

    private void ClearItemDisplays()
    {
        this.subObjects.RemoveAll(itemDisplay => itemDisplay is ItemDisplay);
        _itemDisplays.Clear();
    }
}
