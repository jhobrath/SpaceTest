using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Game
{
    public class PlayerRanges : IGameData
    {
        public float MaxSpeedX { get; set; } = 500f;
        public float MaxSpeedY { get; set; } = 1000f;
        public float MaxRotation { get; set; } = 10f;
    }
}
