using GalagaFighter.Core.Models.Players;

namespace GalagaFighter.Core.Models.Effects.Offensives
{
    public class TimedBarrageEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/timedbarrage.png";
        public override bool IsProjectile => false;
        protected override float Duration => 300f;

        private float _lifetime = 0f;
        private EffectModifiers? _modifiers;

        public TimedBarrageEffect()
        {
            _lifetime = 0f;
        }

        public override void Apply(EffectModifiers modifiers)
        {
            _modifiers = modifiers;
            base.Apply(modifiers);
        }

        public override void OnUpdate(float frameTime)
        {
            _lifetime += frameTime;
            var timeInCurrentCycle = _lifetime % 4f;

            //DebugWriter.Write(timeInCurrentCycle.ToString());

            if (_modifiers == null)
                return;

            _modifiers.Stats.FireRateMultiplier = .65f;

            // Adjust target arrival time to make bullets sync closer to target
            float baseTravelTime = 2.4f;
            float targetArrivalTime = 3.3f + baseTravelTime; // Changed from 3f to 3.3f
            float remainingTravelTime = targetArrivalTime - timeInCurrentCycle;
            float speedMultiplier = baseTravelTime / remainingTravelTime;

            // Clamp to reasonable range
            speedMultiplier = speedMultiplier*1.5f;

            _modifiers.Projectile.SpeedMultiplier = speedMultiplier;
            _modifiers.Projectile.IgnoreShipMovement = true;

            base.OnUpdate(frameTime);
        }
    }
}
