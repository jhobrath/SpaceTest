using GalagaFighter.Core.Behaviors.Players;
using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects
{
    public abstract class PlayerEffect
    {
        public abstract string IconPath { get; }
        public virtual bool IsProjectile { get; }

        public bool IsActive { get; private set; } = true;
        protected virtual float Duration => 0f;
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

        public virtual void Apply(PlayerStats stats) { }
        public virtual void Apply(PlayerDisplay display) { }

        public virtual IPlayerShootingBehavior? ShootingBehavior { get; protected set;  } = null;
        public virtual IPlayerMovementBehavior? MovementBehavior { get; protected set; } = null;
        public virtual IPlayerCollisionBehavior? CollisionBehavior { get; protected set; } = null;
        public virtual IPlayerInputBehavior? InputBehavior { get; protected set; } = null;

        public void SetMovementBehavior(IPlayerMovementBehavior movementBehavior) => MovementBehavior = movementBehavior;
        public void SetCollisionBehavior(IPlayerCollisionBehavior collisionBehavior) => CollisionBehavior = collisionBehavior;
        public void SetShootingBehavior(IPlayerShootingBehavior shootingBehavior) => ShootingBehavior = shootingBehavior;
        public void SetInputBehavior(IPlayerInputBehavior inputBehavior) => InputBehavior = inputBehavior;

        public virtual void Deactivate() => IsActive = false;
    }
}
