using GalagaFighter.Core.Behaviors.Projectiles;
using GalagaFighter.Core.Behaviors.Projectiles.Interfaces;
using GalagaFighter.Core.Behaviors.Projectiles.Updates;
using GalagaFighter.Core.Models.Players;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public abstract class Projectile : GameObject
    {
        public abstract int Damage { get; }

        protected virtual IProjectileMovementBehavior? MovementBehavior { get; set; }
        protected virtual IProjectileDestroyBehavior? DestroyBehavior { get; set; }
        protected virtual IProjectileCollisionBehavior? CollisionBehavior { get; set; }


        protected Projectile(Guid owner, SpriteWrapper sprite, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed) 
            : base(owner, sprite, initialPosition, initialSize, initialSpeed)
        {
        }

        public virtual void Collide()
        {
            CollisionBehavior?.Apply(this);
        }

        public override void Update(Game game)
        {
            MovementBehavior?.Apply(this);
            DestroyBehavior?.Apply(this);
        }

        public override void Draw()
        {
            Sprite.Draw(Center, 0f, Rect.Width, Rect.Height, Color);
        }

        public void SetMovementBehavior(IProjectileMovementBehavior movementBehavior) => MovementBehavior = movementBehavior;
        public void SetDestroyBehavior(IProjectileDestroyBehavior destroyBehavior) => DestroyBehavior = destroyBehavior;
        public void SetCollisionBehavior(IProjectileCollisionBehavior collisionBehavior) => CollisionBehavior = collisionBehavior;
    }
}
