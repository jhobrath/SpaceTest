using Raylib_cs;
using GalagaFighter.Models.Players;
using GalagaFighter.Models.Effects;

namespace GalagaFighter.Models.PowerUps
{
    public class WallPowerUp : PowerUp
    {
        public override PowerUpType Type => PowerUpType.Wall;
        public WallPowerUp(Rectangle rect, float speed)
            : base(rect, speed, Raylib.LoadTexture("Sprites/PowerUps/wall.png"))
        {
        }
        public override PlayerEffect CreateEffect(Player player) => new WallEffect(player);
        protected override Color GetColor() => Color.Gray;
    }
}