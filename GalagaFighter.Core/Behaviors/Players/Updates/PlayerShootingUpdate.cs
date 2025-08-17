using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Players.Updates
{
    public class PlayerShootingUpdate
    {
        public List<Projectile> Projectiles { get; set; } = new List<Projectile>();
    }
}
