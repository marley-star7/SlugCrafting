using Fisobs.Core;
using UnityEngine;

namespace SlugCrafting.Items;

public class AbstractLizardHeadShell : AbstractPhysicalObject
{
    public float health;
    public float maxHealth;

    public Color shellColor;
    public CreatureTemplate.Type templateType;

    public const float MassModifier = 0.5f;

    public string headSprite0Jaw;
    public string headSprite1LowerTeeth;
    public string headSprite2UpperTeeth;
    public string headSprite3Head;
    public string headSprite4Eyes;

    // TODO: need to save and get the scaleX
    public float scaleX = 1f;
    public float scaleY = 1f;

    // TODO: and this
    public float jawOpenAngle = 90;
    public float jawOpenMoveJawsApart = 30;

    // TODO: and this
    public float rad;
    public float mass;

    private static Dictionary<CreatureTemplate.Type, AbstractObjectType> _creatureTemplateToShellAbstractObjectType = new()
    {
        { CreatureTemplate.Type.GreenLizard, Enums.AbstractObjectType.GreenLizardHeadShell },
        { CreatureTemplate.Type.PinkLizard, Enums.AbstractObjectType.PinkLizardHeadShell }
    };

    public AbstractLizardHeadShell(World world, CreatureTemplate.Type templateType, WorldCoordinate pos, EntityID ID)
    : base(
        world, GetAbstractObjectTypeForCreatureTemplate(templateType), null, pos, ID)
    {
        this.templateType = templateType;

        // TODO: later make the fisobs just use the stats from the lizardtemplatetype stuff.

        LizardHeadShellItemProperties itemProperties = GetItemPropertiesForTemplateType(templateType);

        shellColor = itemProperties.DefaultShellColor;

        headSprite0Jaw = itemProperties.HeadSprite0Jaw;
        headSprite1LowerTeeth = itemProperties.HeadSprite1LowerTeeth;
        headSprite2UpperTeeth = itemProperties.HeadSprite2UpperTeeth;
        headSprite3Head = itemProperties.HeadSprite3Head;
        headSprite4Eyes = itemProperties.HeadSprite4Eyes;

        rad = itemProperties.DefaultHeadBodyChunkRadius;
        mass = itemProperties.DefaultHeadBodyChunkMass * itemProperties.MassModifier;

        health = itemProperties.MaxHealth;
        maxHealth = itemProperties.MaxHealth;
    }

    public AbstractLizardHeadShell(Lizard lizard) : base(lizard.room.world, GetAbstractObjectTypeForCreatureTemplate(lizard.Template.type), null, lizard.coord, lizard.room.game.GetNewID())
    {
        var lizardGraphics = lizard.graphicsModule as LizardGraphics;
        var sLeaser = lizard.graphicsModule.GetGraphicsModuleCCGData().sLeaser;

        this.templateType = lizard.Template.type;

        shellColor = lizard.lizardParams.standardColor;

        headSprite0Jaw = sLeaser.sprites[lizardGraphics.SpriteHeadStart].element.name;
        headSprite1LowerTeeth = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 1].element.name;
        headSprite2UpperTeeth = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 2].element.name;
        headSprite3Head = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 3].element.name;
        headSprite4Eyes = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 4].element.name;

        scaleX = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 3].scaleX;
        scaleY = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 3].scaleY;

        jawOpenAngle = lizard.lizardParams.jawOpenAngle;
        jawOpenMoveJawsApart = lizard.lizardParams.jawOpenMoveJawsApart;

        rad = lizard.bodyChunks[EntityBodyChunkIndexes.Lizard.Head].rad;
        mass = lizard.bodyChunks[EntityBodyChunkIndexes.Lizard.Head].mass * 0.25f; // -- Ms7: Copy the mass, but make it a little more bearable lol.

        LizardHeadShellItemProperties itemProperties = GetItemPropertiesForTemplateType(templateType);

        health = itemProperties.MaxHealth;
        maxHealth = itemProperties.MaxHealth;
    }

    public override void Realize()
    {
        base.Realize();
        if (realizedObject == null)
            realizedObject = new LizardHeadShell(this);
    }

    public override string ToString()
    {
        return this.SaveToString($"{shellColor}");
    }

    //
    //-- MS7: My lovely seperater, you wouldn't seperate me from my seperator would you?
    //

    public static AbstractObjectType GetAbstractObjectTypeForCreatureTemplate(CreatureTemplate.Type templateType)
    {
        if (_creatureTemplateToShellAbstractObjectType.ContainsKey(templateType))
        {
            return _creatureTemplateToShellAbstractObjectType[templateType];
        }
        else
        {
            return Enums.AbstractObjectType.LizardHeadShellTemplate; // Default for lizards that don't have a specific shell type.
        }
    }

    public static LizardHeadShellItemProperties GetItemPropertiesForTemplateType(in CreatureTemplate.Type templateType)
    {
        LizardHeadShellItemProperties itemProperties;
        if (LizardHeadShellItemProperties.PropertiesOfTemplateType.TryGetValue(templateType, out itemProperties))
        {
            return itemProperties;
        }
        else
        {
            Plugin.LogDebug($"No Head Shell Item Properties assigned for TemplateType{templateType.ToString()}, using default");
            return LizardHeadShellItemProperties.PropertiesOfTemplateType[CreatureTemplate.Type.LizardTemplate];
        }
    }
}
