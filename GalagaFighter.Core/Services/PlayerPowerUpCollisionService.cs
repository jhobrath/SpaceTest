using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.PowerUps;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerPowerUpCollisionService
    {
        void HandleCollisions();
    }

    public class PlayerPowerUpCollisionService : IPlayerPowerUpCollisionService
    {
        private readonly IObjectService _objectService;
        private readonly IInputService _inputService;

        public PlayerPowerUpCollisionService(IObjectService objectService, IInputService inputService)
        {
            _objectService = objectService;
            _inputService = inputService;
        }

        public void HandleCollisions()
        {
            var players = _objectService.GetGameObjects<Player>();
            var powerUps = _objectService.GetGameObjects<PowerUp>();

            foreach(var powerUp in powerUps)
            {
                if (powerUp.Owner == Game.Id)
                    continue;

                HandleCollision(players, powerUp);
            }
        }

        private static void HandleCollision(List<Player> players, PowerUp powerUp)
        {
            var player = players.Single(x => x.Id == powerUp.Owner);

            var powerUpIsClose = Math.Abs(player.Center.X - powerUp.Center.X) < 20;
            if (!powerUpIsClose)
                return;

            var newEffects = powerUp.CreateEffects();
            foreach (var newEffect in newEffects)
            {
                var existingEffect = player.Effects.FirstOrDefault(e => e.GetType() == newEffect.GetType());
                if (existingEffect != null)
                    ReplaceExistingEffect(player, newEffect, existingEffect);
                else
                    player.Effects.Add(newEffect);
            }

            powerUp.IsActive = false;
        }

        private static void ReplaceExistingEffect(Player player, Models.Effects.PlayerEffect newEffect, Models.Effects.PlayerEffect existingEffect)
        {
            var index = player.Effects.IndexOf(existingEffect);
            player.Effects[index] = newEffect;

            if (player.SelectedProjectile == existingEffect)
                player.SelectedProjectile = newEffect;
        }
    }
}
