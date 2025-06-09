using System;
using RWCustom;
using UnityEngine;

namespace SlugCrafting.Items.Weapons;
/*
public class BoneKnife : Knife
{
    public BoneKnife(AbstractPhysicalObject abstr, Vector2 pos, Vector2 vel) : base(abstr, pos, vel)
    {
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[2];
        sLeaser.sprites[0] = new FSprite("boneKnifeBlade", true);
        sLeaser.sprites[1] = new FSprite("boneKnifeHandle", true);
        AddToContainer(sLeaser, rCam, null);
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        // Bone color calculation stolen directly from base-game source for Scavenger Outpost skulls. 
        // ( "ScavengerOutpost.cs" as "boneColor" in "ApplyPalette()". )
        HandleColor = palette.blackColor;
        BladeColor = Color.Lerp(palette.blackColor, new Color(0.9f, 0.9f, 0.8f), 0.9f);
    }
}
*/