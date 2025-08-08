using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Models.Players
{
    public class PlayerRenderer
    {
        private readonly Texture2D shipSprite;
        private readonly bool isPlayer1;
        private readonly float scaleFactor;

        public PlayerRenderer(Texture2D shipSprite, bool isPlayer1, float scaleFactor)
        {
            this.shipSprite = shipSprite;
            this.isPlayer1 = isPlayer1;
            this.scaleFactor = scaleFactor;
        }

        public void DrawPlayer(Rectangle playerRect, bool isSlowed, bool isMoving)
        {
            DrawShip(playerRect, isSlowed);
            
            if (isMoving)
            {
                DrawEngineTrail(playerRect);
            }
        }

        private void DrawShip(Rectangle playerRect, bool isSlowed)
        {
            float rotation = isPlayer1 ? 90f : -90f;
            var xOffset = isPlayer1 ? playerRect.Width : 0;
            var yOffset = isPlayer1 ? 0 : playerRect.Height;
            Vector2 position = new Vector2(playerRect.X + xOffset, playerRect.Y + yOffset);
            var scale = playerRect.Width / shipSprite.Width;
            
            Raylib.DrawTextureEx(shipSprite, position, rotation, scale, Color.White);
            

            if (isSlowed)
            {
                Raylib.DrawTextureEx(shipSprite, position, rotation, scale, new Color(0, 0, 255, 100));
            }
        }

        private void DrawEngineTrail(Rectangle playerRect)
        {
            int trailX, trailY;
            int trailOffset = (int)(15 * scaleFactor);
            
            if (isPlayer1)
            {
                trailX = (int)playerRect.X - trailOffset;
                trailY = (int)(playerRect.Y + playerRect.Height / 2);
            }
            else
            {
                trailX = (int)(playerRect.X + playerRect.Width + trailOffset / 2);
                trailY = (int)(playerRect.Y + playerRect.Height / 2);
            }
            
            Color trailColor = new Color(255, 150, 0, 150);
            
            int particleCount = (int)(7 * scaleFactor);
            int maxRadius = (int)(6 * scaleFactor);
            int spacing = (int)(4 * scaleFactor);
            
            for (int i = 0; i < particleCount; i++)
            {
                int offset = i * (isPlayer1 ? -spacing : spacing);
                int radius = Math.Max(1, maxRadius - i);
                
                Raylib.DrawCircle(trailX + offset, trailY, radius, trailColor);
            }
        }

        public void DrawDebugBounds(Rectangle rect)
        {
            // Debug: Draw the player's bounding rectangle (red)
            Raylib.DrawRectangleLinesEx(rect, 2, Color.Red);

            // Debug: Draw the sprite's actual drawn bounds (green)
            float spriteX = rect.X + rect.Width / 2 - rect.Width / 2;
            float spriteY = rect.Y + rect.Height / 2 - rect.Height / 2;
            Rectangle spriteBounds = new Rectangle(spriteX, spriteY, rect.Width, rect.Height);
            Raylib.DrawRectangleLinesEx(spriteBounds, 2, Color.Green);
        }
    }
}