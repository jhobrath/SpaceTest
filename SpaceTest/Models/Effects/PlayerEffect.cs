using GalagaFighter.Models.Players;

namespace GalagaFighter.Models.Effects
{
    public abstract class PlayerEffect
    {
        protected readonly Player Player;
        public bool IsActive { get; protected set; } = true;

        private float _remainingTime = 0f;

        // Speed multiplier for stacking movement effects
        public virtual float SpeedMultiplier => 1.0f;

        protected virtual float Duration => 0f;

        public virtual bool AllowSelfStacking => true;
        public virtual bool DisableShooting => false;

        public abstract string IconPath { get; }

        public virtual void OnStatsSwitch()
        {

        }

        protected PlayerEffect(Player player)
        {
            Player = player;
            _remainingTime = Duration;
        }

        public virtual void OnActivate() { }
        public virtual void OnUpdate(float frameTime) 
        {
            if (Duration != 0f)
            {
                _remainingTime -= frameTime;
                if (_remainingTime <= 0)
                    IsActive = false;
            }
        }
        public virtual void OnDeactivate() { }
        public virtual void OnShoot(Game game) { }
        public bool ShouldDeactivate() => !IsActive;


        public virtual void ModifyPlayerRendering(PlayerRendering playerRendering)
        {
        }

        public void SetMaxRemainingTime(float duration)
        {
            _remainingTime = Math.Min(_remainingTime, duration);
        }
    }
}