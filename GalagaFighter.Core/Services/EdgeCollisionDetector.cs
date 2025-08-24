using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;

namespace GalagaFighter.Core.Services
{
    public class EdgeCollisionDetector
    {
        public bool HasCollision(Projectile projectile)
        {
            if (projectile.Modifiers.CollideDistanceFromEdge <= 0f)
                return false;

            var edge = projectile.Speed.X < 0
                ? projectile.Modifiers.CollideDistanceFromEdge - projectile.Rect.Width / 2
                : Game.Width - projectile.Modifiers.CollideDistanceFromEdge - projectile.Rect.Width / 2;

            return (projectile.Speed.X > 0 && projectile.Center.X > edge) ||
                   (projectile.Speed.X < 0 && projectile.Center.X < edge);
        }
    }
}