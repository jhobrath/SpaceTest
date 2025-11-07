using GalagaFighter.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Players
{
    public class PlayerBoundsData : IGameObjectData<Player>
    {
        public Vector2 Min { get; set; }
        public Vector2 Max { get; set; }
        public Vector2 MaxSpeed { get; set; }

        public Vector2 LastLegalPosition { get; set; }
        public Vector2 LastLegalWorldPosition { get; set; }
    }
}
