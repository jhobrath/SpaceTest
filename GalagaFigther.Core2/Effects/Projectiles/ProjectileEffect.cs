using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Effects.Projectiles
{
    public abstract class ProjectileEffect : PlayerEffect
    {
        //GunOffset should be set for a ship facing right
        //(90 degrees Raylib rotation, 0 radians mathematical rotation)
        public abstract Vector2 GunOffset { get; }

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.GunOffset = GunOffset;
        }
    }
}
