using RWCustom;
using SlugCrafting.Creatures;
using UnityEngine;

namespace SlugCrafting.Items
{
    class LizardHeadShell : PlayerCarryableItem, IDrawable
    {
        public override float ThrowPowerFactor => 1f;
        public const int TotalSprites = 5;
        public const int HeadSpritesStart = 3;
        public const float JawOpenSensitivity = 20f;
        public const float JawVelocityOverOpenSensitivity = 2.5f;

        public AbstractLizardHeadShell abstractLizardHeadShell;
        public LizardEffectColorGraphics lizardShellColorGraphics;

        public Vector2 rotation;
        public Vector2 lastRotation;

        public Vector2 jawRotation;
        public Vector2 lastJawRotation;

        private Vector2 rotVel;
        private bool facingRight;
        private string headAngleNum = "0";

        public float donned;
        public float jawOpenRatio;
        public string[] HeadSprites { get; private set; }

        public LizardHeadShell(AbstractLizardHeadShell abstractPhysicalObject) : base(abstractPhysicalObject)
        {
            abstractLizardHeadShell = abstractPhysicalObject;

            HeadSprites = new string[TotalSprites];
            lizardShellColorGraphics = new LizardEffectColorGraphics(abstractLizardHeadShell.shellColor);

            var pos = abstractLizardHeadShell.Room.realizedRoom.MiddleOfTile(abstractLizardHeadShell.pos.Tile);
            base.bodyChunks = new[] {
                new BodyChunk(this, 0, pos, abstractLizardHeadShell.rad, abstractLizardHeadShell.mass),
            };

            base.bodyChunkConnections = new BodyChunkConnection[0];
            base.airFriction = 0.97f;
            base.gravity = 0.9f;
            base.bounce = 0.1f;
            base.surfaceFriction = 0.45f;
            base.collisionLayer = 1;
            base.waterFriction = 0.92f;
            base.buoyancy = 0.75f;

            rotation = Vector2.zero;
            lastRotation = rotation;
            jawRotation = Vector2.zero;
            lastJawRotation = jawRotation;
            facingRight = abstractLizardHeadShell.scaleX > 0;
        }

        public override void Update(bool eu)
        {
            StorePreviousStates();
            base.Update(eu);
            lizardShellColorGraphics.Update();

            UpdateRotation();
            UpdateFacingDirection();
            HandleGrabbedState();
            HandleGroundCollision();
            UpdateJawRotation();
        }

        private void StorePreviousStates()
        {
            lastRotation = rotation;
            lastJawRotation = jawRotation;
        }

        private void UpdateRotation()
        {
            rotation = Custom.DegToVec(Custom.VecToDeg(rotation) + rotVel.x);
            rotVel = Vector2.ClampMagnitude(rotVel, 50f);
            rotVel *= Custom.LerpMap(rotVel.magnitude, 5f, 50f, 1f, 0.8f);
        }

        private void UpdateFacingDirection()
        {
            facingRight = Custom.VecToDeg(rotation) > 0;
        }

        private void HandleGrabbedState()
        {
            if (grabbedBy.Count == 0) return;

            var grabber = grabbedBy[0].grabber;
            UpdateHeadAngleBasedOnRotation();

            if (grabber is Player scug && scug.privSneak > 0.5f)
            {
                HandlePlayerWearing(scug);
            }
            else
            {
                HandleGenericGrab(grabber);
            }
        }

        private void UpdateHeadAngleBasedOnRotation()
        {
            headAngleNum = Math.Abs(rotation.x) switch
            {
                < 0.3f => "2",
                < 0.6f => "1",
                _ => "0"
            };
        }

        private void HandlePlayerWearing(Player scug)
        {
            Vector2 faceDir = Custom.DegToVec(Custom.AimFromOneVectorToAnother(
                scug.bodyChunks[1].pos,
                scug.bodyChunks[0].pos));

            donned = scug.privSneak;
            rotation = faceDir;
            facingRight = faceDir.x > 0 == abstractLizardHeadShell.scaleX > 0;
        }

        private void HandleGenericGrab(Creature grabber)
        {
            rotation = abstractLizardHeadShell.scaleX < 0
                ? Custom.RotateAroundOrigo(
                    Custom.PerpendicularVector(Custom.DirVec(firstChunk.pos, grabber.mainBodyChunk.pos)), 180)
                : Custom.PerpendicularVector(Custom.DirVec(firstChunk.pos, grabber.mainBodyChunk.pos));

            rotation.y = Mathf.Abs(rotation.y);
            facingRight = rotation.x > 0;
        }

        private void HandleGroundCollision()
        {
            if (firstChunk.ContactPoint.y < 0)
            {
                Vector2 groundRotation = Custom.DegToVec(90f * (facingRight ? 1 : -1));
                rotation = Vector2.Lerp(rotation, groundRotation, UnityEngine.Random.value);
                rotVel *= UnityEngine.Random.value;
            }
            else if (Vector2.Distance(firstChunk.lastPos, firstChunk.pos) > 5f && rotVel.magnitude < 7f)
            {
                rotVel += Custom.RNV() * (Mathf.Lerp(7f, 25f, UnityEngine.Random.value) + firstChunk.vel.magnitude * 2f);
            }

            ResolveTileCollisions();
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

            Vector2 headForward = rotation.normalized;
            Vector2 velDir = firstChunk.vel.normalized;

            float velAgainstFacing = Vector2.Dot(headForward, velDir);
            float openAmount = Mathf.Clamp01(1 - velAgainstFacing);
            float velocityFactor = Mathf.Clamp(firstChunk.vel.magnitude / 10f, 0f, 1f);

            float targetJawRotationDegrees = -openAmount * velocityFactor * abstractLizardHeadShell.jawOpenAngle;
            float headRotationDegrees = Custom.VecToDeg(rotation);

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

        public void HitEffect(Vector2 impactVelocity)
        {
            Color sparkColor = lizardShellColorGraphics.ShellColor(
                abstractLizardHeadShell.health,
                abstractLizardHeadShell.maxHealth);

            for (int k = 0; k < UnityEngine.Random.Range(3, 8); k++)
            {
                Vector2 pos = firstChunk.pos + Custom.DegToVec(Rand * 360f) * 5f * Rand;
                Vector2 vel = -impactVelocity * -0.1f +
                    Custom.DegToVec(Rand * 360f) * Mathf.Lerp(0.2f, 0.4f, Rand) * impactVelocity.magnitude;
                room.AddObject(new Spark(pos, vel, sparkColor, null, 10, 170));
            }

            room.AddObject(new StationaryEffect(
                firstChunk.pos,
                new Color(1f, 1f, 1f),
                null,
                StationaryEffect.EffectType.FlashingOrb));
        }

        public void Shatter()
        {
            room.PlaySound(SoundID.Spear_Fragment_Bounce, firstChunk.pos, 0.35f, 2f);
            for (int k = 0; k < 6; k++)
            {
                room.AddObject(new LizardShellFragment(
                    firstChunk.pos,
                    Custom.RNV() * Mathf.Lerp(5f, 15f, UnityEngine.Random.value),
                    lizardShellColorGraphics.ShellColor(
                        abstractLizardHeadShell.health,
                        abstractLizardHeadShell.maxHealth)));
            }
            Destroy();
        }

        public override void PickedUp(Creature upPicker)
        {
            room.PlaySound(SoundID.Lizard_Light_Terrain_Impact, firstChunk);
            lizardShellColorGraphics.Flicker(20);
        }

        public override void HitByWeapon(Weapon weapon)
        {
            base.HitByWeapon(weapon);
            AddDamage(weapon.HeavyWeapon ? 0.5f : 0.2f);

            lizardShellColorGraphics.WhiteFlicker(20);
            lizardShellColorGraphics.Flicker(30);

            if (grabbedBy.Count > 0)
            {
                Creature grabber = grabbedBy[0].grabber;
                Vector2 push = firstChunk.vel * firstChunk.mass / grabber.firstChunk.mass;
                grabber.firstChunk.vel += push;
            }

            firstChunk.vel = Vector2.zero;
            HitEffect(weapon.firstChunk.vel);
        }

        public override void TerrainImpact(int chunk, IntVector2 direction, float speed, bool firstContact)
        {
            base.TerrainImpact(chunk, direction, speed, firstContact);
            if (speed > 10)
            {
                room.PlaySound(SoundID.Spear_Fragment_Bounce, firstChunk);
                lizardShellColorGraphics.Flicker(20);
            }
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
            lizardShellColorGraphics.DrawSpritesUpdate();

            Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);
            Vector2 rot = Vector3.Slerp(lastRotation, rotation, timeStacker);
            Vector2 jawRot = Vector3.Slerp(lastJawRotation, jawRotation, timeStacker);

            float headRot = Custom.VecToDeg(rot);
            float jawRotDeg = Custom.VecToDeg(jawRot);
            Color effectColor = lizardShellColorGraphics.ShellColor(
                abstractLizardHeadShell.health,
                abstractLizardHeadShell.maxHealth);

            UpdateHeadSprites(sLeaser, pos, headRot, effectColor, camPos);
            UpdateJawSprites(sLeaser, pos, jawRotDeg, effectColor, camPos);

            UpdateTeethSpritesColor(sLeaser, effectColor);
            UpdateEyeSpriteColor(sLeaser, effectColor);

            if (slatedForDeletetion || room != rCam.room)
            {
                sLeaser.CleanSpritesAndRemove();
            }
        }

        private void UpdateHeadSprites(RoomCamera.SpriteLeaser sLeaser, Vector2 pos, float rotation, Color color, Vector2 camPos)
        {
            for (int i = HeadSpritesStart; i < TotalSprites; i++)
            {
                UpdateSprite(sLeaser, sLeaser.sprites[i], pos, rotation, color, camPos);
            }
        }

        private void UpdateJawSprites(RoomCamera.SpriteLeaser sLeaser, Vector2 pos, float rotation, Color color, Vector2 camPos)
        {
            for (int i = 0; i < HeadSpritesStart; i++)
            {
                UpdateSprite(sLeaser, sLeaser.sprites[i], pos, rotation, color, camPos);
            }
        }

        private void UpdateTeethSpritesColor(RoomCamera.SpriteLeaser sLeaser, Color baseColor)
        {
            for (int i = 1; i < HeadSpritesStart; i++)
            {
                sLeaser.sprites[i].color = lizardShellColorGraphics.palette.blackColor;
            }
        }

        private void UpdateEyeSpriteColor(RoomCamera.SpriteLeaser sLeaser, Color baseColor)
        {
            sLeaser.sprites[TotalSprites - 1].color = Color.Lerp(
                baseColor,
                lizardShellColorGraphics.palette.blackColor,
                1f);
        }

        private void UpdateSprite(RoomCamera.SpriteLeaser sLeaser, FSprite sprite, Vector2 pos, float rotation, Color color, Vector2 camPos)
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
            lizardShellColorGraphics.ApplyPalette(palette);
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
}