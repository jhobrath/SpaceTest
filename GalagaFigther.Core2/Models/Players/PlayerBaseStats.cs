using GalagaFighter.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Players
{
    public class PlayerBaseStats : IGameObjectData<Player>
    {
        public float Health { get; set; } = 100f;
        public float Damage { get; set; } = 1f;
        public float Shield { get; set; } = 1f;
        public float FireRate { get; set; } = .15f;

        public Vector2 Speed { get; set; } = new(800, 650);
        public Vector2 Acceleration { get; set; } = new(4700, 4400);
        public Vector2 Drag { get; set; } = new(2700, 2400);
    }
}
