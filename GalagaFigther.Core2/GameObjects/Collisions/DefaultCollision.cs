using GalagaFighter.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace GalagaFighter.Core2.GameObjects.Collisions
{
    public class DefaultCollision : Collision
    {
        public DefaultCollision(Vector2 position, float size) 
            : base(Game.Id, position - Vector2.One*size*.5f, Vector2.One*size, Vector2.Zero, new NonRepeatingAnimatedImageSprite("Sprites/Collisions/burst.png", 8, 104, 104, .06f))
        {
            Color = Color.White.ShiftHueForTexture(40f);
        }
    }
}
