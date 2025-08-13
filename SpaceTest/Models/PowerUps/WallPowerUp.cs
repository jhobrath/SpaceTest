using Raylib_cs;
using GalagaFighter.Models.Players;
using GalagaFighter.Models.Effects;
using GalagaFigther;

namespace GalagaFighter.Models.PowerUps
{
    public class WallPowerUp : PowerUp
    {
        public WallPowerUp(Rectangle rect, float speed)
            : base(rect, speed, TextureLibrary.Get("Sprites/PowerUps/wall.png"))
        {
        }
        public override PlayerEffect CreateEffect(Player player) => new WallEffect(player);
        protected override Color GetColor() => Color.Gray;
    }
}