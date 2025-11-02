using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.PowerUps;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
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
    public interface IPlayerPowerUpCollisionHandler
    {
        void Handle(Player player, PowerUp powerUp);
    }
    public class PlayerPowerUpCollisionHandler : IPlayerPowerUpCollisionHandler
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        public PlayerPowerUpCollisionHandler(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public void Handle(Player player, PowerUp powerUp)
        {
            if (Vector2.Distance(player.Center, powerUp.Center) > 20)
                return;

            var playerEffects = _gameDataRegistry.Get<PlayerEffects>(player);
            var powerUpEffects = powerUp.CreateEffects(player);
            playerEffects.AddRange(powerUpEffects);
            powerUp.IsActive = false;

            AddCollectRenderEffect(player);
        }

        private void AddCollectRenderEffect(Player player)
        {
            var renderEffects = _gameDataRegistry.Get<PlayerRenderEffects>(player);
            renderEffects.Add(new PlayerRenderEffect(RenderEffectActions.Flash(Color.White.ApplyBlue(.75f)), .75f, 5f));
            renderEffects.Add(new PlayerRenderEffect(RenderEffectActions.Heartbeat, .35f, 1f));
        }
    }
}
