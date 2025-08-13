using Raylib_cs;
using GalagaFighter.Models.Players;
using GalagaFighter.Models.Effects;
using GalagaFigther;

namespace GalagaFighter.Models.PowerUps
{
    public class FireRatePowerUp : PowerUp
    {
        public FireRatePowerUp(Rectangle rect, float speed)
            : base(rect, speed, TextureLibrary.Get("Sprites/PowerUps/firerate.png"))
        {
        }
        public override PlayerEffect CreateEffect(Player player) => new FireRateEffect(player);
        protected override Color GetColor() => new Color(255, 150, 0, 255);
    }
}