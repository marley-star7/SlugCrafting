using SlugCrafting.Animations.Scavenging;
using SlugCrafting.Items;

namespace SlugCrafting.Scavenges.ScavengeSpots
{
    public class LizardShellScavenge : AbstractPhysicalObjectScavenge
    {
        AbstractPhysicalObject.AbstractObjectType lizardShellType;

        public LizardShellScavenge(Lizard lizard, AbstractPhysicalObject.AbstractObjectType lizardShellType) : base(lizard)
        {
            this.lizardShellType = lizardShellType;
        }

        public override AbstractPhysicalObject Scavenge()
        {
            var lizard = owner as Lizard;
            var lizardGraphics = lizard.graphicsModule as LizardGraphics;
            var sLeaser = lizardGraphics.GetGraphicsData().spriteLeaser;

            var shell =  new AbstractLizardShell(lizard.room.world, lizardShellType, lizard.coord, lizard.room.game.GetNewID())
            {
                // Copy all the data of the lizard to the scavenged shell.
                shellColor = lizardGraphics.effectColor,
                headBodyChunkRadius = lizard.firstChunk.rad,
                headBodyChunkMass = lizard.firstChunk.mass * 0.4f, // Temporary value, TODO: find a way to get the mass modifier of the lizard head chunk associated with the shell type dynamically.

                headSprite0Jaw = sLeaser.sprites[lizardGraphics.SpriteHeadStart].element.name,
                headSprite1LowerTeeth = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 1].element.name,
                headSprite2UpperTeeth = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 2].element.name,
                headSprite3Head = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 3].element.name,
                headSprite4Eyes = sLeaser.sprites[lizardGraphics.SpriteHeadStart + 4].element.name,
            };
            canScavenge = false; // Prevents the scavenge from being used again.

            return shell;
        }
    }
}
