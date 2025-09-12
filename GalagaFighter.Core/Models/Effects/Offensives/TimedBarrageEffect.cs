using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects.Offensives
{
    public class TimedBarrageEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/offensives/timedbarrage.png";
        public override bool IsProjectile => false;
        protected override float Duration => 300f;

        private const float _defaultProjectileSpeed = 1666.6667f;

        private float _lifetime = 0f;
        private EffectModifiers? _modifiers;
        private float _originalSpeedMultiplier = -1f;

        public TimedBarrageEffect()
        {
            _lifetime = 0f;
        }

        public override void Apply(EffectModifiers modifiers)
        {
            _modifiers = modifiers;
            _originalSpeedMultiplier = -1;
            base.Apply(modifiers);
        }

        public override void OnUpdate(float frameTime)
        {
            _lifetime += frameTime;
            var timeInCurrentCycle = _lifetime % 4f;

            if (_modifiers == null)
                return;

            _originalSpeedMultiplier = _originalSpeedMultiplier == -1
                ? _modifiers!.Projectile.SpeedMultiplier
                : _originalSpeedMultiplier;

            _modifiers.Stats.FireRateMultiplier = .35f;
            _modifiers.AffectedByShootMeter = false;
            _modifiers.Projectile.Homing *= 0f;

            //Timed barrage is timed to work for a speed of 700f. 
            //We need to compensate for the difference in speed
            var factor = 700f/(_modifiers.GetProjectileSpeed().X * _originalSpeedMultiplier);

            // Adjust target arrival time to make bullets sync closer to target
            float baseTravelTime = 2.4f*factor;
            float targetArrivalTime = 2.2f + baseTravelTime; // Changed from 3f to 3.3f
            float remainingTravelTime = targetArrivalTime - timeInCurrentCycle;
            float speedMultiplier = baseTravelTime / remainingTravelTime;
          
            _modifiers.Projectile.SpeedMultiplier = speedMultiplier;
            _modifiers.Projectile.IgnoreShipMovement = true;

            base.OnUpdate(frameTime);
        }
    }
}
