using GalagaFighter.Core.Behaviors.Projectiles.Interfaces;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Projectiles
{
    public class WoodReleasedDestroyBehavior : ProjectileDestroyBehavior, IProjectileDestroyBehavior
    {
        private float _lifeTimeDuration = 0f;
        private readonly float _plankedDuration = 10f;

        public override void Apply(Projectile projectile)
        {
            _lifeTimeDuration += Raylib.GetFrameTime();
            if (_lifeTimeDuration >= _plankedDuration)
                projectile.IsActive = false;
        }
    }
}
