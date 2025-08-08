using GalagaFighter.Models.Players;

namespace GalagaFighter.Models.Effects
{
    public class FireRateEffect : PlayerEffect
    {
        private readonly float multiplier = 0.93f; // 7% faster fire rate

        public FireRateEffect(Player player) : base(player)
        {
        }

        public override void OnActivate()
        {
            Player.Stats.ModifyFireRate(multiplier);
        }
    }
}