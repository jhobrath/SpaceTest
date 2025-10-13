using GalagaFigther.Core2.Controllers;
using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Services;
using GalagaFigther.Core2.Models.Game;

namespace GalagaFigther.Core2.Handlers.Players
{
    public interface IPlayerMover
    {
        void Move(Player player, float frameTime);
    }
    public class PlayerMover : IPlayerMover
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IInputService _inputService;

        public PlayerMover(IGameDataRegistry gameDataRegistry, IInputService inputService)
        {
            _gameDataRegistry = gameDataRegistry;
            _inputService = inputService;
        }

        public void Move(Player player, float frameTime)
        {
            var ranges = _gameDataRegistry.Get<PlayerRanges>();

            HandleAcceleration(player);
            HandleSpeed(player, ranges, frameTime);
            HandlePosition(player, frameTime);
        }

        private static void HandleSpeed(Player player, PlayerRanges ranges, float frameTime)
        {
            var speedDeltaX = player.Acceleration.X * frameTime;
            var speedDeltaY = player.Acceleration.Y * frameTime;

            var _airResistance = -player.Speed * 5f;
            var airResistanceX = _airResistance.X * frameTime;
            var airResistanceY = _airResistance.Y * frameTime;

            var newSpeedX = player.Speed.X + speedDeltaX + airResistanceX;
            var newSpeedY = player.Speed.Y + speedDeltaY + airResistanceY;

            newSpeedX = Math.Clamp(newSpeedX, -ranges.MaxSpeedX, ranges.MaxSpeedX);
            newSpeedY = Math.Clamp(newSpeedY, -ranges.MaxSpeedY, ranges.MaxSpeedY);

            player.HurryTo(newSpeedX, newSpeedY);
        }

        private void HandleAcceleration(Player player)
        {
            var accelX = 0f;
            var accelY = 0f;
            if (_inputService.Left.IsDown && !_inputService.Right.IsDown)
                accelX = -10000f;

            if (_inputService.Right.IsDown && !_inputService.Left.IsDown)
                accelX = 10000f;

            if (_inputService.Forward.IsDown && !_inputService.Back.IsDown)
                accelY = -10000f;

            if (_inputService.Back.IsDown && !_inputService.Forward.IsDown)
                accelY = 10000f;

            player.AccelTo(accelX, accelY);
        }

        private static void HandlePosition(Player player, float frameTime)
        {
            player.Move(player.Speed.X * frameTime, player.Speed.Y * frameTime);
        }
    }
}
