using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Models
{
    public class GameState
    {
        public Vector2 ScreenSize { get; set; }
        public Vector2 UniformScale => ScreenSize / new Vector2(1920, 1080);
    }
}
