using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Projectiles
{
    public class WoodReleasedMovementBehavior : ProjectileMovementBehavior
    {
        private bool _hasClearedVerticalSpeed = false;

        public WoodReleasedMovementBehavior()
        {
        }

        public override void Apply(Projectile projectile)
        {
            if (!_hasClearedVerticalSpeed)
            {
                projectile.HurryTo(y: 0f);
                _hasClearedVerticalSpeed = true;
            }

            projectile.SetDrawPriority(1);
            base.Apply(projectile);

            if (projectile.Rect.X < 0)
            {
                projectile.MoveTo(x: 0f);
                projectile.HurryTo(0f, 0f);
                projectile.SetMovementBehavior(new ProjectileMovementBehavior());

                ((WoodProjectile)projectile).Planked = true;
            }

            if(projectile.Rect.X + projectile.Rect.Width >= Game.Width)
            {
                projectile.MoveTo(Game.Width - projectile.Rect.Width);
                projectile.HurryTo(0f, 0f);
                projectile.SetMovementBehavior(new ProjectileMovementBehavior());

                ((WoodProjectile)projectile).Planked = true;
            }
        }
    }
}
