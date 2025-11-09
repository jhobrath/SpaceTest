using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Shields;
using GalagaFighter.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Players
{
    public class PlayerShieldState : IGameObjectData<Player>
    {
        public bool IsDrawing { get; set; } = false;
        public Vector2? Start { get; set; } = null;
        public List<ShieldPixel> JustDrawn { get; set; } = [];
    }
}
