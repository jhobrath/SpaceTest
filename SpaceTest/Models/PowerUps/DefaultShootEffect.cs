using GalagaFighter.Models.Players;
using SpaceTest.Models.Projectiles;

namespace GalagaFighter.Models.PowerUps
{
    public class DefaultShootEffect : ProjectileEffect
    {
        public DefaultShootEffect(Player player) : base(player) { }

        protected override ProjectileType ProjectileType => ProjectileType.Normal;
        protected override int ProjectileWidth => 30;
        protected override int ProjectileHeight => 15;
        protected override bool OneTimeUse => false;
    }
}