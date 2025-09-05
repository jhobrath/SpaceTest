using GalagaFighter.Core.Models.Projectiles;

namespace GalagaFighter.Core.Handlers.Collisions
{
    public static class EdgeCollisionDetector
    {
        public static bool HasCollision(GameObject gameObject, float? distance)
        {
            if (!distance.HasValue)
                return false;

            var edge = gameObject.Speed.X < 0
                ? distance
                : Game.Width - distance;

             return (gameObject.Speed.X > 0 && gameObject.Rect.X + gameObject.Rect.Width > edge) ||
                   (gameObject.Speed.X < 0 && gameObject.Rect.X < edge);
        }
    }
}