using GalagaFighter.Core.Behaviors.Players.Updates;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Players.Interfaces
{
    public interface IPlayerCollisionBehavior
    {
        void Apply(Player player, Projectile projectile);
    }
}
