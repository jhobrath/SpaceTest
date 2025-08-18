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
                if (powerUp.EffectsApplied)
                    continue;

                foreach(var player in players)
                {
                    if (powerUp.Owner != player.Id)
                        continue;

                    if(Math.Abs(player.Center.X - powerUp.Center.X) < 50)
                    { 
                        foreach(var effect in powerUp.CreateEffects(_objectService, _inputService))
                        {
                            player.AddEffect(effect);
                        }

                        powerUp.EffectsApplied = true;
                    }
                }
            }
        }
    }
}
