using UnityEngine;

using MRCustom.Animations;

namespace SlugCrafting.Animations;

public class SwallowCraftPlayerHandAnimation : PlayerHandAnimation
{
    public SwallowCraftPlayerHandAnimation(float length) : base(length)
    {
    }

    protected Player player;
    protected PlayerGraphics playerGraphics;
    protected PlayerCraftingData playerCraftingData;

    public override void Play(Player player)
    {
        this.player = player;
        this.playerGraphics = (PlayerGraphics)player.graphicsModule;
        this.playerCraftingData = player.GetCraftingData();
    }

    public override void GraphicsUpdate(PlayerGraphics playerGraphics)
    {
        if (playerCraftingData.craftTimer > 0)
        {
            foreach (SlugcatHand hand in self.hands)
            {
                hand.pos = Vector2.Lerp(hand.pos, self.drawPositions[0, 0], playerCraftingData.craftTimer / 25f);
            }

            float num10 = Mathf.InverseLerp(0f, 110f, playerCraftingData.craftTimer);
            float num11 = playerCraftingData.craftTimer / Mathf.Lerp(30f, 15f, num10);
            if (self.player.standing)
            {
                self.drawPositions[0, 0].y += Mathf.Sin(num11 * 3.1415927f * 2f) * num10 * 2f;
                self.drawPositions[1, 0].y += -Mathf.Sin((num11 + 0.2f) * 3.1415927f * 2f) * num10 * 3f;
            }
            else
            {
                self.drawPositions[0, 0].y += Mathf.Sin(num11 * 3.1415927f * 2f) * num10 * 3f;
                self.drawPositions[0, 0].x += Mathf.Cos(num11 * 3.1415927f * 2f) * num10 * 1f;
                self.drawPositions[1, 0].y += Mathf.Sin((num11 + 0.2f) * 3.1415927f * 2f) * num10 * 2f;
                self.drawPositions[1, 0].x += -Mathf.Cos(num11 * 3.1415927f * 2f) * num10 * 3f;
            }
        }
    }

    public override void Update(Player player)
    {
        throw new NotImplementedException();
    }
}
