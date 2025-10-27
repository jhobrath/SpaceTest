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

            projectile.IsActive = false;

            var currentEffects = _gameDataRegistry.Get<PlayerEffects>(player);
            var effects = projectile.CreateEffects(player);
            currentEffects.AddRange(effects);

            var collisionPoint = new Vector2(projectile.Speed.X < 0 ? projectile.WorldPosition.X : projectile.WorldPosition.X + projectile.Width,
                projectile.WorldPosition.Y + projectile.Height / 2f);

            var collision = new DefaultCollision(collisionPoint, 55f);
            _objectService.Add(collision);
        }
    }
}
