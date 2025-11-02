using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.PowerUps;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Models.PowerUps;
using GalagaFighter.Core2.Services;
using Raylib_cs;
using System.Numerics;
using GalagaFighter.Core2.Helpers;

namespace GalagaFighter.Core2.Controllers
{
    public interface IPowerUpController : IController<PowerUp>
    {

    }
    public class PowerUpController : IPowerUpController
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;
        private readonly IPersistentValueHandler _persistentValueHandler;

        public PowerUpController(IGameDataRegistry gameDataRegistry, IObjectService objectService, 
            IPersistentValueHandler persistentValueHandler)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
            _persistentValueHandler = persistentValueHandler;
        }

        public void Update(PowerUp powerUp, float frameTime)
        {
            if(powerUp.Owner == Game.Id)
            { 
                Deactivate(powerUp);
            }
            else
            {
                Collect(powerUp, frameTime);
            }
        }

        private void Collect(PowerUp powerUp, float frameTime)
        {
            var collectionData = _gameDataRegistry.Get<PowerUpCollectionData>(powerUp);
            if (!collectionData.IsCollecting)
            {
                collectionData.IsCollecting = true;
                collectionData.SinceHit = 0f;
                collectionData.OriginalSize = powerUp.Rect.Size;
                powerUp.AngularVelocity = 1000f * (powerUp.Owner == Game.Player1Id ? -1 : 1);
            }

            collectionData.SinceHit += frameTime;

            var player = _objectService.Get(powerUp.Owner);
            var newDist = (player.Center - powerUp.Center) * 1f / MathF.Pow(1 - collectionData.SinceHit, 1);
            var newScale = (1 - collectionData.SinceHit) * collectionData.OriginalSize * 2f;
            newScale = MathExtensions.Min(collectionData.OriginalSize, newScale);

            powerUp.ScaleTo(newScale.X, newScale.Y);
            powerUp.HurryTo(newDist.X, newDist.Y);
            //powerUp.Rotation = 360f + collectionData.SinceHit*1080f;
        }

        private void Deactivate(PowerUp powerUp)
        {
            var gameState = _gameDataRegistry.Get<GameState>();
            if (powerUp.Y < -100 || powerUp.Y > gameState.ScreenSize.Y + 10)
                powerUp.IsActive = false;
        }

        public void Draw(PowerUp powerUp, float frameTime)
        {
            powerUp.Sprite.Draw(powerUp);
        }
    }
}
