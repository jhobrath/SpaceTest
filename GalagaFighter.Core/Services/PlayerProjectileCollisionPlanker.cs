using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        private readonly ICollisionCreationService _collisionCreationService;

        public PlayerProjectileCollisionPlanker(ICollisionCreationService collisionCreationService)
        {
            _collisionCreationService = collisionCreationService;
        }

        private Dictionary<Player, Projectile> _stuckProjectile = [];

        public bool IsPlanked(Player player, Projectile projectile)
        {
            var isPlanked = projectile.Modifiers.PlankDuration > 0f;
            if (!isPlanked)
                return false;

            var alreadyStuck = _stuckProjectile.ContainsKey(player) && _stuckProjectile[player] == projectile;
            if(!alreadyStuck)
            {
                if (!(projectile.Rect.X == 0 || projectile.Rect.X + projectile.Rect.Width == Game.Width))
                {
                    alreadyStuck = true;
                    _collisionCreationService.Create(player, projectile, speedOverride: new Vector2(0f,0f));
                }
            }

            if(alreadyStuck)
                _stuckProjectile[player] = projectile;

            return isPlanked;
        }

        public void HandlePlankCollision(Player player, Projectile projectile)
        {
            if (player.Id == projectile.Owner)
                return;

            if(_stuckProjectile.TryGetValue(player, out Projectile? value) && value == projectile)
            {
                player.MoveTo(y: projectile.Center.Y - player.Rect.Height/2);
                return;
            }

            if(projectile.Modifiers.PlankStopsMovement)
            {     
                var playerIsStuck = _stuckProjectile.ContainsKey(player) && _stuckProjectile[player] == projectile;
                if (player.Center.Y < projectile.Rect.Y && !playerIsStuck)
                    player.MoveTo(y: projectile.Rect.Y - player.Rect.Height);
                else if(player.Rect.Y + player.Rect.Height > projectile.Center.Y && !playerIsStuck)
                    player.MoveTo(y: projectile.Rect.Y + projectile.Rect.Height);
            }
        }
    }
}
