using GalagaFighter.Core.Services;
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
        private const int _frameCount = 38;
        private const float _frameLength = .04f;
        private const string _texture = "Sprites/collision.png";
        private const int _frameSkip = 4;

        protected float SpeedDecreaseFactor = 10; //Fully stopped after a fifth of a second

        protected virtual float FrameLength => _frameLength;
        protected virtual int FrameCount => _frameCount;
        protected virtual string Texture => _texture;
        protected virtual int FrameSkip => _frameSkip;

        private float _frameTimer = 0f;

        public DefaultCollision(Vector2 initialPosition, Vector2 initialSize, Vector2 initialVelocity)
            : base(new SpriteWrapper(TextureService.Get(_texture), _frameCount, _frameLength), initialPosition, initialSize, initialVelocity)
        {
        }

        public override void Update(Game game)
        {
            var frameTime = Raylib.GetFrameTime();

            UpdatePosition(frameTime);
            UpdateFrame(frameTime);
            UpdateColor(frameTime);
        }

        private void UpdatePosition(float frameTime)
        {
            var speedChange = 1 - SpeedDecreaseFactor * frameTime;
            Hurry(speedChange, speedChange);
            Move(Speed.X * frameTime, Speed.Y * frameTime);
        }

        private void UpdateFrame(float frameTime)
        {
            if (FrameSkip <= 1)
                Sprite.Update(frameTime);
            else
                _frameTimer += frameTime;

            if (_frameTimer > FrameLength)
                Sprite.CurrentFrame += FrameSkip;

            if (Sprite.CurrentFrame >= Sprite.FrameCount - 1)
                IsActive = false;
        }

        private void UpdateColor(float frameTime)
        {
            var alphaPercentage = Math.Max(0, Math.Min(1f, 2f - ((float)Sprite.CurrentFrame * 2.0f / (float)Sprite.FrameCount)));
            Color = new Color(255, 255, 255, Convert.ToInt32(100f * alphaPercentage));
        }

        public override void Draw()
        {
            Sprite.Draw(Center, 0f, Rect.Width, Rect.Height, Color);
        }
    }
}
