using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Collisions
{
    public class DefaultCollision : GameObject
    {

        protected SpriteWrapper Sprite;
        protected Vector2 Speed;
        protected float SpeedDecreaseFactor = 10; //Fully stopped after a fifth of a second
        protected virtual float FrameLength => .04f;
        protected virtual int FrameCount => 38;
        protected virtual string Texture => "Sprites/collision.png";
        protected virtual int FrameSkip => 4;

        protected virtual Color GetColor()
        {
            var alphaPercentage = Math.Max(0, Math.Min(1f, 2f - ((float)Sprite.CurrentFrame * 2.0f / (float)Sprite.FrameCount)));
            return new Color(255, 255, 255, Convert.ToInt32(100f * alphaPercentage));
        }

        private float _frameTimer = 0f;

        public DefaultCollision(Vector2 initialPosition, Vector2 initialSize, Vector2 initialVelocity)
            : base(new Rectangle(initialPosition, initialSize))
        {
            Sprite = new SpriteWrapper(TextureLibrary.Get(Texture), FrameCount, FrameLength);
            Speed = initialVelocity;
        }

        public override void Update(Game game)
        {
            var frameTime = Raylib.GetFrameTime();

            Speed.X *= (1-SpeedDecreaseFactor*frameTime);
            Speed.Y *= (1-SpeedDecreaseFactor*frameTime);

            //Speed.X *= .9f;

            Rect.X = Rect.X + Speed.X*frameTime;
            Rect.Y = Rect.Y + Speed.Y*frameTime;

            if(FrameSkip <= 1)
                Sprite.Update(frameTime);
            else
                _frameTimer += frameTime;

            if (_frameTimer > FrameLength)
                Sprite.CurrentFrame += FrameSkip;

            if (Sprite.CurrentFrame >= Sprite.FrameCount - 1)
                IsActive = false;
        }

        public override void Draw()
        {
            Sprite.Draw(Center, 0f, Rect.Width, Rect.Height, GetColor());
        }
    }
}
