using GalagaFighter.Core.Models.PowerUps;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Controllers
{
    public interface IPowerUpController
    {
        void Update(Game game, PowerUp powerUp);
    }

    public class PowerUpController : IPowerUpController
    {
        private readonly IObjectService _objectService;

        // Per-PowerUp instance state (no more dictionaries!)
        private float _sinceHit = 0f;
        private Vector2? _originalSize = null;
        private float? _originalDistance = null;

        public PowerUpController(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Update(Game game, PowerUp powerUp)
        {
            var frameTime = Raylib.GetFrameTime();
            if (powerUp.Owner == Game.Id)
            {
                UpdateUncollectedPowerUp(powerUp, frameTime);
            }
            else
            {
                StoreOriginalSizeAndDistanceIfNeeded(powerUp);
                UpdateCollectedPowerUp(powerUp, frameTime);
            }

            if (powerUp.IsActive && powerUp.Rect.Y > Game.Height)
                powerUp.IsActive = false;
        }

        private void UpdateUncollectedPowerUp(PowerUp powerUp, float frameTime)
        {
            var newX = powerUp.Speed.X * frameTime;
            var newY = powerUp.Speed.Y * frameTime;
            powerUp.Move(newX, newY);

            powerUp.Rotation += frameTime * 180f % 360f;
        }

        private void UpdateCollectedPowerUp(PowerUp powerUp, float frameTime)
        {
            var player = _objectService.GetOwner(powerUp);
            if (player == null)
                return;

            var movement = GetCollectedMovement(powerUp, player);
            var size = GetCollectedSize(powerUp, player);
            var rotation = frameTime * 360f % 360f;

            powerUp.Move(movement.X, movement.Y);
            powerUp.ScaleTo(size.X, size.Y);
            powerUp.Rotation += rotation;
        }

        private void StoreOriginalSizeAndDistanceIfNeeded(PowerUp powerUp)
        {
            if (_originalSize == null)
            {
                _originalSize = new Vector2(powerUp.Rect.Width, powerUp.Rect.Height);
            }
            var player = _objectService.GetOwner(powerUp);
            if (player != null && _originalDistance == null)
            {
                _originalDistance = Math.Abs(powerUp.Center.X - player.Center.X) + Math.Abs(powerUp.Center.Y - player.Center.Y);
            }
        }

        private float GetSinceHit(PowerUp powerUp)
        {
            var frameTime = Raylib.GetFrameTime();
            _sinceHit += frameTime;
            return _sinceHit;
        }

        private Vector2 GetCollectedMovement(PowerUp powerUp, GameObject player)
        {
            var sinceHit = GetSinceHit(powerUp);
            var collectFactor = sinceHit / 7f;
            var xMovement = (player.Center.X - powerUp.Center.X) * collectFactor;
            var yMovement = (player.Center.Y - powerUp.Center.Y) * collectFactor;

            return new Vector2(xMovement, yMovement);
        }

        private Vector2 GetCollectedSize(PowerUp powerUp, GameObject player)
        {
            var originalSize = _originalSize ?? new Vector2(powerUp.Rect.Width, powerUp.Rect.Height);
            var originalDistance = _originalDistance ?? 1f;
            var currentDistance = Math.Abs(powerUp.Center.X - player.Center.X) + Math.Abs(powerUp.Center.Y - player.Center.Y);
            var pct = originalDistance > 0 ? currentDistance / originalDistance : 0f;
            var xScale = Math.Min(originalSize.X, originalSize.X * pct);
            var yScale = Math.Min(originalSize.Y, originalSize.Y * pct);

            return new Vector2(xScale, yScale);
        }
    }
}
