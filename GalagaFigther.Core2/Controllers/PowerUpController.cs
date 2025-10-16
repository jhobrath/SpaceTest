using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.PowerUps;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Models.PowerUp;
using GalagaFighter.Core2.Services;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Core2.Controllers
{
    public interface IPowerUpController : IController<PowerUp>
    {

    }
    public class PowerUpController : IPowerUpController
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;

        public PowerUpController(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        public void Update(PowerUp powerUp, float frameTime)
        {
            if(powerUp.Owner == Guid.Empty)
            { 
                Rotate(powerUp, frameTime);
                Move(powerUp, frameTime);
                Deactivate(powerUp);
            }
            else
            {
                var collectionData = _gameDataRegistry.Get<PowerUpCollectionData>(powerUp);
                StoreOriginalSizeAndDistanceIfNeeded(powerUp, collectionData);
                UpdateCollectedPowerUp(powerUp, collectionData, frameTime);
            }
        }

        private void UpdateCollectedPowerUp(PowerUp powerUp, PowerUpCollectionData collectionData, float frameTime)
        {
            var player = _objectService.Get(powerUp.Owner);
            if (player == null)
                return;

            var movement = GetCollectedMovement(powerUp, collectionData, player);
            var size = GetCollectedSize(powerUp, collectionData, player);
            var rotation = frameTime * 360f % 360f;

            powerUp.Move(movement.X, movement.Y);
            powerUp.ScaleTo(size.X, size.Y);
            powerUp.Rotation += rotation;
        }

        private void StoreOriginalSizeAndDistanceIfNeeded(PowerUp powerUp, PowerUpCollectionData collectionData)
        {
            if (collectionData.OriginalSize == null)
            {
                collectionData.OriginalSize = new Vector2(powerUp.Rect.Width, powerUp.Rect.Height);
            }
            var player = _objectService.Get(powerUp.Owner);
            if (player != null && collectionData.OriginalDistance == null)
            {
                collectionData.OriginalDistance = Math.Abs(powerUp.Center.X - player.Center.X) + Math.Abs(powerUp.Center.Y - player.Center.Y);
            }
        }

        private float GetSinceHit(PowerUp powerUp, PowerUpCollectionData collectionData)
        {
            var frameTime = Raylib.GetFrameTime();
            collectionData.SinceHit += frameTime;
            return collectionData.SinceHit;
        }

        private Vector2 GetCollectedMovement(PowerUp powerUp, PowerUpCollectionData collectionData, GameObject player)
        {
            var sinceHit = GetSinceHit(powerUp, collectionData);
            var collectFactor = sinceHit / 7f;
            var xMovement = (player.Center.X - powerUp.Center.X) * collectFactor;
            var yMovement = (player.Center.Y - powerUp.Center.Y) * collectFactor;

            return new Vector2(xMovement, yMovement);
        }

        private Vector2 GetCollectedSize(PowerUp powerUp, PowerUpCollectionData collectionData, GameObject player)
        {
            var originalSize = collectionData.OriginalSize ?? new Vector2(powerUp.Rect.Width, powerUp.Rect.Height);
            var originalDistance = collectionData.OriginalDistance ?? 1f;
            var currentDistance = Math.Abs(powerUp.Center.X - player.Center.X) + Math.Abs(powerUp.Center.Y - player.Center.Y);
            var pct = originalDistance > 0 ? currentDistance / originalDistance : 0f;
            var xScale = Math.Min(originalSize.X, originalSize.X * pct);
            var yScale = Math.Min(originalSize.Y, originalSize.Y * pct);

            return new Vector2(xScale, yScale);
        }

        private void Deactivate(PowerUp powerUp)
        {
            var gameState = _gameDataRegistry.Get<GameState>();
            if (powerUp.Y < -100 || powerUp.Y > gameState.ScreenSize.Y + 10)
                powerUp.IsActive = false;
        }

        private static void Move(PowerUp powerUp, float frameTime)
        {
            powerUp.Move(powerUp.Speed.X * frameTime, powerUp.Speed.Y * frameTime);
        }

        private static void Rotate(PowerUp powerUp, float frameTime)
        {
            powerUp.Rotation += 100f * frameTime;
        }

        public void Draw(PowerUp powerUp, float frameTime)
        {
            powerUp.Sprite.Draw(powerUp.Rect, powerUp.Rotation, powerUp.Color);
        }
    }
}
