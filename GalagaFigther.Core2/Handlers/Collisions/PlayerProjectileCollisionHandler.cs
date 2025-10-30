using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Collisions;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Collisions
{
    public interface IPlayerProjectileCollisionHandler
    {
        void Handle(Player player, Projectile projectile);
    }

    public class PlayerProjectileCollisionHandler : IPlayerProjectileCollisionHandler
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;

        public PlayerProjectileCollisionHandler(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        public void Handle(Player player, Projectile projectile)
        {
            if (player.Id == projectile.Owner)
                return;

            UpdateHealth(player, projectile);
            UpdateEffects(player, projectile);
            CreateCollision(projectile);
            DeactivateProjectile(projectile);
        }

        private void DeactivateProjectile(Projectile projectile)
        {
            projectile.IsActive = false;
        }

        private void CreateCollision(Projectile projectile)
        {
            var collisionPoint = new Vector2(projectile.Speed.X < 0 ? projectile.WorldPosition.X : projectile.WorldPosition.X + projectile.Width,
                            projectile.WorldPosition.Y + projectile.Height / 2f);

            var collision = new DefaultCollision(collisionPoint, 55f);
            _objectService.Add(collision);
        }

        private void UpdateEffects(Player player, Projectile projectile)
        {
            var currentEffects = _gameDataRegistry.Get<PlayerEffects>(player);
            var effects = projectile.CreateEffects(player);
            currentEffects.AddRange(effects);
        }

        private void UpdateHealth(Player player, Projectile projectile)
        {
            var opponent = _objectService.GetOpponent(player);
            var opponentStats = _gameDataRegistry.Get<PlayerBaseStats>(opponent);
            var opponentModifiers = _gameDataRegistry.Get<PlayerModifiers>(opponent);
            var shooterDamageMultiplier = opponentStats.Damage * opponentModifiers.Stats.DamageMultiplier;

            var baseStats = _gameDataRegistry.Get<PlayerBaseStats>(player);
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);
            var shieldMultiplier = baseStats.Shield * modifiers.Stats.ShieldMultiplier;

            var damage = projectile.Damage * shooterDamageMultiplier * (1 / shieldMultiplier);
            player.Health -= damage;
        }
    }
}
