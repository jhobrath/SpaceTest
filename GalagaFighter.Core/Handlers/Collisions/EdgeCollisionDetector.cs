using GalagaFighter.Core.Models.Projectiles;

namespace GalagaFighter.Core.Handlers.Collisions
{
    public class EdgeCollisionDetector
    {
        public bool HasCollision(GameObject gameObject)
        {
            // Only projectiles currently have edge collision logic
            if (gameObject is not Projectile projectile)
                return false;

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