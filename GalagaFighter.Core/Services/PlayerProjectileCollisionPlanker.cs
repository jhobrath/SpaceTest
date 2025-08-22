using GalagaFighter.Core.Behaviors.Projectiles;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerProjectileCollisionPlanker
    {
        void HandlePlankCollision(Player player, Projectile projectile);
        bool IsPlanked(Player player, Projectile projectile);
    }
    public class PlayerProjectileCollisionPlanker : IPlayerProjectileCollisionPlanker
    {
        private Dictionary<Player, Projectile> _stuckProjectile = [];
        public bool IsPlanked(Player player, Projectile projectile)
        {
            var isPlanked = projectile.Modifiers.PlankDuration > 0f;
            if (!isPlanked)
                return false;

            var alreadyStuck = _stuckProjectile.ContainsKey(player) && _stuckProjectile[player] == projectile;
            alreadyStuck = alreadyStuck || !(projectile.Rect.X == 0 || projectile.Rect.X + projectile.Rect.Width == Game.Width);

            if(alreadyStuck)
                _stuckProjectile[player] = projectile;

            return isPlanked;
        }

        public void HandlePlankCollision(Player player, Projectile projectile)
        {
            if (player.Id == projectile.Owner)
                return;

            if(_stuckProjectile.ContainsKey(player) && _stuckProjectile[player] == projectile)
            {
                player.MoveTo(y: projectile.Rect.Y - player.Rect.Height/2);
                return;
            }

            if(projectile.Modifiers.PlankStopsMovement)
            {     
                if(player.Center.Y < projectile.Rect.Y)
                    player.MoveTo(y: projectile.Rect.Y - player.Rect.Height);
                else if(player.Rect.Y + player.Rect.Height > projectile.Center.Y)
                        player.MoveTo(y: projectile.Rect.Y + projectile.Rect.Height);
            }
        }
    }
}
