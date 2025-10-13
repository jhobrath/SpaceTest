using GalagaFigther.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.GameObjects
{
    public class Player : GameObject
    {
        public Player(Vector2 position, Vector2 size, Vector2 speed, SpriteBase sprite) 
            : base(position, size, speed, sprite)
        {
        }
    }
}
