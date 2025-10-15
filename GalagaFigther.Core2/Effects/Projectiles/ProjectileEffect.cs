using GalagaFigther.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Effects.Projectiles
{
    public abstract class ProjectileEffect : PlayerEffect
    {
        public abstract Vector2 GunOffset { get; }

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.GunOffset = GunOffset;
        }
    }
}
