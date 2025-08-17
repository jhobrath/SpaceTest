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
        public virtual void Apply(PlayerStats stats) { }
        public virtual void Apply(PlayerDisplay stats) { }

        public virtual IPlayerShootingBehavior? ShootingBehavior { get; } = null;
        public virtual IPlayerMovementBehavior? MovementBehavior { get; } = null;
        public virtual IPlayerCollisionBehavior? CollisionBehavior { get; } = null;
        public virtual IPlayerInputBehavior? InputBehavior { get; } = null;
    }
}
