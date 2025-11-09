using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Collisions;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Handlers.Projectiles;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using Raylib_cs;
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
        private readonly Dictionary<Type, IProjectilePlayerCollisionBehavior> _collisionBehaviors;

        public PlayerProjectileCollisionHandler(IGameDataRegistry gameDataRegistry, IObjectService objectService, IEnumerable<IProjectilePlayerCollisionBehavior> collisionBehaviors)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
            _collisionBehaviors = collisionBehaviors.ToDictionary(b => b.GetType());
        }

        public void Handle(Player player, Projectile projectile)
        {
            if (projectile.PlayerCollisionHandler != null &&
                _collisionBehaviors.TryGetValue(projectile.PlayerCollisionHandler, out var behavior))
            {
                behavior.OnPlayerCollision(projectile, player, _gameDataRegistry, _objectService);
            }
        }
    }
}
