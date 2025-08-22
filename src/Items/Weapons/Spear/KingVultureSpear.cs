using System.Drawing;
using System.Numerics;
using System.Security.Policy;
using UnityEngine;

namespace SlugCrafting.Items.Weapons;

public class KingVultureSpear : Spear, WeaponsExtension.IWeaponExtension
{
    public bool IsMagnetic => true;

    public AbstractKingVultureSpear abstractKingVultureSpear;

    public Vector2[,] chunkPoints = new Vector2[2, 3];

    public int side = 0;

    private const int tuskSegs = 15;

    public const int totalSprites = 2;

    public int tuskSprite => 0;
    public int tuskDetailSprite => 0;

    public KingVultureSpear(AbstractKingVultureSpear abstractKingVultureSpear)
    : base(abstractKingVultureSpear, abstractKingVultureSpear.world)
    {
        this.GetSpearCraftingData().distancePastCordConnectionDistanceForDislodge = 50f;
        spearDamageBonus = 2;
    }

    public override void Update(bool eu)
    {
        base.Update(eu);
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[totalSprites];

        sLeaser.sprites[tuskSprite] = TriangleMesh.MakeLongMesh(tuskSegs, pointyTip: true, customColor: true);
        sLeaser.sprites[tuskDetailSprite] = TriangleMesh.MakeLongMesh(tuskSegs, pointyTip: true, customColor: true);
        sLeaser.sprites[tuskDetailSprite].shader = rCam.game.rainWorld.Shaders["KingTusk"];

        AddToContainer(sLeaser, rCam, null);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        float num2 = 0f;

        Vector2 rot = Vector3.Slerp(lastRotation, rotation, timeStacker);
        Vector2 chunkPointsPos = (Vector2.Lerp(chunkPoints[0, 1], chunkPoints[0, 0], timeStacker) + Vector2.Lerp(chunkPoints[1, 1], chunkPoints[1, 0], timeStacker)) / 2f;
        Vector2 vector8 = Custom.DirVec(Vector2.Lerp(chunkPoints[1, 1], chunkPoints[1, 0], timeStacker), Vector2.Lerp(chunkPoints[0, 1], chunkPoints[0, 0], timeStacker));
        Vector2 vector9 = Custom.PerpendicularVector(vector8);
        Vector2 vector12 = chunkPointsPos + vector8 * -35f + vector9 * rot.y * ((side == 0) ? (-1f) : 1f) * -15f;

        for (int i = 0; i < tuskSegs; i++)
        {
            float num3 = Mathf.InverseLerp(0f, tuskSegs - 1, i);
            Vector2 vector13 = chunkPointsPos + vector8 * Mathf.Lerp(-30f, 60f, num3) + TuskBend(num3) * vector9 * 20f * rot.x + TuskProfBend(num3) * vector9 * rot.y * ((side == 0) ? (-1f) : 1f) * 10f;
            Vector2 normalized = (vector13 - vector12).normalized;
            Vector2 vector14 = Custom.PerpendicularVector(normalized);
            float num4 = Vector2.Distance(vector13, vector12) / 5f;
            float num5 = TuskRad(num3, Mathf.Abs(rot.y));
            (sLeaser.sprites[tuskSprite] as TriangleMesh).MoveVertice(i * 4, vector12 - vector14 * (num5 + num2) * 0.5f + normalized * num4 - camPos);
            (sLeaser.sprites[tuskSprite] as TriangleMesh).MoveVertice(i * 4 + 1, vector12 + vector14 * (num5 + num2) * 0.5f + normalized * num4 - camPos);
            if (i == tuskSegs - 1)
            {
                (sLeaser.sprites[tuskSprite] as TriangleMesh).MoveVertice(i * 4 + 2, vector13 + normalized * num4 - camPos);
            }
            else
            {
                (sLeaser.sprites[tuskSprite] as TriangleMesh).MoveVertice(i * 4 + 2, vector13 - vector14 * num5 - normalized * num4 - camPos);
                (sLeaser.sprites[tuskSprite] as TriangleMesh).MoveVertice(i * 4 + 3, vector13 + vector14 * num5 - normalized * num4 - camPos);
            }
            num2 = num5;
            vector12 = vector13;
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
