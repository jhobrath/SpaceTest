using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public PlayerProjectileCollisionHandler(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public void Handle(Player player, Projectile projectile)
        {
            if (player.Id == projectile.Owner)
                return;

            projectile.IsActive = false;

            var currentEffects = _gameDataRegistry.Get<PlayerEffects>(player);
            var effects = projectile.CreateEffects(player);
            currentEffects.AddRange(effects);
        }
    }
}
