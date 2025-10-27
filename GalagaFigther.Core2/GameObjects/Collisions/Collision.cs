using GalagaFighter.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.Collisions
{
    public abstract class Collision : GameObject
    {
        public virtual float Duration => 3f;
        public float Lifetime { get; set; }
        public Collision(Guid owner, Vector2 position, Vector2 size, Vector2 speed, SpriteBase sprite) 
            : base(owner, position, size, speed, sprite)
        {
        }
    }
}
