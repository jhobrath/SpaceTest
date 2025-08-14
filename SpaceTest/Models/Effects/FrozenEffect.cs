using GalagaFighter.Models.Players;
using Raylib_cs;

namespace GalagaFighter.Models.Effects
{
    public class FrozenEffect : PlayerEffect
    {
        private readonly float slowPerEffect = 0.3f;
        private readonly float blueAlpha = .4f;

        protected override float Duration => 5.0f;
        public override string IconPath => "Sprites/Effects/frozen.png";

        public FrozenEffect(Player player)
            : base(player)
        {
        }

        public override void OnActivate()
        {
            Player.Stats.ModifyFireRate(1/.8f);
            base.OnActivate();
        }

        public override void OnDeactivate()
        {
            Player.Stats.ModifyFireRate(.8f);
        }

        public override float SpeedMultiplier => 1.0f - slowPerEffect;

        public override void ModifyPlayerRendering(PlayerRendering playerRendering)
        {
            // Apply blue tint (alpha is a blend, not replace)
            playerRendering.Color = new Color(
                (byte)(playerRendering.Color.R * (1 - blueAlpha)),
                (byte)(playerRendering.Color.G * (1 - blueAlpha)),
                (byte)(playerRendering.Color.B + (255 - playerRendering.Color.B) * blueAlpha),
                playerRendering.Color.A);
        }
    }
}