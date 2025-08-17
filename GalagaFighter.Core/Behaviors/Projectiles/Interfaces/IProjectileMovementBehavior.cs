using GalagaFighter.Core.Behaviors.Projectiles.Updates;
using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Projectiles.Interfaces
{
    public interface IProjectileMovementBehavior
    {
        void Apply(Projectile projectile);
    }
}
