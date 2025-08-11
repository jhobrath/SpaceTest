using GalagaFighter.Models.Players;

namespace GalagaFighter.Models.Effects
{
    public class IceShotEffect : ProjectileEffect
    {
        protected override float Duration => 10.0f;

        public IceShotEffect(Player player) : base(player)
        {
        }

        protected override int ProjectileWidth => 40;
        protected override int ProjectileHeight => 20;
        protected override bool OneTimeUse => false;
    }
}