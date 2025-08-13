using GalagaFigther;
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

        public void DrawPlayer(Rectangle playerRect, PlayerRendering playerRendering, bool isMoving)
        {
            DrawShip(playerRect, playerRendering);

            if (isMoving)
            {
                //DrawEngineTrail(playerRect);
            }
        }

        private void DrawShip(Rectangle playerRect, PlayerRendering playerRendering)
        {
            var xOffset = isPlayer1 ? playerRect.Width : 0;
            var yOffset = isPlayer1 ? 0 : playerRect.Height;
            Vector2 position = new Vector2(playerRect.X + xOffset, playerRect.Y + yOffset);
            var rotation = playerRendering.Rotation;
            var scale = playerRendering.Scale;
            var skew = playerRendering.Skew;

            var texture = TextureLibrary.Get(playerRendering.Texture);

            rotation = rotation + playerRendering.Skew * 5;
            //if(skew == 0)
            //{
                Raylib.DrawTextureEx(texture, position, rotation, scale, playerRendering.Color);
            //}
            //else
            //    DrawBankingSpaceship(texture, playerRect.Position, skew, scale, rotation, playerRendering.Color, isPlayer1);
        }

        public static void DrawBankingSpaceship(Texture2D texture, Vector2 position, float bankingAmount, float scale, float rotationDegrees, Color color, bool isPlayer1)
        {
            if (texture.Id == 0) return;

            // Banking amount should be between -1.0 and 1.0
            bankingAmount = Math.Clamp(bankingAmount, -1.0f, 1.0f);

            // Calculate scaled dimensions
            float scaledWidth = texture.Width * scale;
            float scaledHeight = texture.Height * scale;

            // Calculate skew offset based on scaled dimensions
            float maxSkewOffset = scaledWidth * 0.15f; // 15% of scaled width max skew
            float skewOffset = maxSkewOffset * bankingAmount;

            // Calculate the center point for rotation (this should match DrawTextureEx behavior)
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            Vector2 centerPosition = new Vector2(position.X + origin.X * scale, position.Y + origin.Y * scale);

            Vector2 topLeft;
            Vector2 topRight;
            Vector2 bottomLeft;
            Vector2 bottomRight;

            // Define the four vertex positions with banking skew (relative to center)
            if (isPlayer1)
            { 
                topLeft = new Vector2(-origin.X * scale - skewOffset,-origin.Y * scale - skewOffset);
                topRight = new Vector2(origin.X * scale + skewOffset,-origin.Y * scale + skewOffset);
                bottomLeft = new Vector2(-origin.X * scale - skewOffset,origin.Y * scale - skewOffset);
                bottomRight = new Vector2(origin.X * scale - skewOffset,origin.Y * scale - skewOffset);
            }
            else
            {
                topLeft = new Vector2(-origin.X * scale - skewOffset,-origin.Y * scale - skewOffset);
                topRight = new Vector2(origin.X * scale + skewOffset,-origin.Y * scale + skewOffset);
                bottomLeft = new Vector2(-origin.X * scale - skewOffset,origin.Y * scale - skewOffset);
                bottomRight = new Vector2(origin.X * scale - skewOffset,origin.Y * scale - skewOffset);
            }

                // Apply rotation to all vertices
                float rotationRad = rotationDegrees * (float)(Math.PI / 180.0);
            topLeft = RotatePoint(topLeft, Vector2.Zero, rotationRad);
            topRight = RotatePoint(topRight, Vector2.Zero, rotationRad);
            bottomLeft = RotatePoint(bottomLeft, Vector2.Zero, rotationRad);
            bottomRight = RotatePoint(bottomRight, Vector2.Zero, rotationRad);

            // Translate to final position
            topLeft = new Vector2(topLeft.X + centerPosition.X, topLeft.Y + centerPosition.Y);
            topRight = new Vector2(topRight.X + centerPosition.X, topRight.Y + centerPosition.Y);
            bottomLeft = new Vector2(bottomLeft.X + centerPosition.X, bottomLeft.Y + centerPosition.Y);
            bottomRight = new Vector2(bottomRight.X + centerPosition.X, bottomRight.Y + centerPosition.Y);

            // Draw the skewed and rotated texture
            Rlgl.SetTexture(texture.Id);
            Rlgl.Begin(DrawMode.Quads);
            Rlgl.Color4ub(color.R, color.G, color.B, color.A);// 255, 255, 255, 255);

            // Counter-clockwise winding order
            Rlgl.TexCoord2f(0, 0); Rlgl.Vertex2f(topLeft.X, topLeft.Y);
            Rlgl.TexCoord2f(0, 1); Rlgl.Vertex2f(bottomLeft.X, bottomLeft.Y);
            Rlgl.TexCoord2f(1, 1); Rlgl.Vertex2f(bottomRight.X, bottomRight.Y);
            Rlgl.TexCoord2f(1, 0); Rlgl.Vertex2f(topRight.X, topRight.Y);

            Rlgl.End();
            Rlgl.SetTexture(0);
        }

        private static Vector2 RotatePoint(Vector2 point, Vector2 center, float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            float dx = point.X - center.X;
            float dy = point.Y - center.Y;

            return new Vector2(
                center.X + dx * cos - dy * sin,
                center.Y + dx * sin + dy * cos
            );
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

    public class PlayerRendering
    {
        public float Scale { get; set; }
        public Color Color { get; set; }
        public float Rotation { get; set; }
        public float Skew { get; set; }
        public string Texture { get; set; }
    }
}