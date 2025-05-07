using Fisobs.Core;
using UnityEngine;

namespace SlugCrafting.Items;

sealed class AbstractLizardShell : AbstractPhysicalObject
{
    public string headSprite;
    public Color shellColor;
    public AbstractLizardShell(World world, WorldCoordinate pos, EntityID ID)
        : base(world, LizardLeatherFisob.abstractObjectType, null, pos, ID)
    {
        shellColor = Color.green;
        headSprite = "LizardHead1.9";
    }

    public override void Realize()
    {
        base.Realize();
        if (realizedObject == null)
            realizedObject = new LizardShell(this, Room.realizedRoom.MiddleOfTile(pos.Tile));
    }

    public override string ToString()
    {
        return this.SaveToString($"{shellColor};");
    }
}
