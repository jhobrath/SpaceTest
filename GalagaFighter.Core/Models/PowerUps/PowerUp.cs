using GalagaFighter.Core.Behaviors.PowerUps;
using GalagaFighter.Core.Behaviors.PowerUps.Interfaces;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.PowerUps
{
    public abstract class PowerUp : GameObject
    {
        protected IPowerUpMovementBehavior? MovementBehavior { get; set; }
        protected IPowerUpDestroyBehavior? DestroyBehavior { get; set; }
        protected IPowerUpCollisionBehavior? CollisionBehavior { get; set; }

        public PowerUp(Guid owner, string texture, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
            : base(owner, new SpriteWrapper(TextureService.Get(texture)), initialPosition, initialSize, initialSpeed)
        {
        }

        public void SetMovementBehavior(IPowerUpMovementBehavior movementBehavior)
        {
            MovementBehavior = movementBehavior;
        }

        public void SetDestroyBehavior(IPowerUpDestroyBehavior destroyBehavior)
        {
            DestroyBehavior = destroyBehavior;
        }

        public void SetCollisionBehavior(IPowerUpCollisionBehavior collisionBehavior)
        {
            CollisionBehavior = collisionBehavior;
        }

        public virtual void Collide(Projectile projectile)
        {
            CollisionBehavior?.Apply(this, projectile);
        }

        public override void Update(Game game)
        {
            MovementBehavior?.Apply(this);
            DestroyBehavior?.Apply(this);
        }

        public override void Draw()
        {
            Sprite.Draw(Center, Rotation, Rect.Width, Rect.Height, Color);
        }
    }
}
