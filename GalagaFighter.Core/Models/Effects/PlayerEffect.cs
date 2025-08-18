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
    public class PlayerEffect
    {
        public virtual float TimeDuration  => 0f;
        public virtual float BulletDuration => 0f;

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
    }
}
