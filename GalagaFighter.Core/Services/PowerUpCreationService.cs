using GalagaFighter.Core.Models.PowerUps;
using GalagaFighter.Core.Controllers;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IPowerUpService
    {
        void Roll();
    }

    public interface IPowerUpControllerFactory
    {
        IPowerUpController Create();
    }

    public class PowerUpControllerFactory : IPowerUpControllerFactory
    {
        private readonly IObjectService _objectService;

        public PowerUpControllerFactory(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public IPowerUpController Create()
        {
            return new PowerUpController(_objectService);
        }
    }

    public class PowerUpCreationService : IPowerUpService
    {
        private IObjectService _objectService;
        private IPowerUpControllerFactory _powerUpControllerFactory;

        public PowerUpCreationService(IObjectService objectService, IPowerUpControllerFactory powerUpControllerFactory)
        {
            _objectService = objectService;
            _powerUpControllerFactory = powerUpControllerFactory;
        }

        private readonly static List<Func<IPowerUpController, Guid, Rectangle, Vector2, PowerUp>> _powerUpTypes = new List<Func<IPowerUpController, Guid, Rectangle, Vector2, PowerUp>>
        {
            //(up,o,r,f) => new FireRatePowerUp(up,o, r.Position, r.Size, f),
            (up,o,r,f) => new IceShotPowerUp(up,o, r.Position, r.Size, f),
            //(up,o,r,f) => new WoodShotPowerUp(up,o,r.Position,r.Size,f),
            (up,o,r,f) => new NinjaShotPowerUp(up,o,r.Position,r.Size,f),
            (up,o,r,f) => new ExplosiveShotPowerUp(up,o, r.Position, r.Size, f),
            //(up,o,r,f) => new DoubleShotPowerUp(up,o, r.Position, r.Size, f),
            //(up,o,r,f) => new MagnetPowerUp(up,o, r.Position, r.Size, f),
            (up,o,r,f) => new MudShotPowerUp(up,o, r.Position, r.Size, f)
        };

        public void Roll()
        {
            var output = new List<GameObject>();

            if (Game.Random.Next(0, 60 * 10) != 1)
                return;

            var powerUp = Create();

            if (powerUp != null)
                _objectService.AddGameObject(powerUp);
        }

        public PowerUp Create()
        {
            int powerUpTypeIndex = Game.Random.Next(0, _powerUpTypes.Count);
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            float uniformScale = Math.Min(screenWidth / 1920f, screenHeight / 1080f);
            var powerUpSize = new Vector2(87f,41f);
            var rect = new Rectangle(Game.Random.Next(Convert.ToInt32(.2f * screenWidth), Convert.ToInt32(screenWidth * .8f)),
                -powerUpSize.X, powerUpSize.X, powerUpSize.Y);

            var speed = new Vector2(0, 200f * uniformScale);

            // Create a new controller instance for each power-up
            var powerUpController = _powerUpControllerFactory.Create();
            var powerUp = _powerUpTypes[powerUpTypeIndex](powerUpController, Game.Id, rect, speed);
            return powerUp;
        }
    }
}
