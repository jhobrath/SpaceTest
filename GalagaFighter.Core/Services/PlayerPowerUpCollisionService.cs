using GalagaFighter.Core.Events;
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
        private readonly IEventService _eventService;

        public PlayerPowerUpCollisionService(IEventService eventService, IObjectService objectService, IInputService inputService)
        {
            _objectService = objectService;
            _inputService = inputService;
            _eventService = eventService;
        }

        public void HandleCollisions()
        {
            var players = _objectService.GetGameObjects<Player>();
            var powerUps = _objectService.GetGameObjects<PowerUp>();

            foreach(var powerUp in powerUps)
            {
                foreach(var player in players)
                {
                    if (powerUp.Owner != player.Id)
                        continue;

                    if(Math.Abs(player.Center.X - powerUp.Center.X) < 50)
                    {
                        _eventService.Publish(new PowerUpCollectedEventArgs(player, powerUp));
                        powerUp.IsActive = false;
                    }
                }
            }
        }
    }
}
