using GalagaFighter.Core.Services;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Collisions
{
    public class CenteredCollision : GameObject
    {
        private float _lifetime = 0f;
        public CenteredCollision(Guid owner, Vector2 center, Vector2 initialSize, Vector2 initialVelocity)
            : base(owner,
                   new SpriteWrapper(TextureService.Get("Sprites/Collisions/default.png"), 38, .02f),
                   new Vector2(center.X, center.Y),
                   initialSize,
                   initialVelocity)
        {
        }

        public override void Draw()
        {
            Sprite.Draw(Rect.Position, Rotation, Rect.Width, Rect.Height, Raylib_cs.Color.White);
        }

        public override void Update(Game game)
        {
            var frameTime = Raylib_cs.Raylib.GetFrameTime();
            _lifetime += frameTime;

            Hurry(1f - frameTime*3);
            Move(Speed.X * frameTime, Speed.Y*frameTime);

            if (_lifetime > .65)
                IsActive = false;

            Sprite.Update(frameTime);
        }
    }
}
