using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.PowerUps;
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
    public interface IPlayerPowerUpCollisionHandler
    {
        void Handle(Player player, PowerUp powerUp, Vector2 point);
    }
    public class PlayerPowerUpCollisionHandler : IPlayerPowerUpCollisionHandler
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        public PlayerPowerUpCollisionHandler(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public void Handle(Player player, PowerUp powerUp, Vector2 point)
        {
            if (Vector2.Distance(player.Center, powerUp.Center) > 20)
                return;

            var playerEffects = _gameDataRegistry.Get<PlayerEffects>(player);
            var powerUpEffects = powerUp.CreateEffects(player);
            playerEffects.AddRange(powerUpEffects);
            powerUp.IsActive = false;
        }
    }
}
