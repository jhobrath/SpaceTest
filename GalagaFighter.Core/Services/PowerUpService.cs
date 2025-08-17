using GalagaFighter.Core.Behaviors.Players;
using GalagaFighter.Core.Behaviors.PowerUps;
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
    public interface IPowerUpService
    {
        void Roll();
    }

    public class PowerUpService : IPowerUpService
    {
        private IObjectService _objectService;

        public PowerUpService(IObjectService objectService)
        {
            _objectService = objectService;
        }

        private readonly static List<Func<Guid, Rectangle, Vector2, PowerUp>> _powerUpTypes = new List<Func<Guid, Rectangle, Vector2, PowerUp>>
        {
            (o,r,f) => new FireRatePowerUp(o, r.Position, r.Size, f),
            //(r,f) => new IceShotPowerUp(r,f),
            //(r,f) => new WallPowerUp(r,f),
            //(r,f) => new NinjaPowerUp(r,f),
            //(r,f) => new ExplosivePowerUp(r, f),
            //(r,f) => new MagnetPowerUp(r,f),
            //(r,f) => new MudShotPowerUp(r,f)
        };

        public void Roll()
        {
            var output = new List<GameObject>();

            if (Game.Random.Next(0, 60 * 5) != 1)
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

            var powerUp = _powerUpTypes[powerUpTypeIndex](Game.Id, rect, speed);
            powerUp.SetMovementBehavior(new PowerUpMovementBehavior());
            powerUp.SetDestroyBehavior(new PowerUpDestroyBehavior());
            powerUp.SetCollisionBehavior(new PowerUpCollisionBehavior(_objectService));
            return powerUp;
        }
    }
}
