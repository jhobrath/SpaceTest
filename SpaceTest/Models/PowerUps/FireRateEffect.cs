using GalagaFighter.Models.Players;

namespace GalagaFighter.Models.PowerUps
{
    public class FireRateEffect : PowerUpEffect
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