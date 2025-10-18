namespace SlugCrafting.Items;

class LizardHeadShell : PlayerCarryableItem, IDrawable
{
    public VelocityRotationModule rotationModule;
    public DonnableMaskItemModule donnableMaskModule;
    public LizardShellEffectsModule lizardShellEffectsModule;

    public override float ThrowPowerFactor => 1f;
    public const int TotalSprites = 5;
    public const int HeadSpritesStart = 3;
    public const float JawOpenSensitivity = 20f;
    public const float JawVelocityOverOpenSensitivity = 2.5f;

    public AbstractLizardHeadShell abstractLizardHeadShell;

    public Vector2 jawRotation;
    public Vector2 lastJawRotation;

    public Vector2 lastGraphicsPos = Vector2.zero;
    public Vector2 graphicsPos = Vector2.zero;

    private bool facingRight;
    private string headAngleNum = "0";

    public const float minSneakToDon = 0.3f;

    public float jawOpenRatio;
    public string[] HeadSprites { get; private set; }

    public LizardHeadShell(AbstractLizardHeadShell abstractPhysicalObject) : base(abstractPhysicalObject)
    {
        abstractLizardHeadShell = abstractPhysicalObject;

        HeadSprites = new string[TotalSprites];

        var pos = abstractLizardHeadShell.Room.realizedRoom.MiddleOfTile(abstractLizardHeadShell.pos.Tile);
        base.bodyChunks = new[] {
            new BodyChunk(this, 0, pos, abstractLizardHeadShell.rad, abstractLizardHeadShell.mass),
        };

        rotationModule = new VelocityRotationModule(this, firstChunk);
        this.AddModule(rotationModule);

        lizardShellEffectsModule = new LizardShellEffectsModule(this, abstractLizardHeadShell.shellColor);
        this.AddModule(lizardShellEffectsModule);

        donnableMaskModule = new DonnableMaskItemModule(this)
        {
            slugcatHandMode = SlugcatHand.Mode.HuntAbsolutePosition
        };
        this.AddModule(donnableMaskModule);

        base.bodyChunkConnections = new BodyChunkConnection[0];
        base.airFriction = 0.97f;
        base.gravity = 0.9f;
        base.bounce = 0.1f;
        base.surfaceFriction = 0.45f;
        base.collisionLayer = 1;
        base.waterFriction = 0.92f;
        base.buoyancy = 0.75f;

        jawRotation = Vector2.zero;
        lastJawRotation = jawRotation;
        facingRight = abstractLizardHeadShell.scaleX > 0;
    }

    public override void Update(bool eu)
    {
        StorePreviousStates();
        base.Update(eu);

        lizardShellEffectsModule.Update();
        lizardShellEffectsModule.effectColorGraphics.brightness = Mathf.InverseLerp(0, abstractLizardHeadShell.maxHealth, abstractLizardHeadShell.health);
        rotationModule.Update();

        UpdateFacingDirection();
        if (grabbedBy.Count != 0)
        {
            HandleGrabbedState();
        }
        else
        {
            donnableMaskModule.donned = 0;
            graphicsPos = firstChunk.pos;
        }
        ResolveTileCollisions();
        UpdateJawRotation();
    }

    private void StorePreviousStates()
    {
        lastGraphicsPos = graphicsPos;
        lastJawRotation = jawRotation;
    }

    private void UpdateFacingDirection()
    {
        facingRight = Custom.VecToDeg(rotationModule.Rotation) > 0;
    }

    protected virtual bool PlayerShouldWear(in Player scug)
    {
        return (scug.bodyMode == Player.BodyModeIndex.Crawl
            || scug.animation == Player.AnimationIndex.CrawlTurn
            || scug.animation == Player.AnimationIndex.LedgeGrab
            || scug.animation == Player.AnimationIndex.LedgeCrawl
            || scug.animation == Player.AnimationIndex.DownOnFours
            || scug.bodyMode == Player.BodyModeIndex.CorridorClimb
            || scug.animation == Player.AnimationIndex.BellySlide
            || scug.animation == Player.AnimationIndex.RocketJump
            );
    }

    private void HandleGrabbedState()
    {
        if (grabbedBy.Count == 0) return;

        if (grabbedBy[0].grabber is Player scug)
        {
            if (PlayerShouldWear(scug))
            {
                HandlePlayerWearing(scug);
            }
            else
            {
                HandlePlayerNotWearing();
            }

            if (donnableMaskModule.donned > 0)
            {
                PositionOverPlayerHead(scug);
            }
            else
            {
                graphicsPos = firstChunk.pos;
            }
        }
        else
        {
            donnableMaskModule.donned = 0;
            graphicsPos = firstChunk.pos;
        }

        UpdateHeadAngleBasedOnRotation();
    }

    private void UpdateHeadAngleBasedOnRotation()
    {
        headAngleNum = Math.Abs(rotationModule.Rotation.x) switch
        {
            < 0.3f => "2",
            < 0.6f => "1",
            _ => "0"
        };
    }

    private void PositionOverPlayerHead(Player scug)
    {
        Vector2 faceDir = Custom.DegToVec(Custom.AimFromOneVectorToAnother(
            scug.bodyChunks[EntityBodyChunkIndexes.Player.Body].pos,
            scug.bodyChunks[EntityBodyChunkIndexes.Player.Head].pos)).normalized;

        // If we are in a corridoor or climbing into shortcut we position the mask graphics back to original pos to look better.
        Vector2 donnedGraphicsTargetPosOffset;

        if (scug.bodyMode == Player.BodyModeIndex.CorridorClimb
            || scug.bodyMode == Player.BodyModeIndex.ClimbIntoShortCut)
        {
            donnedGraphicsTargetPosOffset = Vector2.zero; // Position upward slightly the more horizontal it is.
        }
        else
        {
            donnedGraphicsTargetPosOffset = faceDir * 8f;
        }

        // Tries to smoothly move to position
        // Offset has less / more control the more donned
        graphicsPos = Vector2.Lerp(
            firstChunk.pos, 
            ((PlayerGraphics)scug.graphicsModule).drawPositions[EntityBodyChunkIndexes.Player.Head, 0] + donnedGraphicsTargetPosOffset,
            donnableMaskModule.donned * donnableMaskModule.donned // Squared, so is smaller when smaller
        );

        donnableMaskModule.donnedHandAbsoluteHuntPos = graphicsPos;

        rotationModule.SetRotation = faceDir;
        facingRight = faceDir.x > 0 == abstractLizardHeadShell.scaleX > 0;
    }

    private void HandlePlayerWearing(Player scug)
    {
        donnableMaskModule.donned = Custom.LerpAndTick(donnableMaskModule.donned, 1, 0.1f, 0.1f);
    }

    private void HandlePlayerNotWearing()
    {
        donnableMaskModule.donned = Custom.LerpAndTick(donnableMaskModule.donned, 0, 0.1f, 0.1f);
    }

    private void ResolveTileCollisions()
    {
        var chunk = firstChunk;
        if (!Custom.DistLess(chunk.lastPos, chunk.pos, 3f) && room.GetTile(chunk.pos).Solid && !room.GetTile(chunk.lastPos).Solid)
        {
            var firstSolid = SharedPhysics.RayTraceTilesForTerrainReturnFirstSolid(
                room,
                room.GetTilePosition(chunk.lastPos),
                room.GetTilePosition(chunk.pos));

            if (firstSolid != null)
            {
                HandleTileCollisionResponse(chunk, firstSolid.Value);
            }
        }
    }

    private void HandleTileCollisionResponse(BodyChunk chunk, IntVector2 firstSolid)
    {
        FloatRect collisionRect = Custom.RectCollision(
            chunk.pos,
            chunk.lastPos,
            room.TileRect(firstSolid).Grow(2f));

        chunk.pos = collisionRect.GetCorner(FloatRect.CornerLabel.D);
        AdjustVelocityAfterCollision(chunk, collisionRect);
    }

    private void AdjustVelocityAfterCollision(BodyChunk chunk, FloatRect collisionRect)
    {
        if (collisionRect.GetCorner(FloatRect.CornerLabel.B).x < 0f)
        {
            chunk.vel.x = Mathf.Abs(chunk.vel.x) * 0.15f;
        }
        else if (collisionRect.GetCorner(FloatRect.CornerLabel.B).x > 0f)
        {
            chunk.vel.x = -Mathf.Abs(chunk.vel.x) * 0.15f;
        }
        else if (collisionRect.GetCorner(FloatRect.CornerLabel.B).y < 0f)
        {
            chunk.vel.y = Mathf.Abs(chunk.vel.y) * 0.15f;
        }
        else if (collisionRect.GetCorner(FloatRect.CornerLabel.B).y > 0f)
        {
            chunk.vel.y = -Mathf.Abs(chunk.vel.y) * 0.15f;
        }
    }

    private void UpdateJawRotation()
    {
        if (firstChunk.vel.magnitude <= 0.1f) return;

        Vector2 headForward = rotationModule.Rotation.normalized;
        Vector2 velDir = firstChunk.vel.normalized;

        float velAgainstFacing = Vector2.Dot(headForward, velDir);
        float openAmount = Mathf.Clamp01(1 - velAgainstFacing);
        float velocityFactor = Mathf.Clamp(firstChunk.vel.magnitude / 10f, 0f, 1f);

        float targetJawRotationDegrees = -openAmount * velocityFactor * abstractLizardHeadShell.jawOpenAngle;
        float headRotationDegrees = Custom.VecToDeg(rotationModule.Rotation);

        targetJawRotationDegrees = Mathf.Clamp(
            Mathf.DeltaAngle(headRotationDegrees, targetJawRotationDegrees + headRotationDegrees),
            -abstractLizardHeadShell.jawOpenAngle,
            0f);

        float currentJawRotationDegrees = Custom.VecToDeg(jawRotation);
        currentJawRotationDegrees = Mathf.LerpAngle(
            currentJawRotationDegrees,
            headRotationDegrees + targetJawRotationDegrees,
            velocityFactor + 0.7f);

        jawRotation = Custom.DegToVec(currentJawRotationDegrees);
    }

    public void AddDamage(float damage)
    {
        abstractLizardHeadShell.health -= damage;
        if (abstractLizardHeadShell.health <= 0) Shatter();
    }

    public void Shatter()
    {
        lizardShellEffectsModule.DoShatterEffects(firstChunk.pos);
        Destroy();
    }

    public override void PickedUp(Creature upPicker)
    {
        room.PlaySound(SoundID.Lizard_Light_Terrain_Impact, firstChunk);
        lizardShellEffectsModule.Flicker(20);
    }

    public override void HitByWeapon(Weapon weapon)
    {
        base.HitByWeapon(weapon);

        var damageTook = weapon.HeavyWeapon ? 0.5f : 0.2f;
        AddDamage(damageTook);

        lizardShellEffectsModule.WhiteFlicker(20);
        lizardShellEffectsModule.Flicker(30);

        if (grabbedBy.Count > 0)
        {
            Creature grabber = grabbedBy[0].grabber;
            Vector2 push = firstChunk.vel * firstChunk.mass / grabber.firstChunk.mass;
            grabber.firstChunk.vel += push;
        }

        firstChunk.vel = Vector2.zero;
        lizardShellEffectsModule.DoDeflectEffects(firstChunk, firstChunk.pos, weapon.firstChunk.vel, damageTook, 0);
    }

    public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
    {
        base.TerrainImpact(chunk, direction, speed, firstContact);
        lizardShellEffectsModule.DoTerrainImpactEffects(bodyChunks[chunk], Custom.IntVector2ToVector2(direction), speed, firstContact);
    }

    private static float Rand => UnityEngine.Random.value;

    public void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        sLeaser.sprites = new FSprite[TotalSprites]
        {
            new FSprite(abstractLizardHeadShell.headSprite0Jaw, true),
            new FSprite(abstractLizardHeadShell.headSprite1LowerTeeth, true),
            new FSprite(abstractLizardHeadShell.headSprite2UpperTeeth, true),
            new FSprite(abstractLizardHeadShell.headSprite3Head, true),
            new FSprite(abstractLizardHeadShell.headSprite4Eyes, true),
        };

        for (int i = 0; i < sLeaser.sprites.Length; i++)
        {
            sLeaser.sprites[i].color = abstractLizardHeadShell.shellColor;
            string headSpriteName = sLeaser.sprites[i].element.name;
            HeadSprites[i] = headSpriteName.Remove(headSpriteName.Length - 3, 1);
        }

        AddToContainer(sLeaser, rCam, null);
    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        lizardShellEffectsModule.DrawSpritesUpdate();

        Vector2 pos = Vector2.Lerp(lastGraphicsPos, graphicsPos, timeStacker);
        Vector2 rot = Vector3.Slerp(rotationModule.LastRotation, rotationModule.Rotation, timeStacker);
        Vector2 jawRot = Vector3.Slerp(lastJawRotation, jawRotation, timeStacker);

        float headRot = Custom.VecToDeg(rot);
        float jawRotDeg = Custom.VecToDeg(jawRot);
        Color effectColor = lizardShellEffectsModule.effectColorGraphics.ShellColor();

        UpdateHeadSprites(sLeaser, pos, headRot, effectColor, camPos);
        UpdateJawSprites(sLeaser, pos, jawRotDeg, effectColor, camPos);

        UpdateTeethSpritesColor(sLeaser, effectColor);
        UpdateEyeSpriteColor(sLeaser, effectColor);

        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    protected virtual void UpdateHeadSprites(RoomCamera.SpriteLeaser sLeaser, Vector2 pos, float rotation, Color color, Vector2 camPos)
    {
        for (int i = HeadSpritesStart; i < TotalSprites; i++)
        {
            UpdateSprite(sLeaser, sLeaser.sprites[i], pos, rotation, color, camPos);
        }
    }

    protected virtual void UpdateJawSprites(RoomCamera.SpriteLeaser sLeaser, Vector2 pos, float rotation, Color color, Vector2 camPos)
    {
        for (int i = 0; i < HeadSpritesStart; i++)
        {
            UpdateSprite(sLeaser, sLeaser.sprites[i], pos, rotation, color, camPos);
        }
    }

    protected virtual void UpdateTeethSpritesColor(RoomCamera.SpriteLeaser sLeaser, Color baseColor)
    {
        for (int i = 1; i < HeadSpritesStart; i++)
        {
            sLeaser.sprites[i].color = lizardShellEffectsModule.effectColorGraphics.palette.blackColor;
        }
    }

    protected virtual void UpdateEyeSpriteColor(RoomCamera.SpriteLeaser sLeaser, Color baseColor)
    {
        sLeaser.sprites[TotalSprites - 1].color = Color.Lerp(
            baseColor,
            lizardShellEffectsModule.effectColorGraphics.palette.blackColor,
            1f);
    }

    protected virtual void UpdateSprite(RoomCamera.SpriteLeaser sLeaser, FSprite sprite, Vector2 pos, float rotation, Color color, Vector2 camPos)
    {
        sprite.element = Futile.atlasManager.GetElementWithName(
            HeadSprites[Array.IndexOf(sLeaser.sprites, sprite)]
                .Insert(HeadSprites[Array.IndexOf(sLeaser.sprites, sprite)].Length - 2,
            headAngleNum));

        sprite.x = pos.x - camPos.x;
        sprite.y = pos.y - camPos.y;
        sprite.rotation = rotation;
        sprite.color = color;
        sprite.scaleX = facingRight ? -1 : 1;
    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        lizardShellEffectsModule.effectColorGraphics.ApplyPalette(palette);
    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
    {
        newContainer ??= rCam.ReturnFContainer("Items");
        foreach (FSprite fsprite in sLeaser.sprites)
        {
            fsprite.RemoveFromContainer();
            newContainer.AddChild(fsprite);
        }
    }
}