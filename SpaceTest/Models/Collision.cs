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
        private SpriteWrapper spriteWrapper;

        public Collision(Rectangle rect, int frameCount, bool useRight = false, bool useBottom = false)
            : base(rect)
        {
            var size = Math.Max(20, rect.Height);
            var rectX = Rect.X + (useRight ? (rect.Width - rect.Width/2) : -(size/2));
            var rectY = Rect.Y + (useBottom ? (rect.Height + rect.Height/2) : -(size/2));
            
            Rect = new Rectangle(rectX, rectY, Math.Max(50, rect.Height), Math.Max(50, rect.Height));
            _frameCount = frameCount;

            var texture = Raylib.LoadTexture("Sprites/collision.png");
            spriteWrapper = new SpriteWrapper(texture, 38, 0.12f);
        }

        public override void Draw()
        {
            _frame++;

            if(_frame > _frameCount)
            {
                IsActive = false;
                return;
            }

            var frameToShow = Convert.ToInt32(Math.Floor((38f * (float)_frame) / _frameCount));

            // Draw animated ninja sprite
            spriteWrapper.DrawAnimated(
                new Vector2(Rect.X + Rect.Width / 2f, Rect.Y + Rect.Height / 2f),
                0f,
                Rect.Width,
                Rect.Height,
                frameToShow
            );
        }

        public override void Update(Game game)
        {
        }
    }
}
