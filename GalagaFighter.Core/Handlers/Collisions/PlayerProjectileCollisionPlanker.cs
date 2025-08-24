using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Handlers.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GalagaFighter.Core.Handlers.Collisions
{
    public interface IPlayerProjectileCollisionPlanker
    {
        void HandlePlankCollision(Player player, Projectile projectile);
        bool IsPlanked(Player player, Projectile projectile);
    }
    public class PlayerProjectileCollisionPlanker : IPlayerProjectileCollisionPlanker
    {
        private readonly ICollisionCreationService _collisionCreationService;
        private Projectile? _stuckProjectile = null;

        public PlayerProjectileCollisionPlanker(ICollisionCreationService collisionCreationService)
        {
            _collisionCreationService = collisionCreationService;
        }

        public bool IsPlanked(Player player, Projectile projectile)
        {
            var isPlanked = projectile.Modifiers.PlankDuration > 0f;
            if (!isPlanked)
                return false;

            var alreadyStuck = _stuckProjectile == projectile;
            if(!alreadyStuck)
            {
                if (!(projectile.CurrentFrameRect.X == 0 || projectile.CurrentFrameRect.X + projectile.CurrentFrameRect.Width == Game.Width))
                {
                    alreadyStuck = true;
                    _collisionCreationService.Create(player, projectile);
                }
            }

            if(alreadyStuck)
                _stuckProjectile = projectile;

            return isPlanked;
        }

        public void HandlePlankCollision(Player player, Projectile projectile)
        {
            if (player.Id == projectile.Owner)
                return;

            if(_stuckProjectile == projectile)
            {
                player.MoveTo(y: projectile.Center.Y - player.Rect.Height/2);
                return;
            }

            if(projectile.Modifiers.PlankStopsMovement)
            {     
                var playerIsStuck = _stuckProjectile == projectile;
                if (player.Center.Y < projectile.CurrentFrameRect.Y && !playerIsStuck)
                    player.MoveTo(y: projectile.CurrentFrameRect.Y - player.Rect.Height);
                else if(player.Rect.Y + player.Rect.Height > projectile.Center.Y && !playerIsStuck)
                    player.MoveTo(y: projectile.CurrentFrameRect.Y + projectile.CurrentFrameRect.Height);
            }
        }
    }
}
