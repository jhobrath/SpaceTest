using Raylib_cs;
using GalagaFighter.Models;
using System;
using System.Numerics;

namespace GalagaFighter
{
    public class PowerUp : GameObject
    {
        public PowerUpType Type;
        private readonly float fallSpeed;
        private readonly Texture2D sprite;

        public PowerUp(Rectangle rect, PowerUpType type, float speed) : base(rect)
        {
            Type = type;
            fallSpeed = speed;
            sprite = SpriteGenerator.CreatePowerUpSprite(type, (int)rect.Width);
        }

        public override void Update(Game game)
        {
            Rect.Y += fallSpeed;
            int screenHeight = Raylib.GetScreenHeight();
            if (Rect.Y > screenHeight)
            {
                IsActive = false;
            }
        }

        public override void Draw()
        {
            if (sprite.Id > 0)
            {
                // Add rotation effect to make power-ups more eye-catching
                float rotation = (float)Raylib.GetTime() * 50; // Rotate slowly
                Vector2 origin = new Vector2(Rect.Width / 2, Rect.Height / 2);
                Vector2 position = new Vector2(Rect.X + origin.X, Rect.Y + origin.Y);
                
                Raylib.DrawTextureEx(sprite, position, rotation, 1.0f, Color.White);
                
                // Add a subtle glow effect
                Color glowColor = Type switch
                {
                    PowerUpType.FireRate => new Color(255, 255, 255, 50),
                    PowerUpType.IceShot => new Color(0, 100, 255, 50),
                    PowerUpType.Wall => new Color(139, 69, 19, 50),
                    _ => new Color(128, 128, 128, 50)
                };
                
                Rectangle glowRect = new Rectangle(Rect.X - 2, Rect.Y - 2, Rect.Width + 4, Rect.Height + 4);
                Raylib.DrawRectangleRec(glowRect, glowColor);
            }
            else
            {
                // Fallback to original color rectangle
                Color color = Type switch
                {
                    PowerUpType.FireRate => Color.White,
                    PowerUpType.IceShot => Color.Blue,
                    PowerUpType.Wall => Color.Brown,
                    _ => Color.Black
                };
                Raylib.DrawRectangleRec(Rect, color);
            }
        }
    }
}
