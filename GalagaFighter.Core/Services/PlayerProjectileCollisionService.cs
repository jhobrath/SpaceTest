using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerProjectileCollisionService
    {
        void HandleCollisions(Player player, EffectModifiers modifiers);
    }

    public class PlayerProjectileCollisionService : IPlayerProjectileCollisionService
    {
        private readonly IObjectService _objectService;
        private readonly IPlayerProjectileCollisionPlanker _planker;
        private readonly ICollisionCreationService _collisionCreationService;
        private readonly EdgeCollisionDetector _edgeDetector;
        private readonly PlayerCollisionDetector _playerDetector;

        public PlayerProjectileCollisionService(IObjectService objectService, IPlayerProjectileCollisionPlanker planker,
            ICollisionCreationService collisionCreationService)
        {
            _objectService = objectService;
            _planker = planker;
            _collisionCreationService = collisionCreationService;
            _edgeDetector = new EdgeCollisionDetector();
            _playerDetector = new PlayerCollisionDetector();
        }

        public void HandleCollisions(Player player, EffectModifiers modifiers)
        {
            var projectiles = _objectService.GetGameObjects<Projectile>();

            foreach(var projectile in projectiles)
            {
                if (projectile.Owner == player.Id)
                    continue;

                // Check both collision types - projectile could hit both edge AND player
                var hasEdgeCollision = _edgeDetector.HasCollision(projectile);
                var hasPlayerCollision = _playerDetector.HasCollision(player, projectile);

                if (hasEdgeCollision || hasPlayerCollision)
                {
                    Collide(player, projectile, modifiers);
                }
            }
        }

        private void Collide(Player player, Projectile projectile, EffectModifiers modifiers)
        {
            var effects = projectile.CreateEffects();
            foreach (var effect in effects)
            {
                if(effect.MaxCount != 0)
                {
                    var soFar = player.Effects.Count(x => x.GetType() == effect.GetType());
                    if (soFar >= effect.MaxCount)
                        continue;
                }

                player.Effects.Add(effect);
            }

            player.Health -= projectile.BaseDamage * projectile.Modifiers.DamageMultiplier * (1 / modifiers.Stats.Shield);

            var collisionObjects = projectile.Modifiers.OnCollide?.Invoke(player, projectile);
            if (collisionObjects != null && collisionObjects.Count > 0)
            {
                _objectService.AddRange(collisionObjects);
            }

            var planked = _planker.IsPlanked(player, projectile);
            if (planked)
            {
                _planker.HandlePlankCollision(player, projectile);
                return;
            }

            _collisionCreationService.Create(player, projectile);

            if(projectile.Modifiers.DeactivateOnCollision)
            { 
                projectile.IsActive = false;
                projectile.Modifiers.OnProjectileDestroyed?.Invoke(projectile);
            }
        }
    }
}
