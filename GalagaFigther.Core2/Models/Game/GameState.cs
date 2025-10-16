using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Game
{
    public class GameState
    {
        public Vector2 ScreenSize { get; set; } = new Vector2(1920, 1080);
        public Vector2 UniformScale => ScreenSize / new Vector2(1920, 1080);
    }
}
