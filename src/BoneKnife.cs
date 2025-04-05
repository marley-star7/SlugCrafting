using RWCustom;
using UnityEngine;

namespace SlugCrafting.Items;

public class BoneKnife : Weapon
{
    private Color handleColor;
    private Color bladeColor;

    public BoneKnife(AbstractPhysicalObject abstr, Vector2 pos, Vector2 vel) : base(abstr, abstr.world)
    {
        airFriction = 0.999f;
        gravity = 0.9f;
        bounce = 0.6f;
        surfaceFriction = 0.45f;
        collisionLayer = 1;
        waterFriction = 0.92f;
        buoyancy = 0.75f;
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = new FSprite("boneKnifeBlade", true);
        sLeaser.sprites[1] = new FSprite("boneKnifeHandle", true);
        AddToContainer(sLeaser, rCam, null);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(base.firstChunk.lastPos, base.firstChunk.pos, timeStacker);

        // Loop through and update the sprites.
        for (int i = 0; i < 2; i++)
        {
            sLeaser.sprites[i].x = vector.x - camPos.x;
            sLeaser.sprites[i].y = vector.y - camPos.y;
            sLeaser.sprites[i].rotation = 1;
        }

        sLeaser.sprites[0].color = bladeColor;
        sLeaser.sprites[1].color = handleColor;

        // TODO: need to look how lighting is normally calculated?

        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        // Bone color calculation stolen directly from base-game source for Scavenger Outpost skulls. 
        // ( "ScavengerOutpost.cs" as "boneColor" in "ApplyPalette()". )
        handleColor = palette.blackColor;
        bladeColor = Color.Lerp(palette.blackColor, new Color(0.9f, 0.9f, 0.8f), Mathf.Lerp(0.9f, 0.2f, 1f));
    }

    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContainer)
    {
        newContainer ??= rCam.ReturnFContainer("Items");

        foreach (FSprite fsprite in sLeaser.sprites)
        {
            fsprite.RemoveFromContainer();
            newContainer.AddChild(fsprite);
        }
    }
}
