using GalagaFighter.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Players
{
    public class PlayerBaseStats : IGameObjectData<Player>
    {
        public float Health { get; set; } = 100f;
        public float Damage { get; set; } = 1f;
        public float Shield { get; set; } = 1f;
        public float Speed { get; set; } = 500f;
        public float FireRate { get; set; } = .15f;
    }
}
