using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Players.Updates
{
    public class PlayerCollisionUpdate
    {
        public List<Projectile> Hits { get; set; } = [];
        public List<GameObject> Destroy { get; set; } = [];
        public List<GameObject> Create { get; set; } = [];
        public int DamageDealt { get; set; }
    }
}
