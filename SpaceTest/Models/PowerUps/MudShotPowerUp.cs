using Raylib_cs;
using GalagaFighter.Models.Players;
using GalagaFighter.Models.Effects;
using GalagaFigther;

namespace GalagaFighter.Models.PowerUps
{
    public class MudShotPowerUp : PowerUp
    {
        public MudShotPowerUp(Rectangle rect, float speed)
            : base(rect, speed, TextureLibrary.Get("Sprites/PowerUps/Mud.png"))
        {
        }
        public override PlayerEffect CreateEffect(Player player) => new MudShotEffect(player);
        protected override Color GetColor() => Color.SkyBlue;
    }
}