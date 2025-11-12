using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Services.Static;
using System.Numerics;
using GalagaFighter.Core2.Models.Players;

namespace GalagaFighter.Core2.Handlers.Players
{
    public interface IPlayerAccelerator
    {
        void Accelerate(Player player, float frameTime);
    }
    public class PlayerMover : IPlayerAccelerator
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IInputService _inputService;

        public PlayerMover(IGameDataRegistry gameDataRegistry, IInputService inputService)
        {
            _gameDataRegistry = gameDataRegistry;
            _inputService = inputService;
        }

        public void Accelerate(Player player, float frameTime)
        {
            var accelX = 0f;
            var accelY = 0f;

            var inputData = _gameDataRegistry.Get<PlayerInputData>(player);
            var baseStats = _gameDataRegistry.Get<PlayerBaseStats>(player);
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);
            var shootData = _gameDataRegistry.Get<PlayerShootData>(player);


            if (inputData.Left && !inputData.Right)
                accelX = -baseStats.Acceleration.X;
            if (inputData.Right && !inputData.Left)
                accelX = baseStats.Acceleration.X;

            if (inputData.Up.IsDown && !inputData.Down.IsDown)
                accelY = -baseStats.Acceleration.Y;

            if (inputData.Down.IsDown && !inputData.Up.IsDown)
                accelY = baseStats.Acceleration.Y;

            if (shootData.RecoveryTime > 0)
            {
                var pct = shootData.RecoveryTime / shootData.RecoveryWindow;
                player.AccelTo(accelX * pct, accelY * pct);
                return;
            }
            else
                shootData.RecoveryWindow = 0f;

                player.AccelTo(accelX, accelY);
        }
    }
}
