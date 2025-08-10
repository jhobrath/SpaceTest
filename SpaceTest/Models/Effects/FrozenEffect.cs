using GalagaFighter.Models.Players;
using Raylib_cs;

namespace GalagaFighter.Models.Effects
{
    public class FrozenEffect : PlayerEffect
    {
        private float remainingTime;
        private readonly float slowPerEffect;
        private readonly float blueAlpha;

        public FrozenEffect(Player player, float duration = 5.0f, float slowPerEffect = 0.3f, float maxSlowFactor = 0.1f, float blueAlpha = 0.4f)
            : base(player)
        {
            this.slowPerEffect = slowPerEffect;
            this.blueAlpha = blueAlpha;
            remainingTime = duration;
        }

        public override void OnActivate()
        {
            // No-op: effect is applied in OnUpdate and via hooks
        }

        public override void OnUpdate(float frameTime)
        {
            remainingTime -= frameTime;
            if (remainingTime <= 0)
                IsActive = false;
        }

        public override void OnDeactivate()
        {
            // No-op: effect is removed from list, so no lingering state
        }

        public override float SpeedMultiplier => 1.0f - slowPerEffect;

        public override void ModifyPlayer(Player player, ref float speed, ref Color color)
        {
            speed *= SpeedMultiplier;
            // Apply blue tint (alpha is a blend, not replace)
            color = new Color(
                (byte)(color.R * (1 - blueAlpha)),
                (byte)(color.G * (1 - blueAlpha)),
                (byte)(color.B + (255 - color.B) * blueAlpha),
                color.A);
        }
    }
}