using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Projectiles.Updates
{
    public class ProjectileMovementUpdate
    {
        public Vector2 From { get; set; } = Vector2.Zero;
        public Vector2 To { get; set; } = Vector2.Zero;

        public ProjectileMovementUpdate() { }

        public ProjectileMovementUpdate(Projectile projectile)
        {
            From = projectile.Rect.Position;
            To = projectile.Rect.Position;
        }
    }
}
