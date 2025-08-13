using GalagaFighter.Models.Players;
using Raylib_cs;

namespace GalagaFighter.Models.Effects
{
    public class BurningEffect : PlayerEffect
    {
        private readonly float redAlpha = .4f;
        private float _count = 0;

        protected override float Duration => 2.0f;
        public override bool AllowSelfStacking => false;

        public BurningEffect(Player player)
            : base(player)
        {
        }

        public override void OnActivate()
        {
            Game.PlayBurningSound();
            Player.Stats.ModifyFireRate(.4f);
        }

        public override void OnDeactivate()
        {
            Player.Stats.ModifyFireRate(1/.4f);
        }
       

        public override void ModifyPlayerRendering(PlayerRendering playerRendering)
        {
            _count = (_count + 1f) % 8f;
            var multiplier = _count / 4f;

            var newRedAlpha = Math.Min(1, Math.Max(0, redAlpha * multiplier));

            // Apply blue tint (alpha is a blend, not replace)
            playerRendering.Color = new Color(
                (byte)(playerRendering.Color.R + (255 - playerRendering.Color.R) * newRedAlpha),
                (byte)(playerRendering.Color.G * (1 - newRedAlpha)),
                (byte)(playerRendering.Color.B * (1 - newRedAlpha)),
                playerRendering.Color.A);
        }
    }
}