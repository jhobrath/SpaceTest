using GalagaFighter.Core2.GameObjects.PowerUps;
using GalagaFighter.Core2.Models.Game;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services
{
    public interface IPowerUpCreationService
    {
        void Update(float frameTime);
    }

    public class PowerUpCreationService : IPowerUpCreationService
    {
        private readonly Random _random = new();
        private float _ellapsedTime;

        private readonly List<Func<Vector2, Vector2, PowerUp>> _powerUpTypes = [ 
            (p, s) => new FireRatePowerUp(p, s)
        ];

        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;

        public PowerUpCreationService(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        public void Update(float frameTime)
        {
            _ellapsedTime += frameTime;

            var chanceOfDropPerSecond = (_ellapsedTime)/5;
            var chanceOfDrop = chanceOfDropPerSecond * frameTime;

            if (_random.NextDouble() < chanceOfDrop)
            {
                var existingPowerUps = _objectService.GetAll<PowerUp>();
                if (existingPowerUps.Count() >= 2)
                    return;

                var powerUp = CreatePowerUp();
                _objectService.Add(powerUp);
                _ellapsedTime = 0f;
            }
        }

        private PowerUp CreatePowerUp()
        {
            int powerUpTypeIndex = _random.Next(0, _powerUpTypes.Count);
            var gameState = _gameDataRegistry.Get<GameState>();

            var isAbove = _random.NextDouble() < .5;
            var posX = 400f + (float)_random.NextDouble() * (gameState.ScreenSize.X - 800f);
            var posY = isAbove ? -50f : gameState.ScreenSize.Y;

            var speedX = 0f;
            var speedY = (isAbove ? 1 : -1) * 200f;

            var powerUp = _powerUpTypes[powerUpTypeIndex](new(posX, posY), new(speedX, speedY));
            powerUp.AngularVelocity = 100f;

            return powerUp;
        }
    }
}
