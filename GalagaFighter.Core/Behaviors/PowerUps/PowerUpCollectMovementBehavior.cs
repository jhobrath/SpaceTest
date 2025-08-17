using GalagaFighter.Core.Behaviors.PowerUps.Interfaces;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.PowerUps;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.PowerUps
{
    public class PowerUpCollectMovementBehavior : IPowerUpMovementBehavior
    {
        private IObjectService _objectService;
        private GameObject _player;

        public PowerUpCollectMovementBehavior(IObjectService objectService, PowerUp powerUp, GameObject player)
        {
            _objectService = objectService;

            _player = player;
            _originalWidth = powerUp.Rect.Width;
            _originalHeight = powerUp.Rect.Height;
            _originalDistanceX = Math.Abs(powerUp.Center.X - player.Center.X);
        }

        private float _sinceHit = 0f;
        private const float _rotationChange = 360f;
        private float _originalWidth = 0f;
        private float _originalHeight = 0f;
        private float _originalDistanceX = 0f;

        public void Apply(PowerUp powerUp)
        {
            var frameTime = Raylib.GetFrameTime();
            _sinceHit += frameTime;

            var collectFactor = (_sinceHit / 7);

            var xMovement = (_player.Center.X - powerUp.Center.X)* collectFactor;
            var yMovement = (_player.Center.Y - powerUp.Center.Y)* collectFactor;
            powerUp.Move(xMovement, yMovement);

            var newDistanceX = Math.Abs(powerUp.Center.X - _player.Center.X);
            var pct = newDistanceX / _originalDistanceX;

            var xScale = Math.Min(_originalWidth, _originalWidth * pct*1.5f);
            var yScale = Math.Min(_originalHeight, _originalHeight * pct*1.5f);
            powerUp.ScaleTo(xScale, yScale);

            powerUp.Rotation += _rotationChange*frameTime;
        }
    }
}
