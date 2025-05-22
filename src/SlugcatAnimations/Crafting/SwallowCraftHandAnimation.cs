/*
using RWCustom;
using UnityEngine;

using MRCustom.Animations;
using MRCustom.Math;

namespace SlugCrafting.Animations;

// TODO: make generic interfaces
public class SwallowCraftHandAnimation : MRAnimation<Player>
{
    string _name;
    string MRAnimation<Player>.Name
    {
        get { return _name; }
        set { }
    }

    float _length;
    public float GetLength(Player player)
    {
        return _length;
    }

    public SwallowCraftHandAnimation(string name, float length)
    {
        _name = name;
        _length = length;
    }

    public void Start(Player player)
    {
    }

    public void Stop(Player owner)
    {

    }

    public void Update(Player player, int animationTime)
    {
        for (int i = 0; i < 2; i++)
        {
            player.bodyChunks[0].pos += Custom.DirVec(player.grasps[i].grabbed.firstChunk.pos, player.bodyChunks[0].pos) * 2f;
            (player.graphicsModule as PlayerGraphics).swallowing = 20;
        }
    }

    public void UpdateGraphics(Player player, int animationTime)
    {
        var playerGraphics = (PlayerGraphics)player.graphicsModule;
        var playerCraftingData = player.GetCraftingData();

        if (playerCraftingData.craftTimer > 0)
        {
            foreach (SlugcatHand hand in playerGraphics.hands)
            {
                hand.pos = Vector2.Lerp(hand.pos, playerGraphics.drawPositions[0, 0], playerCraftingData.craftTimer / 25f);
            }

            float num10 = Mathf.InverseLerp(0f, 110f, playerCraftingData.craftTimer);
            float num11 = playerCraftingData.craftTimer / Mathf.Lerp(30f, 15f, num10);
            if (playerGraphics.player.standing)
            {
                playerGraphics.drawPositions[0, 0].y += Mathf.Sin(num11 * 3.1415927f * 2f) * num10 * 2f;
                playerGraphics.drawPositions[1, 0].y += -Mathf.Sin((num11 + 0.2f) * 3.1415927f * 2f) * num10 * 3f;
            }
            else
            {
                playerGraphics.drawPositions[0, 0].y += Mathf.Sin(num11 * 3.1415927f * 2f) * num10 * 3f;
                playerGraphics.drawPositions[0, 0].x += Mathf.Cos(num11 * 3.1415927f * 2f) * num10 * 1f;
                playerGraphics.drawPositions[1, 0].y += Mathf.Sin((num11 + 0.2f) * 3.1415927f * 2f) * num10 * 2f;
                playerGraphics.drawPositions[1, 0].x += -Mathf.Cos(num11 * 3.1415927f * 2f) * num10 * 3f;
            }
        }
    }
}
*/