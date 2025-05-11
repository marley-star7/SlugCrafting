using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlugCrafting.Animations.Scavenging;

public abstract class HandAnimation
{
    public abstract void PlaySlugcatAnimation(Player scug, int objectGraspIndexA, int objectGraspIndexB, int animationTime);
}

