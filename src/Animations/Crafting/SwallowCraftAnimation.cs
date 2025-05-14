using RWCustom;
using UnityEngine;

namespace SlugCrafting.Animations.Crafting
{
    public class SwallowCraftAnimation : HandAnimation
    {
        public override void PlaySlugcatAnimation(Player scug, int firstObjectGraspIndex, int secondObjectGraspIndex, int animationTime)
        {
            for (int i = 0; i < 2; i++)
            {
                scug.bodyChunks[0].pos += Custom.DirVec(scug.grasps[i].grabbed.firstChunk.pos, scug.bodyChunks[0].pos) * 2f;
                (scug.graphicsModule as PlayerGraphics).swallowing = 20;
            }
        }

        public override void PlaySlugcatAnimationGraphics(PlayerGraphics scugGraphics)
        {
            var playerCraftingData = scugGraphics.player.GetCraftingData();

            if (playerCraftingData.craftTimer > 0)
            {
                foreach (SlugcatHand hand in scugGraphics.hands)
                {
                    hand.pos = Vector2.Lerp(hand.pos, scugGraphics.drawPositions[0, 0], playerCraftingData.craftTimer / 25f);
                }

                float num10 = Mathf.InverseLerp(0f, 110f, playerCraftingData.craftTimer);
                float num11 = playerCraftingData.craftTimer / Mathf.Lerp(30f, 15f, num10);
                if (scugGraphics.player.standing)
                {
                    scugGraphics.drawPositions[0, 0].y += Mathf.Sin(num11* 3.1415927f * 2f) * num10 * 2f;
                    scugGraphics.drawPositions[1, 0].y += -Mathf.Sin((num11 + 0.2f) * 3.1415927f * 2f) * num10 * 3f;
                }
                else
                {
                    scugGraphics.drawPositions[0, 0].y += Mathf.Sin(num11 * 3.1415927f * 2f) * num10 * 3f;
                    scugGraphics.drawPositions[0, 0].x += Mathf.Cos(num11 * 3.1415927f * 2f) * num10 * 1f;
                    scugGraphics.drawPositions[1, 0].y += Mathf.Sin((num11 + 0.2f) * 3.1415927f * 2f) * num10 * 2f;
                    scugGraphics.drawPositions[1, 0].x += -Mathf.Cos(num11 * 3.1415927f * 2f) * num10 * 3f;
                }
            }
        }
    }
}
