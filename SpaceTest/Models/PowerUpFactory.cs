using GalagaFighter.Models.Effects;
using GalagaFighter.Models.Players;
using GalagaFighter.Models.PowerUps;
using Raylib_cs;
using SpaceTest.Models.Projectiles;
using System;
using System.Numerics;

namespace GalagaFighter.Models
{

    public static class PowerUpFactory
    {
        private static Random _random = new Random();

        private readonly static List<Func<Rectangle, float, PowerUp>> _powerUpTypes = new List<Func<Rectangle, float, PowerUp>> 
        {
            (r,f) => new FireRatePowerUp(r,f),
            (r,f) => new IceShotPowerUp(r,f),
            (r,f) => new WallPowerUp(r,f),
            (r,f) => new NinjaPowerUp(r,f),
            (r,f) => new ExplosivePowerUp(r, f)
        };

        public static PowerUp Create()
        {
            int powerUpTypeIndex = _random.Next(0, _powerUpTypes.Count);
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            float uniformScale = Math.Min(screenWidth / 1920f, screenHeight / 1080f);
            int powerUpSize = (int)(30 * uniformScale);
            var rect = new Rectangle(_random.Next(Convert.ToInt32(.1f * screenWidth), Convert.ToInt32(screenWidth * .9f)),
                -powerUpSize, powerUpSize, powerUpSize);

            return _powerUpTypes[powerUpTypeIndex](rect, 2f * uniformScale);
        }
    }
}
