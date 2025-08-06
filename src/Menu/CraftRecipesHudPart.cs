using HUD;

namespace SlugCrafting.Menu;

public class CraftRecipesHudPart : HudPart
{
    public Vector2 pos = new Vector2(80f, 20f);
    public Vector2 lastPos;

    private Player player => hud.owner as Player;

    private FContainer hudFContainer;

    public CraftRecipesHudPart(HUD.HUD hud, FContainer fContainer)
    : base(hud)
    {
        lastPos = pos;
        hudFContainer = fContainer;
    }

    public override void Update()
    {
        
    }
}
