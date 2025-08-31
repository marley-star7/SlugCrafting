
namespace SlugCrafting.Menus;

public class InventoryDisplay : PositionedMenuObject
{
    public class ItemDisplay : ButtonTemplate
    {
        public new InventoryDisplay owner => (InventoryDisplay)base.owner;

        public new Vector2 pos
        {
            get => base.pos;
            set => SetPosition(value);
        }

        public static readonly Vector2 ButtonSize = new Vector2(32, 32);

        public FSprite iconSymbolSprite;

        public FLabel amountLabel;

        public int amount;

        public ItemDisplay(ObjectIconSymbolProperties iconSymbolProperties, int amount, Menu.Menu menu, InventoryDisplay owner, Vector2 pos) : base(menu, owner, pos, ButtonSize)
        {
            this.amount = amount;
            iconSymbolSprite = new FSprite(iconSymbolProperties.spriteName, true)
            {
                color = iconSymbolProperties.color,
                x = pos.x,
                y = pos.y
            };

            amountLabel = new FLabel(LabelTest.GetFont(false), amount.ToString())
            {
                color = iconSymbolProperties.color,
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

    private Inventory _inventory;

    private List<ItemDisplay> _itemDisplays = new List<ItemDisplay>();

    public InventoryDisplay(Menu.Menu menu, MenuObject owner, Vector2 pos) : base(menu, owner, pos)
    {

    }

    public void SetInventory(Inventory inventory)
    {
        _inventory = inventory;
        foreach(KeyValuePair<ObjectDefinition, List<AbstractPhysicalObject>> itemEntry in _inventory.Items)
        {
            ObjectIconSymbolProperties iconSymbolProperties = ObjectIconSymbolPropertiesManager.GetObjectIconSymbolProperties(itemEntry.Key);

            ItemDisplay itemDisplay = new ItemDisplay(iconSymbolProperties, itemEntry.Value.Count, menu, this, new Vector2(0, 0));
            AddItemDisplay(itemDisplay);
        }
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
}
