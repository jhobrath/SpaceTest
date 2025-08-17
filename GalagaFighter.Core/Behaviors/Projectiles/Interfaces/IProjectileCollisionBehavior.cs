using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.PowerUps;
using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Projectiles.Interfaces
{
    public interface IProjectileCollisionBehavior
    {
        void Apply(Projectile projectile, Player player);
        void Apply(Projectile projectile, PowerUp powerUp);
    }
}
