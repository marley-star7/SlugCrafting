//-- MS7: Just like base game, functionality and graphics are seperate classes.
// Cosmetics are the accessory graphics, while the gameplay function code resides in the accessory.

namespace SlugCrafting.Core;

public class Accessory : UpdatableAndDeletable, IDrawable, IDynamicCreatureCosmetic
{
    /// <summary>
    /// The enum of areas that a cosmetic can be equipped to.
    /// Used to decide what accessories can be worn together with this.
    /// </summary>
    public enum EquipRegion
    {
        None,
        Ears,
        Eyes,
        Nose,
        Head,
        Neck,
        Torso,
        Waist,
        Back,
        Hands,
        Feet,
        Cloak,
    }

    /// <summary>
    /// The areas the accessory counts as equipping to.
    /// Use this to decide what accessories can or cannot be worn together with this.
    /// </summary>
    public EquipRegion[] equipRegions = new[]
    {
        EquipRegion.None
    };

    public enum HideRegion
    {
        None,
        Head,
        LeftEar,
        RightEar,
        LeftEye,
        RightEye,
        Nose,
    }

    /// <summary>
    /// Regions of the body that will be hidden by this cosmetic.
    /// Hiding a body region simply makes it invisible.
    /// </summary>
    public HideRegion[] hideRegions = new[]
    {
        HideRegion.None,
    };

    protected RoomCamera.SpriteLeaser? _sLeaser;
    public RoomCamera.SpriteLeaser? sLeaser => _sLeaser;

    private Player _wearer;
    public Creature wearer => _wearer;
    public GraphicsModule? wearerGraphics => _wearer.graphicsModule;

    public Player owner => _wearer as Player;

    public int wearingBodyChunkIndex = 0; // Default body chunk index for wearables, can be overridden in derived classes.
    public float mass = 0f;

    private SpriteLayerGroup[] _spriteLayerGroups;
    public SpriteLayerGroup[] spriteLayerGroups
    {
        get => _spriteLayerGroups;
        set => _spriteLayerGroups = value;
    }

    public Accessory(Player owner)
    {
        this._wearer = owner;
        var wearerCraftingData = _wearer.GetPlayerCraftingData();
        var wearerCCGData = _wearer.graphicsModule.GetGraphicsModuleCCGData();

        wearerCraftingData.accessories.Add(this);
        // Equip this accessory to all equip regions.
        for (int i = 0; i < equipRegions.Length; i++)
        {
            if (wearerCraftingData.equipRegionAccessories.ContainsKey(equipRegions[i]))
            {
                var existingAccessory = wearerCraftingData.equipRegionAccessories[equipRegions[i]];
                //TODO: add unequip functionality here, or that scug just says (no)
            }
            else
            {
                wearerCraftingData.equipRegionAccessories[equipRegions[i]] = this;
            }
        }

        owner.UpdateMass();
    }

    /*
    ~Accessory()
    {
        var wearerCraftingData = _wearer.GetPlayerCraftingData();
        wearerCraftingData.accessories.Remove(this);

        Wearer.graphicsModule.RemoveCreatureCosmetic(this);
    }
    */

    public virtual bool PreSpearHitWearer(Spear spear, SharedPhysics.CollisionResult result, bool eu)
    {
        return true;
    }

    public virtual void PreWearerViolence(ViolenceContext violenceContext)
    {

    }

    public virtual void PostWearerGrabbed(Creature.Grasp grasp)
    {

    }

    public virtual void PostWearerTerrainImpact(Player player, int chunk, IntVector2 direction, float speed, bool firstContact)
    {

    }

    public virtual void PostWearerCollide(Player player, PhysicalObject otherObject, int myChunk, int otherChunk)
    {

    }

    //
    // IDRAWABLE
    //

    public virtual void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
    {
        this._sLeaser = sLeaser;
    }

    public void PostWearerInitiateSprites(RoomCamera.SpriteLeaser wearerSLeaser, RoomCamera rCam)
    {

    }

    public void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {
        this._sLeaser = sLeaser;

        if (slatedForDeletetion || room != rCam.room)
        {
            sLeaser.CleanSpritesAndRemove();
        }
    }

    public virtual void PostWearerDrawSprites(RoomCamera.SpriteLeaser wearerSLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
    {

    }

    public void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        this._sLeaser = sLeaser;
    }

    public virtual void PostWearerApplyPalette(RoomCamera.SpriteLeaser wearerSLeaser, RoomCamera rCam, in RoomPalette palette)
    {

    }

    public void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContainer)
    {

    }
}