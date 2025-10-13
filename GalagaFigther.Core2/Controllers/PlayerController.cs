using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Models.Player;
using GalagaFigther.Core2.Services;
using GalagaFigther.Core2.Services.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Controllers
{
    public class PlayerController : ControllerBase<Player>
    {
        private const float _rotationSpeed = 250f;
        private const float _maxPlayerSpeedY = 1000f;
        private const float _maxPlayerSpeedX = 500f;
        private const float _maxSpeedReached = .5f;
        private float _timer = 0f;
        private bool _resetTimer = false;
        //private float _airResistance = 10f;
        private readonly IInputService _inputService;

        public PlayerController(IGameDataRegistry gameDataRegistry, IInputService inputService)
            : base(gameDataRegistry)
        {
            _inputService = inputService;
        }

        public override void Update(Player player, float frameTime)
        {
            var moveData = _gameDataRegistry.Get<PlayerMoveData>(player);
            HandleInputsStandard(player, moveData, frameTime);

            player.Move(player.Speed.X * frameTime, player.Speed.Y * frameTime);
            player.Rotation = 90 + 10 * player.Speed.Y / _maxPlayerSpeedY;
        }

        private Vector2 CalculateSpeedVector(float directionalSpeed, float rotation)
        {
            // Convert rotation from degrees to radians
            float radians = rotation * (float)(Math.PI / 180.0);
            
            // Adjust for game coordinate system where 0° typically points up
            float velocityX = directionalSpeed * (float)Math.Sin(radians);
            float velocityY = -directionalSpeed * (float)Math.Cos(radians); // Negative because Y increases downward
            
            return new Vector2(velocityX, velocityY);
        }

        private void HandleInputsStandard(Player player, PlayerMoveData moveData, float frameTime)
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

            var speedDeltaX = player.Acceleration.X * frameTime;
            var speedDeltaY = player.Acceleration.Y * frameTime;

            var _airResistance = -player.Speed * 5f;
            var airResistanceX = _airResistance.X * frameTime;
            var airResistanceY = _airResistance.Y * frameTime;

            var newSpeedX = player.Speed.X + speedDeltaX + airResistanceX;
            var newSpeedY = player.Speed.Y + speedDeltaY + airResistanceY;

            newSpeedX = Math.Clamp(newSpeedX, -_maxPlayerSpeedX, _maxPlayerSpeedX);
            newSpeedY = Math.Clamp(newSpeedY, -_maxPlayerSpeedY, _maxPlayerSpeedY);

            // Set the speed to the final clamped value
            player.HurryTo(newSpeedX, newSpeedY);
        }

        private void HandleInputsTankControls(Player player, PlayerMoveData moveData, float frameTime)
        {
            if (_inputService.Left.IsDown)
                player.Rotation -= _rotationSpeed * frameTime;

            if (_inputService.Right.IsDown)
                player.Rotation += _rotationSpeed * frameTime;

            if (_inputService.Forward.IsDown && !_inputService.Back.IsDown)
            {
                var newSpeed = moveData.DirectionalSpeed + (_maxPlayerSpeedX * frameTime * (1f / _maxSpeedReached));
                moveData.DirectionalSpeed = Math.Min(_maxPlayerSpeedX, newSpeed);
            }

            if (_inputService.Back.IsDown && !_inputService.Forward.IsDown)
            {
                var newSpeed = moveData.DirectionalSpeed - (_maxPlayerSpeedX * frameTime * (1f / _maxSpeedReached));
                moveData.DirectionalSpeed = Math.Max((-_maxPlayerSpeedX)/1.25f, newSpeed);
            }

            if(!_inputService.Forward.IsDown && !_inputService.Back.IsDown)
            {
                var speedWasAboveZero = moveData.DirectionalSpeed >= 0;
                var newSpeed = moveData.DirectionalSpeed - (moveData.DirectionalSpeed * frameTime* (1f/(.25f*_maxSpeedReached)));

                moveData.DirectionalSpeed = newSpeed;
            }

            var speed = CalculateSpeedVector(moveData.DirectionalSpeed, player.Rotation % 360f);
            player.HurryTo(speed.X, speed.Y);
        }

        public override void Draw(Player player, float frameTime)
        {
            player.Sprite.Draw(player);
        }
    }
}
