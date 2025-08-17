using GalagaFighter.Core.Behaviors.Projectiles;
using GalagaFighter.Core.Behaviors.Projectiles.Interfaces;
using GalagaFighter.Core.Behaviors.Projectiles.Updates;
using Raylib_cs;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public abstract class Projectile : GameObject
    {
        protected SpriteWrapper Sprite;
        public virtual Vector2 Speed { get; }
        public abstract int Damage { get; }
        public virtual Color Color => Color.White;

        protected virtual IProjectileMovementBehavior MovementBehavior { get; set; } = new ProjectileMovementBehavior();
        protected virtual IProjectileDestroyBehavior DestroyBehavior { get; set; } = new ProjectileDestroyBehavior();
        protected virtual IProjectileCollisionBehavior CollisionBehavior { get; set; } = new ProjectileCollisionBehavior();


        protected Projectile(SpriteWrapper sprite, Vector2 position, Vector2 size, Vector2 speed) 
            : base(new Rectangle(position, size))
        {
            Speed = speed;
            IsActive = true;
            Sprite = sprite;
        }

        public virtual List<GameObject> Collide()
        {
            return CollisionBehavior.Apply(this);
        }

        public override void Update(Game game)
        {
            MovementBehavior.Apply(this);
            DestroyBehavior.Apply(this);
        }

        public override void Draw()
        {
            Sprite.Draw(Center, 0f, Rect.Width, Rect.Height, Color);
        }
    }
}
