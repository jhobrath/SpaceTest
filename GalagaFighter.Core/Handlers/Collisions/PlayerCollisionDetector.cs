using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;

namespace GalagaFighter.Core.Handlers.Collisions
{
    public class PlayerCollisionDetector
    {
        public bool HasCollision(Player player, Projectile projectile)
        {
            return Raylib.CheckCollisionRecs(player.Rect, projectile.Rect);
        }
    }
}