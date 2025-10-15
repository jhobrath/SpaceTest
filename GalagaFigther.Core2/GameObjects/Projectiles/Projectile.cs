using GalagaFigther.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.GameObjects.Projectiles
{
    public enum ProjectileDirection
    {
        Left, 
        Right 
    };

    public abstract class Projectile : GameObject
    {
        public Guid Owner { get; set; }

        public Projectile(Guid owner, Vector2 position, Vector2 size, Vector2 speed, SpriteBase sprite) 
            : base(position, size, speed, sprite)
        {
            Owner = owner;
        }

        public abstract Vector2 GetBaseSpeed();
        public abstract Vector2 GetBaseSize();
    }
}
