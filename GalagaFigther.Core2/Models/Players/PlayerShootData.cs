using GalagaFighter.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Players
{
    public class PlayerShootData : IGameObjectData<Player>
    {
        public float ShotCountdown { get; set; }
        public float RecoveryTime { get; set; }
    }
}
