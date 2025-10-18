using CompartmentalizedCreatureGraphics.Extensions;
using Fisobs.Properties;

namespace SlugCrafting.Items;

public class LizardShellCuirassItem : LizardShellArmorItem
{
    public LizardShellCuirass LizardShellCuirass
    {
        get => (LizardShellCuirass)base.lizardShellArmor;
        set => base.lizardShellArmor = value;
    }

    public LizardShellCuirassItem(AbstractLizardShellCuirass abstractLizardShellCuirass, LizardShellCuirass lizardShellCuirass = null) : base(abstractLizardShellCuirass, lizardShellCuirass)
    {
        if (LizardShellCuirass == null)
        {
            LizardShellCuirass = new LizardShellCuirass(abstractLizardShellCuirass, LizardShellCuirassItemProperties.GetPropertiesForType(abstractLizardShellCuirass.type), new LizardShellEffectsModule(this, abstractLizardShellCuirass.shellColor));
        }

        var pos = abstractPhysicalObject.Room.realizedRoom.MiddleOfTile(abstractPhysicalObject.pos.Tile);

        airFriction = 0.97f;
        gravity = 0.9f;
        bounce = 0.1f;
        surfaceFriction = 0.45f;
        collisionLayer = 1;
        waterFriction = 0.92f;
        buoyancy = 0.75f;
    }

    public override void Equip(Player wearer)
    {
        EquipLizardShellArmorAccessory(new LizardShellCuirassAccessory(wearer, LizardShellCuirass));
    }

    public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        if (Wearer != null)
            return; // Use on wearer draw instead.

        Vector2 pos = Vector2.Lerp(firstChunk.lastPos, firstChunk.pos, timeStacker);

        Vector3 rotVec = Vector3.Slerp(lastRotation, rotation, timeStacker);
        float rot = Custom.AimFromOneVectorToAnother(new Vector2(0f, 0f), rotVec);

        string angle = LizardShellCuirass.GetAngleForRotationZ(rotVec.z);
        pos -= camPos; // Offset by camera position to draw in the correct place.

        LizardShellCuirass.DrawSpritesContext context = new LizardShellCuirass.DrawSpritesContext(
            posX: pos.x,
            posY: pos.y,
            scaleX: 1,
            scaleY: 1,
            rotation: rot,
            spriteAngle: "A0",
            anchorX: FSprite.defaultAnchorX,
            anchorY: 0.25f
        );

        LizardShellCuirass.DrawSprites(sLeaser, rCam, timeStacker, camPos, context, context);

        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }
}
