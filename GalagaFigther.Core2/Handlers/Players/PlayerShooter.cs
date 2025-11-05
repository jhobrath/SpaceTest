using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Players
{
    public interface IPlayerShooter
    {
        void Shoot(Player player, float frameTime);
    }

    public class PlayerShooter : IPlayerShooter
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;
        private readonly IGameObjectPositionService _positionService;

        public PlayerShooter(IGameDataRegistry gameDataRegistry, IObjectService objectService, IGameObjectPositionService positionService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
            _positionService = positionService;
        }

        public void Shoot(Player player, float frameTime)
        {
            var inputData = _gameDataRegistry.Get<PlayerInputData>(player);
            if (inputData?.Shoot == null || !inputData.Shoot.IsDown)
                return;

            foreach (var gun in player.Guns)
                gun.ShotRequested = true;
        }
    }
}
