using System;
using RWCustom;
using UnityEngine;

namespace SlugCrafting.Items;

class LizardShell : PlayerCarryableItem, IDrawable
{
    public override float ThrowPowerFactor => 0.5f;

    public int totalSprites;

    public readonly AbstractLizardShell abstractLizardShell;
    public LizardShell(AbstractLizardShell abstractPhysicalObject, Vector2 pos) 
        : base(abstractPhysicalObject)
    {
        abstractLizardShell = abstractPhysicalObject;

        base.bodyChunks = new[] {
            new BodyChunk(this, 0, pos, 3f, 0.14f),
            new BodyChunk(this, 0, pos - new Vector2(3f, 0), 3f, 0.14f),
            new BodyChunk(this, 0, pos + new Vector2(3f, 0), 3f, 0.14f)
        };

        // Cloth is made up of 3 small chunks to fake physics.
        base.bodyChunkConnections = new BodyChunkConnection[] {
            new BodyChunkConnection(
                bodyChunks[0],
                bodyChunks[1],
                3f,
                BodyChunkConnection.Type.Normal,
                0.14f,
                0.3f
            ),
            new BodyChunkConnection(
                bodyChunks[1],
                bodyChunks[2],
                3f,
                BodyChunkConnection.Type.Normal,
                0.14f,
                0.3f
            )
        };

        totalSprites = 1;

        base.airFriction = 0.97f;
        base.gravity = 0.9f;
        base.bounce = 0.1f;
        base.surfaceFriction = 0.45f;
        base.collisionLayer = 0;
        base.waterFriction = 0.92f;
        base.buoyancy = 0.75f;
    }
    public override void Update(bool eu)
    {
        base.Update(eu);
    }

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite(abstractLizardShell.headSprite, true);
        AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 pos = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);

        // Loop through and update the sprites.
        for (int i = 0; i < totalSprites; i++)
        {
            sLeaser.sprites[i].x = pos.x - camPos.x;
            sLeaser.sprites[i].y = pos.y - camPos.y;
        }

        sLeaser.sprites[0].color = abstractLizardShell.shellColor;

        // TODO: need to look how lighting is normally calculated?

        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
    {
        newContainer ??= rCam.ReturnFContainer("Items");
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].RemoveFromContainer();
            newContainer.AddChild(sLeaser.sprites[i]);
        }
    }
}
