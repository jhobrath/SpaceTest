using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects.Projectiles
{
    public abstract class ProjectileEffect : PlayerEffect
    {
        public override bool IsProjectile => true;
        public abstract Vector2 GetProjectileSpeed();
    }
}
