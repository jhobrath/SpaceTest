using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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

        public PlayerProjectileCollisionService(IObjectService objectService, IPlayerProjectileCollisionPlanker planker,
            ICollisionCreationService collisionCreationService)
        {
            _objectService = objectService;
            _planker = planker;
            _collisionCreationService = collisionCreationService;
        }

        public void HandleCollisions(Player player, EffectModifiers modifiers)
        {
            var projectiles = _objectService.GetGameObjects<Projectile>();

            foreach(var projectile in projectiles)
            {
                if (projectile.Owner == player.Id)
                    continue;

                if (!Raylib.CheckCollisionRecs(player.Rect, projectile.Rect))
                    continue;

                Collide(player, projectile, modifiers);
            }
        }

        private void Collide(Player player, Projectile projectile, EffectModifiers modifiers)
        {
            var effects = projectile.CreateEffects();
            foreach (var effect in effects)
                player.Effects.AddRange(effects);

            player.Health -= projectile.BaseDamage * projectile.Modifiers.DamageMultiplier * 1/modifiers.Stats.Shield;

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
