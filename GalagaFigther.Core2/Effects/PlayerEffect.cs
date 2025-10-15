using GalagaFigther.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Effects
{
    public abstract class PlayerEffect
    {
        protected virtual float Duration { get; } = 5f;
        public bool IsActive = true;

        protected float _lifeTime = 0f;

        public abstract void Apply(PlayerModifiers player);

        public virtual void Update(float frameTime)
        {
            _lifeTime += frameTime;

            if (Duration == 0)
                return;

            if(_lifeTime > Duration)
                IsActive = false;
        }
    }
}
