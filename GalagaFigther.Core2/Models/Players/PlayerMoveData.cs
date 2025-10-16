using GalagaFighter.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Players
{
    public class PlayerMoveData : IGameObjectData<Player>
    {
        public float DirectionalSpeed { get; set; }
    }
}
