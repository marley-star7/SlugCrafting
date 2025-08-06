namespace SlugCrafting.CosmeticSprites;

public class LizardShellFragment : CosmeticSprite
{
    public float rotation;
    public float lastRotation;

    public float rotVel;

    public Spear owner;

    public Color color;

    public LizardShellFragment(Vector2 pos, Vector2 vel, Color color)
        : this(pos, vel, null)
    {
        this.color = color;
    }

    public LizardShellFragment(Vector2 pos, Vector2 vel, Spear owner)
    {
        base.pos = pos + vel * 2f;
        lastPos = pos;
        base.vel = vel;
        this.owner = owner;
        rotation = Random.value * 360f;
        lastRotation = rotation;
        rotVel = Mathf.Lerp(-26f, 26f, Random.value);
    }

    public override void Update(bool eu)
    {
        vel *= 0.999f;
        vel.y -= room.gravity * 0.9f;
        lastRotation = rotation;
        rotation += rotVel * vel.magnitude;
        if (Vector2.Distance(lastPos, pos) > 18f && room.GetTile(pos).Solid && !room.GetTile(lastPos).Solid)
        {
            IntVector2? intVector = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(room, room.GetTilePosition(lastPos), room.GetTilePosition(pos));
            FloatRect floatRect = Custom.RectCollision(pos, lastPos, room.TileRect(intVector.Value).Grow(2f));
            pos = floatRect.GetCorner(FloatRect.CornerLabel.D);
            bool flag = false;
            if (floatRect.GetCorner(FloatRect.CornerLabel.B).x < 0f)
            {
                vel.x = Mathf.Abs(vel.x) * 0.5f;
                flag = true;
            }
            else if (floatRect.GetCorner(FloatRect.CornerLabel.B).x > 0f)
            {
                vel.x = (0f - Mathf.Abs(vel.x)) * 0.5f;
                flag = true;
            }
            else if (floatRect.GetCorner(FloatRect.CornerLabel.B).y < 0f)
            {
                vel.y = Mathf.Abs(vel.y) * 0.5f;
                flag = true;
            }
            else if (floatRect.GetCorner(FloatRect.CornerLabel.B).y > 0f)
            {
                vel.y = (0f - Mathf.Abs(vel.y)) * 0.5f;
                flag = true;
            }
            if (flag)
            {
                rotVel *= 0.8f;
                rotVel += Mathf.Lerp(-1f, 1f, Random.value) * 4f * Random.value;
                room.PlaySound(SoundID.Spear_Fragment_Bounce, pos, (owner != null) ? owner.abstractPhysicalObject : null);
            }
        }
        if ((room.GetTile(pos).Solid && room.GetTile(lastPos).Solid) || pos.x < -100f)
        {
            Destroy();
        }
        base.Update(eu);
    }

    public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[1];
        sLeaser.sprites[0] = new FSprite("SpearFragment" + (1 + Random.Range(0, 2)));
        sLeaser.sprites[0].color = color;
        sLeaser.sprites[0].scaleX = ((Random.value < 0.5f) ? (-1f) : 1f);
        sLeaser.sprites[0].scaleY = ((Random.value < 0.5f) ? (-1f) : 1f);
        AddToContainer(sLeaser, rCam, null);
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        Vector2 vector = Vector2.Lerp(lastPos, pos, timeStacker);
        sLeaser.sprites[0].x = vector.x - camPos.x;
        sLeaser.sprites[0].y = vector.y - camPos.y;
        sLeaser.sprites[0].rotation = Mathf.Lerp(lastRotation, rotation, timeStacker);
        base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
    }

    public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        sLeaser.sprites[0].color = palette.blackColor;
    }

    public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
    {
        base.AddToContainer(sLeaser, rCam, newContatiner);
    }
}