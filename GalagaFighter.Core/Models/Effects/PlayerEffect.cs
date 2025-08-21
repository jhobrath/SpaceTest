using GalagaFighter.Core.Models.Players;

namespace GalagaFighter.Core.Models.Effects
{
    public abstract class PlayerEffect
    {
        public abstract string IconPath { get; }
        public virtual bool IsProjectile { get; }
        protected virtual float Duration => 0f;
        public bool IsActive { get; private set; } = true;
        
        private float _remainingTime;

        public PlayerEffect()
        {
            _remainingTime = Duration;
        }

        public virtual void OnUpdate(float frameTime)
        {
            if (Duration > 0f)
            {
                _remainingTime -= frameTime;
                if (_remainingTime <= 0f)
                    IsActive = false;
            }
        }

        public virtual void Apply(EffectModifiers modifiers) { }
        public virtual void Deactivate() => IsActive = false;
    }
}
