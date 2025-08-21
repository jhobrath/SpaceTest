using GalagaFighter.Core.Models.PowerUps;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IPowerUpUpdater
    {
        void Update(Game game, PowerUp powerUp);
    }
    public class PowerUpUpdater : IPowerUpUpdater
    {
        private readonly Dictionary<PowerUp, float> _sinceHitDict = new();
        private readonly Dictionary<PowerUp, Vector2> _originalSizeDict = new();
        private readonly Dictionary<PowerUp, float> _originalDistanceDict = new();
        private readonly IObjectService _objectService;

        public PowerUpUpdater(IObjectService objectService)
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
            if (!_originalSizeDict.ContainsKey(powerUp))
            {
                _originalSizeDict[powerUp] = new Vector2(powerUp.Rect.Width, powerUp.Rect.Height);
            }
            var player = _objectService.GetOwner(powerUp);
            if (player != null && !_originalDistanceDict.ContainsKey(powerUp))
            {
                _originalDistanceDict[powerUp] = Math.Abs(powerUp.Center.X - player.Center.X) + Math.Abs(powerUp.Center.Y - player.Center.Y);
            }
        }

        private float GetSinceHit(PowerUp powerUp)
        {
            var frameTime = Raylib.GetFrameTime();
            float sinceHit = _sinceHitDict.TryGetValue(powerUp, out var val) ? val + frameTime : frameTime;
            _sinceHitDict[powerUp] = sinceHit;
            return sinceHit;
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
            var originalSize = _originalSizeDict.TryGetValue(powerUp, out var size) ? size : new Vector2(powerUp.Rect.Width, powerUp.Rect.Height);
            var originalDistance = _originalDistanceDict.TryGetValue(powerUp, out var dist) ? dist : 1f;
            var currentDistance = Math.Abs(powerUp.Center.X - player.Center.X) + Math.Abs(powerUp.Center.Y - player.Center.Y);
            var pct = originalDistance > 0 ? currentDistance / originalDistance : 0f;
            var xScale = originalSize.X * pct;
            var yScale = originalSize.Y * pct;

            return new Vector2(xScale, yScale);
        }
    }
}
