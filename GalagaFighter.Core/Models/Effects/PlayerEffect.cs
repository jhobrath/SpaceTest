using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System.Collections.Generic;

namespace GalagaFighter.Core.Models.Effects
{
    public abstract class PlayerEffect
    {
        public abstract string IconPath { get; }
        public virtual bool IsProjectile { get; } = false;
        public virtual int MaxCount => 0;
        protected virtual float Duration => 0f;
        protected virtual int TotalBullets => 0;
        public bool IsActive { get; private set; } = true;
        
        private float _remainingTime;
        private int _remainingBullets;

        public virtual List<string> DecorationKeys => [];

        public PlayerEffect()
        {
            _remainingTime = Duration;
            _remainingBullets = TotalBullets;
        }

        public virtual void OnUpdate(float frameTime)
        {
            if(IsProjectile)
            {
                //DebugWriter.Write(_remainingTime.ToString());
            }

            if (Duration > 0f)
            {
                _remainingTime -= frameTime;
                if (_remainingTime <= 0f)
                    IsActive = false;
            }
        }

        public virtual void Apply(EffectModifiers modifiers) { }
        public virtual void Deactivate() => IsActive = false;

        protected virtual void HandleShotFired(Projectile projectile)
        {
            _remainingBullets--;
            if (_remainingBullets == 0)
                Deactivate();
        }
    }
}
