using System;
using UnityEngine;
using RWCustom;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Menu;
using MonoMod.Cil;

using SlugCrafting.Items;
using ImprovedInput;

namespace SlugCrafting;

public static partial class Hooks
{
    private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
    {
        // CHECK WHAT HOLDING
        Knife? graspedKnife = null;
        int graspedKnifeGrasp = -1;
        PhysicalObject? graspedPhysicalObject = null;
        int graspedPhysicalObjectGrasp = -1;

        // Check if the player is holding a graspedKnife and a creature simultaneously
        for (int i = 0; i < self.grasps.Length; i++)
        {
            var grasp = self.grasps[i];
            if (grasp == null)
                continue;

            // CREATURE CHECKING
            // Grabbed chunk takes priority first, because can be shared with item and creature.
            if (grasp.grabbedChunk.owner is Creature)
            {
                graspedPhysicalObjectGrasp = i;
                graspedPhysicalObject = grasp.grabbed as PhysicalObject;
            }
            // ITEM CHECKING
            // Grabbed knife overrides grabbed chunk if detected then.
            if (grasp.grabbed is Knife)
            {
                graspedKnifeGrasp = i;
                graspedKnife = grasp.grabbed as Knife;
            }
            else if (grasp.grabbed is PhysicalObject)
            {
                graspedPhysicalObjectGrasp = i;
                graspedPhysicalObject = grasp.grabbed as PhysicalObject;
            }
        }

        // IF CAN SCAVENGE
        if (graspedKnife != null && graspedPhysicalObject != null)
        {
            // TODO: I'm aware this is a mess, it is temporary.
            // IF WANT TO SCAVENGE

            int playerNumber = self.playerState.playerNumber;
            bool playerPressingScavenge = Plugin.InputScavenge.CheckRawPressed(playerNumber);

            //if (playerPressingScavenge)
            //{
                graspedKnife.BladeColor = Color.green;
                AbstractPhysicalObject scavengedAbstractObject = null;

                var tilePosition = self.room.GetTilePosition(graspedPhysicalObject.bodyChunks[0].pos);
                var pos = new WorldCoordinate(self.room.abstractRoom.index, tilePosition.x, tilePosition.y, 0);

                // Check for specific types of physical objects to return scavenge.
                
                /*if (graspedPhysicalObject is Lizard)
                {
                    graspedKnife.BladeColor = Color.red;
                    var lizard = graspedPhysicalObject as Lizard;
                    var lizardGraphics = (lizard.graphicsModule as LizardGraphics);

                    lizardGraphicsStorage.TryGetValue(lizardGraphics, out LizardGraphicsData? lizardGraphicsData);
                    scavengedAbstractObject = new AbstractLizardShell(self.room.world, pos, self.room.game.GetNewID())
                    {
                        shellColor = lizardGraphics.effectColor,
                        headSprite = lizardGraphicsData.spriteLeaser.sprites[lizardGraphics.SpriteHeadStart + 2].element.name
                    };
                }
                else*/ if (graspedPhysicalObject is Lizard)
                {
                    graspedKnife.BladeColor = Color.red;
                    scavengedAbstractObject = new AbstractLizardLeather(self.room.world, pos, self.room.game.GetNewID());
                }

                // GRAB OBJECT IF WE SCAVENGED ANYTHING
                if (scavengedAbstractObject != null)
                {
                    // Where I am desperately going wrong...
                    self.room.abstractRoom.AddEntity(scavengedAbstractObject);
                    scavengedAbstractObject.RealizeInRoom();

                    self.ReleaseGrasp(graspedPhysicalObjectGrasp);
                    self.SlugcatGrab(scavengedAbstractObject.realizedObject, self.FreeHand());
                }
            //}
        }

        orig(self, eu);
    }
    private static void Player_GrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu)
    {
        // Call the original method
        orig(self, eu);
    }
}