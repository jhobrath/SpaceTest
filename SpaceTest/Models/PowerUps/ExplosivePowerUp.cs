using Raylib_cs;
using GalagaFighter.Models.Players;
using GalagaFighter.Models.Effects;
using GalagaFigther;

namespace GalagaFighter.Models.PowerUps
{
    public class ExplosivePowerUp : PowerUp
    {
        public ExplosivePowerUp(Rectangle rect, float speed)
            : base(rect, speed, TextureLibrary.Get("Sprites/PowerUps/bomb.png"))
        {
        }
        public override PlayerEffect CreateEffect(Player player) => new ExplosiveShotEffect(player);
        protected override Color GetColor() => new Color(255, 150, 0, 255);
    }
}