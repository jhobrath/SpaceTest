using Raylib_cs;
using GalagaFighter.Models.Players;
using GalagaFighter.Models.Effects;

namespace GalagaFighter.Models.PowerUps
{
    public class NinjaPowerUp : PowerUp
    {
        public override PowerUpType Type => PowerUpType.Ninja;
        public NinjaPowerUp(Rectangle rect, float speed)
            : base(rect, speed, Raylib.LoadTexture("Sprites/PowerUps/ninja.png"))
        {
        }
        public override PlayerEffect CreateEffect(Player player) => new NinjaShotEffect(player);
        protected override Color GetColor() => Color.SkyBlue;
    }
}