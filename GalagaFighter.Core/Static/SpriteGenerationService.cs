using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Static
{
    public static class SpriteGenerationService
    {
        public static Texture2D CreatePlayerShip(bool isPlayer1, int width = 20, int height = 40)
        {
            // Create a render texture to draw the ship on
            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(width, height);
            
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank); // Transparent background
            
            Color primaryColor = isPlayer1 ? Color.SkyBlue : Color.Red;
            Color accentColor = isPlayer1 ? Color.Blue : Color.Maroon;
            Color engineColor = isPlayer1 ? Color.Yellow : Color.Orange;
            
            // Scale elements proportionally to the ship size
            float scaleX = width / 20f;  // Scale factor based on default width
            float scaleY = height / 40f; // Scale factor based on default height
            
            // Draw ship body (main fuselage) - scaled
            int bodyWidth = (int)(6 * scaleX);
            int bodyHeight = (int)(16 * scaleY);
            Raylib.DrawRectangle(width / 2 - bodyWidth / 2, height / 2 - bodyHeight / 2, bodyWidth, bodyHeight, primaryColor);
            
            // Draw cockpit/nose - scaled
            float noseScale = Math.Min(scaleX, scaleY);
            Raylib.DrawTriangle(
                new Vector2(width / 2, 2 * scaleY),                                    // Nose tip
                new Vector2(width / 2 - 4 * noseScale, height / 2 - bodyHeight / 2),  // Left base  
                new Vector2(width / 2 + 4 * noseScale, height / 2 - bodyHeight / 2),  // Right base
                accentColor
            );
            
            // Draw wings - scaled
            int wingWidth = (int)((width - 8) * scaleX);
            int wingHeight = (int)(6 * scaleY);
            Raylib.DrawRectangle((int)(4 * scaleX), height / 2 - wingHeight / 2, wingWidth, wingHeight, accentColor);
            
            // Draw engine exhaust - scaled
            int engineWidth = (int)(4 * scaleX);
            int engineHeight = (int)(6 * scaleY);
            int exhaustWidth = (int)(2 * scaleX);
            int exhaustHeight = (int)(4 * scaleY);
            
            Raylib.DrawRectangle(width / 2 - engineWidth / 2, height / 2 + bodyHeight / 2, engineWidth, engineHeight, engineColor);
            Raylib.DrawRectangle(width / 2 - exhaustWidth / 2, height / 2 + bodyHeight / 2 + engineHeight, exhaustWidth, exhaustHeight, Color.White);
            
            // Add some detail lines - scaled
            int lineLength = (int)(12 * scaleY);
            Raylib.DrawLine(width / 2, height / 2 - lineLength / 2, width / 2, height / 2 + lineLength / 2, Color.White);
            
            Raylib.EndTextureMode();
            
            return renderTexture.Texture;
        }
        
        public static Texture2D CreateProjectileSprite(ProjectileType type, int width = 10, int height = 5, Color? color = null)
        {
            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(width, height);
            
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            
            switch (type)
            {
                case ProjectileType.Normal:
                    // Simple bullet shape - scaled properly
                    float scaleX = width / 10f;  // Scale based on default width
                    float scaleY = height / 5f;  // Scale based on default height
                    
                    int bulletHeight = (int)(2 * scaleY);
                    int tipRadius = (int)(1 * Math.Min(scaleX, scaleY));
                    
                    Raylib.DrawRectangle(0, height / 2 - bulletHeight / 2, width, bulletHeight, color ?? Color.Yellow);
                    Raylib.DrawCircle(width - tipRadius, height / 2, tipRadius, Color.White);
                    break;
                    
                case ProjectileType.Ice:
                    // Six-sided horizontally stretched diamond using rectangles
                    float centerX = width / 2f;
                    float centerY = height / 2f;
                    
                    // Main body - horizontal rectangle (wider part of diamond)
                    float mainWidth = width * 0.7f;
                    float mainHeight = height * 0.5f;
                    Raylib.DrawRectangle(
                        (int)(centerX - mainWidth / 2), 
                        (int)(centerY - mainHeight / 2), 
                        (int)mainWidth, 
                        (int)mainHeight, 
                        Color.SkyBlue
                    );
                    
                    // Top and bottom diamond points
                    float pointWidth = width * 0.4f;
                    float pointHeight = height * 0.3f;
                    
                    // Top point
                    Raylib.DrawRectangle(
                        (int)(centerX - pointWidth / 2), 
                        (int)(centerY - height * 0.4f), 
                        (int)pointWidth, 
                        (int)pointHeight, 
                        Color.SkyBlue
                    );
                    
                    // Bottom point
                    Raylib.DrawRectangle(
                        (int)(centerX - pointWidth / 2), 
                        (int)(centerY + height * 0.2f), 
                        (int)pointWidth, 
                        (int)pointHeight, 
                        Color.SkyBlue
                    );
                    
                    // Left and right extended points for horizontal stretch
                    float sideWidth = width * 0.25f;
                    float sideHeight = height * 0.3f;
                    
                    // Left extension
                    Raylib.DrawRectangle(
                        (int)(centerX - width * 0.45f), 
                        (int)(centerY - sideHeight / 2), 
                        (int)sideWidth, 
                        (int)sideHeight, 
                        Color.SkyBlue
                    );
                    
                    // Right extension
                    Raylib.DrawRectangle(
                        (int)(centerX + width * 0.25f), 
                        (int)(centerY - sideHeight / 2), 
                        (int)sideWidth, 
                        (int)sideHeight, 
                        Color.SkyBlue
                    );
                    
                    // Add blue outline for definition
                    Raylib.DrawRectangleLines(
                        (int)(centerX - mainWidth / 2), 
                        (int)(centerY - mainHeight / 2), 
                        (int)mainWidth, 
                        (int)mainHeight, 
                        Color.Blue
                    );
                    
                    // Add white sparkle points for ice crystal effect
                    int sparkleRadius = Math.Max(1, (int)(Math.Min(width, height) * 0.08f));
                    Raylib.DrawCircle((int)(centerX - width * 0.4f), (int)centerY, sparkleRadius, Color.White); // Left
                    Raylib.DrawCircle((int)(centerX + width * 0.4f), (int)centerY, sparkleRadius, Color.White); // Right
                    Raylib.DrawCircle((int)centerX, (int)(centerY - height * 0.35f), sparkleRadius, Color.White); // Top
                    
                    break;
                    
                case ProjectileType.Wall:
                    // Brick-like pattern - keep original scaling
                    for (int x = 0; x < width; x += 8)
                    {
                        int brickWidth = Math.Min(6, width - x);
                        Raylib.DrawRectangle(x, 0, brickWidth, height, Color.Brown);
                        Raylib.DrawRectangleLines(x, 0, brickWidth, height, Color.Maroon);
                    }
                    break;
                    
                case ProjectileType.Explosive:
                    // Dangerous looking explosive - keep original scaling
                    Raylib.DrawCircle(width / 2, height / 2, Math.Min(width, height) / 2 - 1, Color.Orange);
                    Raylib.DrawCircle(width / 2, height / 2, Math.Min(width, height) / 3, Color.Red);
                    Raylib.DrawPixel(width / 2 - 1, height / 2 - 1, Color.Yellow);
                    Raylib.DrawPixel(width / 2 + 1, height / 2 + 1, Color.Yellow);
                    break;
            }
            
            Raylib.EndTextureMode();
            return renderTexture.Texture;
        }
        
        public static Texture2D GenerateMagnetShieldSprite(int width = 200, int height = 20)
        {
            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(width, height);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank); // Transparent background
            
            // --- MAIN SHIELD ARC ---
            for (int x = 0; x < width; x++)
            {
                float progress = (float)x / width;
                float curve = 1f - (progress - 0.5f) * (progress - 0.5f) * 4f;
                int baseY = (int)(curve * (height - 6));
                for (int thickness = 0; thickness < 6; thickness++)
                {
                    int y = baseY + thickness;
                    if (y < height)
                    {
                        Color shieldColor;
                        if (thickness <= 1)
                            shieldColor = new Color(255, 255, 255, 255);
                        else if (thickness <= 3)
                            shieldColor = new Color(0, 200, 255, 255);
                        else
                            shieldColor = new Color(0, 100, 200, 180);
                        Raylib.DrawPixel(x, y, shieldColor);
                    }
                }
            }
            // Edge pillars
            for (int edge = 0; edge < 4; edge++)
            {
                for (int y = 0; y < height; y++)
                {
                    float edgeIntensity = 1f - (float)y / height;
                    int alpha = (int)(200 * edgeIntensity);
                    Color leftEdge = new Color(0, 150, 255, alpha);
                    Raylib.DrawPixel(edge, y, leftEdge);
                    Raylib.DrawPixel(width - 1 - edge, y, leftEdge);
                }
            }
            // Energy nodes at quarter points
            for (int node = 1; node <= 3; node++)
            {
                int nodeX = (width / 4) * node;
                float nodeProgress = (float)nodeX / width;
                float nodeCurve = 1f - (nodeProgress - 0.5f) * (nodeProgress - 0.5f) * 4f;
                int nodeBaseY = (int)(nodeCurve * (height - 6));
                if (nodeBaseY + 1 < height)
                    Raylib.DrawPixel(nodeX, nodeBaseY + 1, new Color(255, 255, 255, 255));
            }
            Raylib.EndTextureMode();
            return renderTexture.Texture;
        }
    }
}