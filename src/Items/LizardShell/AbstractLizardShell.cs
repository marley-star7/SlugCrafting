using Fisobs.Core;
using UnityEngine;

namespace SlugCrafting.Items;

sealed class AbstractLizardShell : AbstractPhysicalObject
{
    public float health;
    public float clampedHealth;

    public Color shellColor;
    public CreatureTemplate.Type templateType;

    public const float MassModifier = 0.5f;

    public string headSprite0Jaw;
    public string headSprite1LowerTeeth;
    public string headSprite2UpperTeeth;
    public string headSprite3Head;
    public string headSprite4Eyes;

    public float headBodyChunkRadius;
    public float headBodyChunkMass;

    private static Dictionary<CreatureTemplate.Type, AbstractObjectType> _creatureTemplateToShellAbstractObjectType = new()
    {
        { CreatureTemplate.Type.GreenLizard, GreenLizardShellFisob.abstractObjectType },
        { CreatureTemplate.Type.PinkLizard, PinkLizardShellFisob.abstractObjectType }
    };
    public static AbstractObjectType GetAbstractObjectTypeForCreatureTemplate(CreatureTemplate.Type templateType)
    {
        if (_creatureTemplateToShellAbstractObjectType.ContainsKey(templateType))
        {
            return _creatureTemplateToShellAbstractObjectType[templateType];
        }
        else
        {
            return LizardShellFisob.abstractObjectType; // Default for lizards that don't have a specific shell type.
        }
    }

    public AbstractLizardShell(World world, CreatureTemplate.Type templateType, WorldCoordinate pos, EntityID ID)
        : base(
            world, GetAbstractObjectTypeForCreatureTemplate(templateType), null, pos, ID)
    {
        this.templateType = templateType;
        var type = GetAbstractObjectTypeForCreatureTemplate(templateType);

        LizardShellProperties properties = LizardShellFisob.properties;

        if (LizardShellProperties.PropertiesOfTemplateType.ContainsKey(templateType))
            properties = LizardShellProperties.PropertiesOfTemplateType[templateType];

        shellColor = properties.ShellColor();

        headSprite0Jaw = properties.HeadSprite0Jaw();
        headSprite1LowerTeeth = properties.HeadSprite1LowerTeeth();
        headSprite2UpperTeeth = properties.HeadSprite2UpperTeeth();
        headSprite3Head = properties.HeadSprite3Head();
        headSprite4Eyes = properties.HeadSprite4Eyes();

        headBodyChunkRadius = properties.HeadBodyChunkRadius();
        headBodyChunkMass = properties.HeadBodyChunkMass() * properties.MassModifier();

        health = properties.Health();
        clampedHealth = properties.Health();
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
}
