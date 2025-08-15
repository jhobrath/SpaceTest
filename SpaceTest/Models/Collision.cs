using GalagaFighter;
using GalagaFighter.Models;
using Raylib_cs;
using System.Numerics;

namespace GalagaFigther.Models
{
    public class Collision : GameObject
    {
        private int _frame = 0;
        private int _frameCount = 0;
        private SpriteWrapper _spriteWrapper;
        private Vector2 _speed;

        private bool _hasReachedLastFrame = false;

        public Collision(Rectangle rect, SpriteWrapper spriteWrapper, Vector2 speed, bool useRight = false, bool useBottom = false)
            :base(rect)
        {
            var width = spriteWrapper.Texture.Width / spriteWrapper.FrameCount;
            var height = spriteWrapper.Texture.Height;

            var size = Math.Max(50, height);

            var rectX = Rect.X + (useRight ? width : 0) + (useRight ? -1 : 1) * (size / 2);
            var rectY = Rect.Y + (useBottom ? height : 0) + (useBottom ? -1 : 1) * (size / 2);

            Rect = new Rectangle(rectX, rectY, size, size); ;
            _speed = speed;

            _spriteWrapper = spriteWrapper;
        }

        public Collision(Rectangle rect, int frameCount, Vector2 speed, bool useRight = false, bool useBottom = false)
            : base(rect)
        {
            var size = Math.Max(50, rect.Height);

            var rectWidth = size;
            var rectHeight = size;

            var rectX = Rect.X + (useRight ? Rect.Width : 0) + (useRight ? -1 : 1) * (size/2);
            var rectY = Rect.Y + (useBottom ? Rect.Height : 0) + (useBottom ? -1 : 1) * (size / 2);

            _speed = speed;
            Rect = new Rectangle(rectX, rectY, size, size);
            _frameCount = frameCount;

            var texture = TextureLibrary.Get("Sprites/collision.png");
            _spriteWrapper = new SpriteWrapper(texture, 38, 0.12f);
        }

        public override void Draw()
        {
            if(_frameCount == 0)
            {
                if (_hasReachedLastFrame && _spriteWrapper.CurrentFrame == 0)
                {
                    IsActive = false;
                    return;
                }

                _spriteWrapper.Draw(Rect.Position, 0f, Rect.Width, Rect.Height);
                _hasReachedLastFrame = _hasReachedLastFrame || _spriteWrapper.CurrentFrame == _spriteWrapper.FrameCount - 1;
                return;
            }


            _frame++;

            if(_frame > _frameCount)
            {
                IsActive = false;
                return;
            }

            var frameToShow = Convert.ToInt32(Math.Floor((38f * (float)_frame) / _frameCount));

            // Draw animated ninja sprite
            _spriteWrapper.DrawAnimated(
                new Vector2(Rect.X, Rect.Y),
                0f,
                Rect.Width,
                Rect.Height,
                frameToShow
            );
        }

        public override void Update(Game game)
        {
            _speed.X = _speed.X * .9f;
            _speed.Y = _speed.Y * .9f;

            Rect.X += _speed.X;
            Rect.Y += _speed.Y;

            if (_frameCount == 0)
                _spriteWrapper.Update(Raylib.GetFrameTime());
        }
    }
}
