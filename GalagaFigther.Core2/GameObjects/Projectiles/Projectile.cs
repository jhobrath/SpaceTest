using GalagaFighter.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.Projectiles
{
    public enum ProjectileDirection
    {
        Left, 
        Right 
    };

    public abstract class Projectile : GameObject
    {
        protected abstract Vector2 BaseSpeed { get; }
        protected abstract Vector2 BaseSize { get; }

        public Projectile(Guid owner, Vector2 position, Vector2 size, Vector2 speed, SpriteBase sprite)
            : base(owner, position, size, speed, sprite)
        {
            Owner = owner;
        }
    }
}
