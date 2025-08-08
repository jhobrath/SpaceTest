using Raylib_cs;
using GalagaFighter.Models.Players;
using GalagaFighter.Models.Effects;

namespace GalagaFighter.Models.PowerUps
{
    public class IceShotPowerUp : PowerUp
    {
        public override PowerUpType Type => PowerUpType.IceShot;
        public IceShotPowerUp(Rectangle rect, float speed)
            : base(rect, speed, Raylib.LoadTexture("Sprites/PowerUps/ice.png"))
        {
        }
        public override PlayerEffect CreateEffect(Player player) => new IceShotEffect(player);
        protected override Color GetColor() => Color.SkyBlue;
    }
}