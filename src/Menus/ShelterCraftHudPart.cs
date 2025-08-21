using HUD;

namespace SlugCrafting.Menus;

/*
public class ShelterCraftHudPart : HudPart
{
    public class ShelterCraftSelector
    {
        public class Item : RectangularMenuObject
        {
            public ObjectIconSymbol symbol;

            public Item(ObjectIconSymbolProperties iconProperties, Menu.Menu menu, MenuObject owner, Vector2 pos, Vector2 size) : base(menu, owner, pos, size)
            {
                this.symbol = new ObjectIconSymbol(iconProperties, this.Container);
                this.symbol.Show();
            }

            public override void Update()
            {
                base.Update();
                this.symbol.Update();
                this.symbol.showFlash = 1;
            }

            public override void GrafUpdate(float timeStacker)
            {
                base.GrafUpdate(timeStacker);
                this.symbol.Draw(this.DrawPos(timeStacker) + base.DrawSize(timeStacker) / 2f, timeStacker);
            }

            public override void RemoveSprites()
            {
                base.RemoveSprites();
                this.symbol.RemoveSprites();
            }
        }

        public Item[] items;

        public ShelterCraftSelector()
        {

        }
    }

    public ShelterCraftSelector shelterCraftSelector;

    public ShelterCraftHudPart(HUD.HUD hud, FContainer fContainer) : base(hud)
    {
        shelterCraftSelector = new ShelterCraftSelector();
        shelterCraftSelector.items = new ShelterCraftSelector.Item[]
        {
            new ShelterCraftSelector.Item(ObjectIconSymbolPropertiesManager.GetObjectIconSymbolProperties(AbstractPhysicalObject.AbstractObjectType.KarmaFlower), ),
        };
    }
}
*/