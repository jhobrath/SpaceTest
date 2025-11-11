using GalagaFighter.Core2.GameObjects.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Projectiles
{
    public class DamageByDistanceState : IGameObjectData<Projectile>
    {
        public Vector2? SpawnPoint { get; set; }
        public float OriginalDamage { get; internal set; }
    }
}
