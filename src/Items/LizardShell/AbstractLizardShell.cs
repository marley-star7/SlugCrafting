using Fisobs.Core;
using UnityEngine;

namespace SlugCrafting.Items;

sealed class AbstractLizardShell : AbstractPhysicalObject
{
    public Color shellColor;

    public const float MassModifier = 0.5f;

    public string headSprite0Jaw;
    public string headSprite1LowerTeeth;
    public string headSprite2UpperTeeth;
    public string headSprite3Head;
    public string headSprite4Eyes;

    public float headBodyChunkRadius;
    public float headBodyChunkMass;

    // TODO: later find out a way to get the lizard shell properties via the type dynamically instead of hardcoding them here. (so its more easily moddable to add lizard shells)

    public AbstractLizardShell(World world, AbstractCreature.AbstractObjectType type, WorldCoordinate pos, EntityID ID)
        : base(world, type, null, pos, ID)
    {
        if (type == GreenLizardShellFisob.abstractObjectType)
        {
            shellColor = Color.green;
        }
        else if (type == PinkLizardShellFisob.abstractObjectType)
        {
            var properties = PinkLizardShellFisob.properties;

            shellColor = properties.ShellColor();

            headSprite0Jaw = properties.HeadSprite0Jaw();
            headSprite1LowerTeeth = properties.HeadSprite1LowerTeeth();
            headSprite2UpperTeeth = properties.HeadSprite2UpperTeeth();
            headSprite3Head = properties.HeadSprite3Head();
            headSprite4Eyes = properties.HeadSprite4Eyes();

            headBodyChunkRadius = properties.HeadBodyChunkRadius(); // Default for Pink Lizards
            headBodyChunkMass = properties.HeadBodyChunkMass() * properties.MassModifier(); // Default for Pink Lizards
        }
    }

    public override void Realize()
    {
        base.Realize();
        if (realizedObject == null)
            realizedObject = new LizardShell(this, Room.realizedRoom.MiddleOfTile(pos.Tile));
    }

    public override string ToString()
    {
        return this.SaveToString($"{shellColor}");
    }
}
