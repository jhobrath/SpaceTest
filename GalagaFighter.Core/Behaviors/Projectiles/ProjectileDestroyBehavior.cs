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
    public class ProjectileDestroyBehavior : IProjectileDestroyBehavior
    {
        public void Apply(Projectile projectile)
        {
            if (projectile.Rect.X < 0 || projectile.Rect.X > Game.Width)
                projectile.IsActive = false;
        }
    }
}
