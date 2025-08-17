using GalagaFighter.Core.Behaviors.PowerUps;
using GalagaFighter.Core.Behaviors.PowerUps.Interfaces;
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
        protected virtual IPowerUpMovementBehavior MovementBehavior => new PowerUpMovementBehavior();
        protected virtual IPowerUpDestroyBehavior DestroyBehavior => new PowerUpDestroyBehavior();

        public PowerUp(string texture, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
            : base(new SpriteWrapper(TextureService.Get(texture)), initialPosition, initialSize, initialSpeed)
        {
        }

        public override void Update(Game game)
        {
            MovementBehavior.Apply(this);
            DestroyBehavior.Apply(this);
        }

        public override void Draw()
        {
            Sprite.Draw(Center, Rotation, Rect.Width, Rect.Height, Color);
        }
    }
}
