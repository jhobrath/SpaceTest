using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Players
{
    public class PlayerStats
    {
        public float Damage { get; set; } = 1f;
        public float FireRate { get; set; } = 1f;
        public float ProjectileSpeed { get; set; } = 1f;
        public float MovementSpeed { get; set; } = 1f;
        public float Shield { get; set; } = 1f;
    }
}
