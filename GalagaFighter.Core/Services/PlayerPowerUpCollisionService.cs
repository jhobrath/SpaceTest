using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.PowerUps;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly IPlayerManagerFactory _effectManagerFactory;

        public PlayerPowerUpCollisionService(IObjectService objectService, IInputService inputService, IPlayerManagerFactory effectManagerFactory)
        {
            _objectService = objectService;
            _inputService = inputService;
            _effectManagerFactory = effectManagerFactory;
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

        private void HandleCollision(List<Player> players, PowerUp powerUp)
        {
            var player = players.Single(x => x.Id == powerUp.Owner);

            var powerUpIsClose = Math.Abs(player.Center.X - powerUp.Center.X) < 20;
            if (!powerUpIsClose)
                return;

            var effectManager = _effectManagerFactory.GetEffectManager(player);
            var newEffects = powerUp.CreateEffects();
            
            foreach (var newEffect in newEffects)
            {
                // Let the effect manager handle the logic for replacing/stacking effects
                effectManager.AddEffect(newEffect);
            }

            powerUp.IsActive = false;
        }
    }
}
