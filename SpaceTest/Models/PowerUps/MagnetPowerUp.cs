using Raylib_cs;
using GalagaFighter.Models.Players;
using GalagaFighter.Models.Effects;
using GalagaFigther;
using GalagaFigther.Models.Effects;

namespace GalagaFighter.Models.PowerUps
{
    public class MagnetPowerUp : PowerUp
    {
        public MagnetPowerUp(Rectangle rect, float speed)
            : base(rect, speed, TextureLibrary.Get("Sprites/PowerUps/magnet.png"))
        {
        }


        public override PlayerEffect CreateEffect(Player player) => new MagnetShotEffect(player);
        protected override Color GetColor() => Color.SkyBlue;
    }
}