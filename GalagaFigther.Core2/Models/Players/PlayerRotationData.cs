using GalagaFighter.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Players
{
    public class PlayerRotationData : IGameObjectData<Player>
    {
        public float InitialRotation { get; set; } = float.MinValue;
    }
}
