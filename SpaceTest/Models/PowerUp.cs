using Raylib_cs;
using GalagaFighter.Models.Players;
using GalagaFighter.Models.PowerUps;
using System;
using System.Numerics;
using GalagaFighter.Models;

namespace GalagaFighter
{
    public class PowerUp : GameObject
    {
        private readonly PowerUpType type;
        private readonly float speed;
        private readonly Texture2D sprite;

        public PowerUp(Rectangle rect, PowerUpType type, float speed) : base(rect)
        {
            this.type = type;
            this.speed = speed;
            IsActive = true;
            sprite = SpriteGenerator.CreatePowerUpSprite(type, (int)rect.Width, (int)rect.Height);
        }

        public override void Update(Game game)
        {
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
                // Add rotation effect to make power-ups more eye-catching
                float rotation = (float)Raylib.GetTime() * 50; // Rotate slowly
                Vector2 origin = new Vector2(Rect.Width / 2, Rect.Height / 2);
                Vector2 position = new Vector2(Rect.X + origin.X, Rect.Y + origin.Y);
                
                Raylib.DrawTextureEx(sprite, position, rotation, 1.0f, Color.White);
                
                // Add a subtle glow effect
                Color glowColor = type switch
                {
                    PowerUpType.FireRate => new Color(255, 255, 255, 50), // White for bullet capacity
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
                Color color = type switch
                {
                    PowerUpType.FireRate => Color.White, // White for bullet capacity
                    PowerUpType.IceShot => Color.Blue,
                    PowerUpType.Wall => Color.Brown,
                    _ => Color.Black
                };
                Raylib.DrawRectangleRec(Rect, color);
            }
        }

        public PowerUpEffect CreateEffect(Player player)
        {
            return type switch
            {
                PowerUpType.FireRate => new FireRateEffect(player),
                PowerUpType.IceShot => new IceShotEffect(player),
                PowerUpType.Wall => new WallEffect(player),
                _ => null
            };
        }

        public void OnCollected(Player player)
        {
            var effect = CreateEffect(player);
            if (effect != null)
            {
                player.Stats.AddEffect(player, effect);
            }
        }
    }
}
