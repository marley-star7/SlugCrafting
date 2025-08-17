using Fisobs.Core;
using UnityEngine;

namespace SlugCrafting.Items;

public class AbstractLizardShell : AbstractPhysicalObject
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
        { CreatureTemplate.Type.GreenLizard, SlugCraftingEnums.AbstractObjectType.GreenLizardShell },
        { CreatureTemplate.Type.PinkLizard, SlugCraftingEnums.AbstractObjectType.PinkLizardShell }
    };

    public AbstractLizardShell(World world, CreatureTemplate.Type templateType, WorldCoordinate pos, EntityID ID)
    : base(
        world, GetAbstractObjectTypeForCreatureTemplate(templateType), null, pos, ID)
    {
        this.templateType = templateType;

        // TODO: later make the fisobs just use the stats from the lizardtemplatetype stuff.

        LizardShellProperties properties;
        if (LizardShellProperties.PropertiesOfTemplateType.ContainsKey(templateType))
            properties = LizardShellProperties.PropertiesOfTemplateType[templateType];
        else
            properties = LizardShellFisob.properties;

        shellColor = properties.defaultShellColor;

        headSprite0Jaw = properties.headSprite0Jaw;
        headSprite1LowerTeeth = properties.headSprite1LowerTeeth;
        headSprite2UpperTeeth = properties.headSprite2UpperTeeth;
        headSprite3Head = properties.headSprite3Head;
        headSprite4Eyes = properties.headSprite4Eyes;

        rad = properties.defaultHeadBodyChunkRadius;
        mass = properties.defaultHeadBodyChunkMass * properties.massModifier;

        health = properties.maxHealth;
        maxHealth = properties.maxHealth;
    }

    public AbstractLizardShell(Lizard lizard) : base(lizard.room.world, GetAbstractObjectTypeForCreatureTemplate(lizard.Template.type), null, lizard.coord, lizard.room.game.GetNewID())
    {
        var lizardGraphics = lizard.graphicsModule as LizardGraphics;
        var sLeaser = lizard.graphicsModule.GetGraphicsModuleCCGData().sLeaser;

        this.templateType = lizard.Template.type;

        LizardShellProperties properties;
        if (LizardShellProperties.PropertiesOfTemplateType.ContainsKey(templateType))
            properties = LizardShellProperties.PropertiesOfTemplateType[templateType];
        else
            properties = LizardShellFisob.properties;

        shellColor = properties.defaultShellColor;

        headSprite0Jaw = sLeaser.sprites[lizardGraphics.SpriteHeadStart].element.name;
        headSprite1LowerTeeth = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 1].element.name;
        headSprite2UpperTeeth = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 2].element.name;
        headSprite3Head = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 3].element.name;
        headSprite4Eyes = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 4].element.name;

        rad = lizard.firstChunk.rad;
        mass = lizard.firstChunk.mass * 0.25f; // -- Ms7: Copy the mass, but make it a little more bearable lol.

        health = properties.maxHealth;
        maxHealth = properties.maxHealth;
    }

    public override void Realize()
    {
        base.Realize();
        if (realizedObject == null)
            realizedObject = new LizardShell(this);
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
            return SlugCraftingEnums.AbstractObjectType.LizardShell; // Default for lizards that don't have a specific shell type.
        }
    }
}
