using GalagaFighter.Models;
using GalagaFighter.Models.Players;

namespace GalagaFighter.Models.Effects
{
    public class WallEffect : ProjectileEffect
    {
        public WallEffect(Player player) : base(player) { }

        protected override ProjectileType ProjectileType => ProjectileType.Wall;
        protected override int ProjectileWidth => 150;
        protected override int ProjectileHeight => 15;
        protected override bool OneTimeUse => true;

        public override void OnActivate()
        {
        }

        public override void OnDeactivate()
        {
        }
    }
}