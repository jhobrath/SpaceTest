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
    public static class PowerUpService
    {
        private readonly static List<Func<Rectangle, Vector2, PowerUp>> _powerUpTypes = new List<Func<Rectangle, Vector2, PowerUp>>
        {
            (r,f) => new FireRatePowerUp(r.Position, r.Size, f),
            //(r,f) => new IceShotPowerUp(r,f),
            //(r,f) => new WallPowerUp(r,f),
            //(r,f) => new NinjaPowerUp(r,f),
            //(r,f) => new ExplosivePowerUp(r, f),
            //(r,f) => new MagnetPowerUp(r,f),
            //(r,f) => new MudShotPowerUp(r,f)
        };

        public static List<GameObject> SpawnPowerUps()
        {
            var output = new List<GameObject>();

            if (Game.Random.Next(0, 60 * 5) != 1)
                return output;

            var powerUp = Create();

            if (powerUp != null)
                output.Add(powerUp);

            return output;
        }

        public static PowerUp Create()
        {
            int powerUpTypeIndex = Game.Random.Next(0, _powerUpTypes.Count);
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            float uniformScale = Math.Min(screenWidth / 1920f, screenHeight / 1080f);
            var powerUpSize = new Vector2(87f,41f);
            var rect = new Rectangle(Game.Random.Next(Convert.ToInt32(.2f * screenWidth), Convert.ToInt32(screenWidth * .8f)),
                -powerUpSize.X, powerUpSize.X, powerUpSize.Y);

            var speed = new Vector2(0, 200f * uniformScale);

            return _powerUpTypes[powerUpTypeIndex](rect, speed);
        }
    }
}
