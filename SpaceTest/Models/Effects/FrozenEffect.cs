using GalagaFighter.Models.Players;
using Raylib_cs;

namespace GalagaFighter.Models.Effects
{
    public class FrozenEffect : PlayerEffect
    {
        private float remainingTime;
        private readonly float slowPerEffect;
        private readonly float maxSlowFactor;
        private readonly float blueAlpha;

        public FrozenEffect(Player player, float duration = 5.0f, float slowPerEffect = 0.3f, float maxSlowFactor = 0.1f, float blueAlpha = 0.4f)
            : base(player)
        {
            this.slowPerEffect = slowPerEffect;
            this.maxSlowFactor = maxSlowFactor;
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

        public float GetSlowMultiplier()
        {
            // Each frozen effect stacks
            int count = Player.Stats.GetActiveEffectCount<FrozenEffect>();
            float totalSlow = count * slowPerEffect;
            float clampedSlow = Math.Min(totalSlow, 1.0f - maxSlowFactor);
            return 1.0f - clampedSlow;
        }

        public bool ShouldTintBlue() => true;
        public float GetBlueAlpha() => blueAlpha;
    }
}