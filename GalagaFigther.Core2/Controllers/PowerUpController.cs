using GalagaFigther.Core2.GameObjects.PowerUps;
using GalagaFigther.Core2.Models.Game;
using GalagaFigther.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Controllers
{
    public interface IPowerUpController : IController<PowerUp>
    {

    }
    public class PowerUpController : IPowerUpController
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        public PowerUpController(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public void Update(PowerUp powerUp, float frameTime)
        {
            Rotate(powerUp, frameTime);
            Move(powerUp, frameTime);
            Deactivate(powerUp);
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
