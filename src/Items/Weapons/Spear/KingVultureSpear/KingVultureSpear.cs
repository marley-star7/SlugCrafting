using System;
using System.Drawing;
using System.Numerics;
using System.Security.Policy;
using UnityEngine;

namespace SlugCrafting.Items.Weapons;

public class KingVultureSpear : Spear, WeaponsExtension.IWeaponExtension
{
    public bool IsMagnetic => false;

    public AbstractKingVultureSpear abstractKingVultureSpear;

    public Vector2[,] chunkPoints = new Vector2[2, 3];

    public int side = 0;

    private const int tuskSegs = 15;

    public const int totalSprites = 2;

    public int tuskSprite => 0;
    public int tuskDetailSprite => 1;

    public KingVultureSpear(AbstractKingVultureSpear abstractKingVultureSpear)
    : base(abstractKingVultureSpear, abstractKingVultureSpear.world)
    {
        base.bodyChunks = new BodyChunk[1];
        base.bodyChunks[0] = new BodyChunk(this, 0, new Vector2(0f, 0f), 10f, 0.14f);
        this.GetSpearCraftingData().distancePastCordConnectionDistanceForDislodge = 50f;
        spearDamageBonus = 2;
    }

    Vector2 zRot;
    Vector2 lastRot;
    public override void Update(bool eu)
    {
        base.Update(eu);
        // Sekq: Need to understand, but seting at rotation 0, 0 for now
        chunkPoints[0, 0] = bodyChunks[0].pos;
        chunkPoints[0, 1] = bodyChunks[0].pos;
        chunkPoints[1, 0] = bodyChunks[0].pos;
        chunkPoints[1, 1] = bodyChunks[0].pos;
    }

    public override void PlaceInRoom(Room placeRoom)
    {
        base.PlaceInRoom(placeRoom);
        Reset();
    }

    public override void Thrown(Creature thrownBy, Vector2 thrownPos, Vector2? firstFrameTraceFromPos, IntVector2 throwDir, float frc, bool eu)
    {
        base.Thrown(thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);

    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        
        sLeaser.sprites = new FSprite[totalSprites+1];

        sLeaser.sprites[tuskSprite] = TriangleMesh.MakeLongMesh(tuskSegs, pointyTip: true, customColor: true);
        sLeaser.sprites[tuskDetailSprite] = TriangleMesh.MakeLongMesh(tuskSegs, pointyTip: true, customColor: true);
        sLeaser.sprites[tuskDetailSprite].shader = rCam.game.rainWorld.Shaders["KingTusk"];

        //base.InitiateSprites(sLeaser, rCam);
        //Just a debug circle to see where the spear main chunk is
        sLeaser.sprites[totalSprites] = new FSprite("Circle20");
        sLeaser.sprites[totalSprites].scale = this.bodyChunks[0].rad / 10f;
        AddToContainer(sLeaser, rCam, null);
    }


    // Sekq: Chat-gpt rename var, some can be wrong
    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        float previousRadius = 0f;

        Vector2 rotationLerp = Vector3.Slerp(lastRotation, rotation, timeStacker);

        // Punto central entre los dos chunks principales
        Vector2 tuskBasePos = (Vector2.Lerp(chunkPoints[0, 1], chunkPoints[0, 0], timeStacker)
                             + Vector2.Lerp(chunkPoints[1, 1], chunkPoints[1, 0], timeStacker)) / 2f;

        // Dirección entre los chunks
        Vector2 tuskDirection = Custom.DirVec(
            Vector2.Lerp(chunkPoints[1, 1], chunkPoints[1, 0], timeStacker),
            Vector2.Lerp(chunkPoints[0, 1], chunkPoints[0, 0], timeStacker)
        );

        Plugin.LogGame($"The chunkPoints pos are 00: {chunkPoints[0, 0]}, 01:{chunkPoints[0, 1]}, 10:{chunkPoints[1, 0]}, 11:{chunkPoints[1, 1]}");

        // Vector perpendicular a la dirección
        Vector2 tuskPerp = Custom.PerpendicularVector(tuskDirection);

        // Punto inicial de la defensa
        Vector2 tuskStart = tuskBasePos
                          + tuskDirection * -35f
                          + tuskPerp * rotationLerp.y * ((side == 0) ? -1f : 1f) * -15f;

        for (int i = 0; i < tuskSegs; i++)
        {
            float segmentFactor = Mathf.InverseLerp(0f, tuskSegs - 1, i);

            Vector2 tuskSegment = tuskBasePos
                                + tuskDirection * Mathf.Lerp(-30f, 60f, segmentFactor)
                                + TuskBend(segmentFactor) * tuskPerp * 20f * rotationLerp.x
                                + TuskProfBend(segmentFactor) * tuskPerp * rotationLerp.y * ((side == 0) ? -1f : 1f) * 10f;

            Vector2 tuskSegmentDir = (tuskSegment - tuskStart).normalized;
            Vector2 tuskSegmentPerp = Custom.PerpendicularVector(tuskSegmentDir);

            float segmentLength = Vector2.Distance(tuskSegment, tuskStart) / 5f;
            float segmentRadius = TuskRad(segmentFactor, Mathf.Abs(rotationLerp.y));

            (sLeaser.sprites[tuskSprite] as TriangleMesh).MoveVertice(i * 4,
                tuskStart - tuskSegmentPerp * (segmentRadius + previousRadius) * 0.5f + tuskSegmentDir * segmentLength - camPos);
            (sLeaser.sprites[tuskDetailSprite] as TriangleMesh).MoveVertice(i * 4,
                tuskStart - tuskSegmentPerp * (segmentRadius + previousRadius) * 0.5f + tuskSegmentDir * segmentLength - camPos);

            (sLeaser.sprites[tuskSprite] as TriangleMesh).MoveVertice(i * 4 + 1,
                tuskStart + tuskSegmentPerp * (segmentRadius + previousRadius) * 0.5f + tuskSegmentDir * segmentLength - camPos);
            (sLeaser.sprites[tuskDetailSprite] as TriangleMesh).MoveVertice(i * 4 + 1,
                tuskStart + tuskSegmentPerp * (segmentRadius + previousRadius) * 0.5f + tuskSegmentDir * segmentLength - camPos);

            if (i == tuskSegs - 1)
            {
                (sLeaser.sprites[tuskSprite] as TriangleMesh).MoveVertice(i * 4 + 2,
                    tuskSegment + tuskSegmentDir * segmentLength - camPos);
                (sLeaser.sprites[tuskDetailSprite] as TriangleMesh).MoveVertice(i * 4 + 2,
                    tuskSegment + tuskSegmentDir * segmentLength - camPos);
            }
            else
            {
                (sLeaser.sprites[tuskSprite] as TriangleMesh).MoveVertice(i * 4 + 2,
                    tuskSegment - tuskSegmentPerp * segmentRadius - tuskSegmentDir * segmentLength - camPos);
                (sLeaser.sprites[tuskDetailSprite] as TriangleMesh).MoveVertice(i * 4 + 2,
                    tuskSegment - tuskSegmentPerp * segmentRadius - tuskSegmentDir * segmentLength - camPos);

                (sLeaser.sprites[tuskSprite] as TriangleMesh).MoveVertice(i * 4 + 3,
                    tuskSegment + tuskSegmentPerp * segmentRadius - tuskSegmentDir * segmentLength - camPos);
                (sLeaser.sprites[tuskDetailSprite] as TriangleMesh).MoveVertice(i * 4 + 3,
                    tuskSegment + tuskSegmentPerp * segmentRadius - tuskSegmentDir * segmentLength - camPos);
            }

            previousRadius = segmentRadius;
            tuskStart = tuskSegment;
        }

        // Debug colores
        sLeaser.sprites[totalSprites].color = Color.red;
        sLeaser.sprites[totalSprites].SetPosition(this.bodyChunks[0].pos - camPos);
    }


    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        base.ApplyPalette(sLeaser, rCam, palette);

        HSLColor ColorA = new HSLColor(Mathf.Lerp(0.9f, 1.6f, UnityEngine.Random.value), Mathf.Lerp(0.5f, 0.7f, UnityEngine.Random.value), Mathf.Lerp(0.7f, 0.8f, UnityEngine.Random.value));
        HSLColor ColorB = new HSLColor(ColorA.hue + Mathf.Lerp(-0.25f, 0.25f, UnityEngine.Random.value), Mathf.Lerp(0.8f, 1f, 1f - UnityEngine.Random.value * UnityEngine.Random.value), Mathf.Lerp(0.45f, 1f, UnityEngine.Random.value * UnityEngine.Random.value));
        Color armorColor = Color.Lerp(ColorA.rgb, new Color(1f, 1f, 1f), 0.35f);

        for (int i = 0; i < (sLeaser.sprites[tuskSprite] as TriangleMesh).verticeColors.Length; i++)
        {
            float num = Mathf.InverseLerp(0f, (float)((sLeaser.sprites[tuskSprite] as TriangleMesh).verticeColors.Length - 1), (float)i);
            (sLeaser.sprites[tuskSprite] as TriangleMesh).verticeColors[i] = Color.Lerp(armorColor, Color.white, Mathf.Pow(num, 2f));
            (sLeaser.sprites[tuskDetailSprite] as TriangleMesh).verticeColors[i] = Color.Lerp(Color.Lerp(HSLColor.Lerp(ColorA, ColorB, num).rgb, palette.blackColor, 0.65f - 0.4f * num), armorColor, Mathf.Pow(num, 2f));
        }
    }

    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        if (newContatiner == null)
        {
            newContatiner = rCam.ReturnFContainer("Items");
        }
        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            newContatiner.AddChild(sLeaser.sprites[i]);
        }
    }

    public void Reset()
    {
        for (int i = 0; i < this.chunkPoints.GetLength(0); i++)
        {
            this.chunkPoints[i, 0] = this.bodyChunks[0].pos + Custom.RNV();
            this.chunkPoints[i, 1] = this.chunkPoints[i, 0];
            this.chunkPoints[i, 2] *= 0f;
        }
    }

    //
    // --- Ms7: Source Code Copied Functions ---
    //

    public float TuskBend(float f)
    {
        return Mathf.Sin(Mathf.Pow(f, 0.85f) * (float)Math.PI * 2f) * Mathf.Pow(1f - f, 2f);
    }

    public float TuskProfBend(float f)
    {
        return (0f - Mathf.Cos(Mathf.Pow(f, 0.85f) * (float)Math.PI * 2.5f)) * Mathf.Pow(1f - f, 3f);
    }

    public float TuskRad(float f, float profileFac)
    {
        return 0.5f + 2f * Mathf.Pow(Mathf.Clamp01(Mathf.Sin(Mathf.Pow(f, Mathf.Lerp(0.65f, 0.5f, profileFac)) * (float)Math.PI)), 1.2f - 0.3f * profileFac);
    }
}
