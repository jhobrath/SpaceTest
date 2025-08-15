using GalagaFighter.Models.Players;

namespace GalagaFighter.Models.Effects
{
    public class FireRateEffect : PlayerEffect
    {
        public override float FireRateMultiplier => .72f;

        protected override float Duration => 0f;

        public override string IconPath => throw new NotImplementedException();

        public FireRateEffect(Player player) : base(player)
        {
        }
    }
}