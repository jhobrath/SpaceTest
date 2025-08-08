using Raylib_cs;
using GalagaFighter.Models.Players;
using GalagaFighter.Models.Effects;
using GalagaFighter;
using GalagaFighter.Models;
using System.Numerics;

namespace GalagaFighter.Models.PowerUps
{
    public abstract class PowerUp : GameObject
    {
        protected readonly float speed;
        protected readonly Texture2D sprite;
        public abstract PowerUpType Type { get; }
        protected  float _rotation = 0f;

        protected PowerUp(Rectangle rect, float speed, Texture2D sprite) : base(rect)
        {
            this.speed = speed;
            this.sprite = sprite;
            IsActive = true;
        }

        public override void Update(Game game)
        {
            var frameTime = Raylib.GetFrameTime();
            _rotation += frameTime * 100f; // Rotate at 100 degrees per second

            Rect.Y += speed;
            if (Rect.Y > Raylib.GetScreenHeight())
            {
                IsActive = false;
            }
        }

        public override void Draw()
        {
            if (sprite.Id > 0)
            {
                float destHeight = Rect.Height * 1.5f;
                float destWidth = 2.1f * Rect.Height * 1.5f;
                Vector2 center = new Vector2(Rect.X + Rect.Width / 2f, Rect.Y + Rect.Height / 2f);
                Raylib.DrawTexturePro(
                    sprite,
                    new Rectangle(0, 0, sprite.Width, sprite.Height),
                    new Rectangle(center.X, center.Y, destWidth, destHeight),
                    new Vector2(destWidth / 2f, destHeight / 2f),
                    _rotation,
                    Color.White
                );
            }
            else
            {
                Raylib.DrawRectangleRec(Rect, GetColor());
            }
        }

        public abstract PlayerEffect CreateEffect(Player player);
        protected abstract Color GetColor();
    }
}
