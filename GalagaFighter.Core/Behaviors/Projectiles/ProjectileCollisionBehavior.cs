using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Behaviors.Players.Updates;
using GalagaFighter.Core.Behaviors.Projectiles.Interfaces;
using GalagaFighter.Core.Models;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Projectiles
{
    public class ProjectileCollisionBehavior : IProjectileCollisionBehavior
    {
        public List<GameObject> Apply(Projectile projectile)
        {
            var initialPosition = GetInitialPosition(projectile);
            var initialSize = GetInitialSize(projectile);
            var initialVelocity = GetInitialVelocity(projectile);

            initialPosition.Y -= initialSize.Y / 2;

            var collision = new DefaultCollision(initialPosition, initialSize, initialVelocity);

            return new List<GameObject> { collision };
        }

        protected virtual Vector2 GetInitialPosition(Projectile projectile)
        {
            var positionX = projectile.Speed.X < 0 ? projectile.Rect.X : projectile.Rect.X + projectile.Rect.Width;
            var positionY = projectile.Speed.Y < 0 ? projectile.Rect.Y + projectile.Rect.Height : projectile.Rect.Y;

            if (projectile.Speed.Y == 0)
                positionY = projectile.Center.Y;

            var position = new Vector2(positionX, positionY);
            return position;
        }

        protected virtual Vector2 GetInitialSize(Projectile projectile)
        {
            var average = (projectile.Rect.Width + projectile.Rect.Height) / 2;
            average = Math.Max(50, average);
            return new Vector2(average, average);
        }

        protected virtual Vector2 GetInitialVelocity(Projectile projectile)
        {
            return projectile.Speed;
        }
    }
}
