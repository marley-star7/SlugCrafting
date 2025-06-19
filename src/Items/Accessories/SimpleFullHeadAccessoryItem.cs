using UnityEngine;

namespace SlugCrafting.Items;

public class SimpleFullHeadAccessoryItem : AccessoryItem
{
    public SimpleFullHeadAccessoryItem(AbstractPhysicalObject abstractPhysicalObject) : base(abstractPhysicalObject)
    {
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 pos = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);

        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].x = pos.x - camPos.x;
            sLeaser.sprites[i].y = pos.y - camPos.y;
            sLeaser.sprites[i].rotation = 0;
        }
    }
}
