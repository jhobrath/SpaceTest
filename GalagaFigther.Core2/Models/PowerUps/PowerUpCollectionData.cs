using GalagaFighter.Core2.GameObjects.PowerUps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.PowerUps
{
    public class PowerUpCollectionData : IGameObjectData<PowerUp>
    {
        public Vector2? OriginalSize { get; set; }
        public float? OriginalDistance { get;  set; }
        public float SinceHit { get;  set; }
    }
}
