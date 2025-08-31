using Raylib_cs;
using System;
using System.Numerics;
using System.Collections.Generic;
using GalagaFighter.Core.Services;

namespace GalagaFighter.Core.Static
{
    public static class SpriteGenerationService
    {
        public static Texture2D CreatePlayerShip(bool isPlayer1, int width = 20, int height = 40)
        {
            string key = $"PlayerShip_{isPlayer1}_{width}_{height}";
            if (TextureService.TryGetFromKey(key, out Texture2D texture))
                return texture;

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(width, height);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            Color primaryColor = isPlayer1 ? Color.SkyBlue : Color.Red;
            Color accentColor = isPlayer1 ? Color.Blue : Color.Maroon;
            Color engineColor = isPlayer1 ? Color.Yellow : Color.Orange;
            float scaleX = width / 20f;
            float scaleY = height / 40f;
            int bodyWidth = (int)(6 * scaleX);
            int bodyHeight = (int)(16 * scaleY);
            Raylib.DrawRectangle(width / 2 - bodyWidth / 2, height / 2 - bodyHeight / 2, bodyWidth, bodyHeight, primaryColor);
            float noseScale = Math.Min(scaleX, scaleY);
            Raylib.DrawTriangle(
                new Vector2(width / 2, 2 * scaleY),
                new Vector2(width / 2 - 4 * noseScale, height / 2 - bodyHeight / 2),
                new Vector2(width / 2 + 4 * noseScale, height / 2 - bodyHeight / 2),
                accentColor
            );
            int wingWidth = (int)((width - 8) * scaleX);
            int wingHeight = (int)(6 * scaleY);
            Raylib.DrawRectangle((int)(4 * scaleX), height / 2 - wingHeight / 2, wingWidth, wingHeight, accentColor);
            int engineWidth = (int)(4 * scaleX);
            int engineHeight = (int)(6 * scaleY);
            int exhaustWidth = (int)(2 * scaleX);
            int exhaustHeight = (int)(4 * scaleY);
            Raylib.DrawRectangle(width / 2 - engineWidth / 2, height / 2 + bodyHeight / 2, engineWidth, engineHeight, engineColor);
            Raylib.DrawRectangle(width / 2 - exhaustWidth / 2, height / 2 + bodyHeight / 2 + engineHeight, exhaustWidth, exhaustHeight, Color.White);
            int lineLength = (int)(12 * scaleY);
            Raylib.DrawLine(width / 2, height / 2 - lineLength / 2, width / 2, height / 2 + lineLength / 2, Color.White);
            Raylib.EndTextureMode();
            TextureService.Set(key, renderTexture.Texture);
            return renderTexture.Texture;
        }
        public static Texture2D CreateProjectileSprite(ProjectileType type, int width = 10, int height = 5, Color? color = null)
        {
            string colorKey = color.HasValue ? $"_{color.Value.R},{color.Value.G},{color.Value.B},{color.Value.A}" : "";
            string key = $"Projectile_{type}_{width}_{height}{colorKey}";
            if (TextureService.TryGetFromKey(key, out var texture))
                return texture;

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(width, height);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            switch (type)
            {
                case ProjectileType.Normal:
                    float scaleX = width / 10f;
                    float scaleY = height / 5f;
                    int bulletHeight = (int)(2 * scaleY);
                    int tipRadius = (int)(1 * Math.Min(scaleX, scaleY));
                    Raylib.DrawRectangle(0, height / 2 - bulletHeight / 2, width, bulletHeight, color ?? Color.Yellow);
                    Raylib.DrawCircle(width - tipRadius, height / 2, tipRadius, Color.White);
                    break;
                case ProjectileType.Ice:
                    float centerX = width / 2f;
                    float centerY = height / 2f;
                    float mainWidth = width * 0.7f;
                    float mainHeight = height * 0.5f;
                    Raylib.DrawRectangle(
                        (int)(centerX - mainWidth / 2),
                        (int)(centerY - mainHeight / 2),
                        (int)mainWidth,
                        (int)mainHeight,
                        Color.SkyBlue
                    );
                    float pointWidth = width * 0.4f;
                    float pointHeight = height * 0.3f;
                    Raylib.DrawRectangle(
                        (int)(centerX - pointWidth / 2),
                        (int)(centerY - height * 0.4f),
                        (int)pointWidth,
                        (int)pointHeight,
                        Color.SkyBlue
                    );
                    Raylib.DrawRectangle(
                        (int)(centerX - pointWidth / 2),
                        (int)(centerY + height * 0.2f),
                        (int)pointWidth,
                        (int)pointHeight,
                        Color.SkyBlue
                    );
                    float sideWidth = width * 0.25f;
                    float sideHeight = height * 0.3f;
                    Raylib.DrawRectangle(
                        (int)(centerX - width * 0.45f),
                        (int)(centerY - sideHeight / 2),
                        (int)sideWidth,
                        (int)sideHeight,
                        Color.SkyBlue
                    );
                    Raylib.DrawRectangle(
                        (int)(centerX + width * 0.25f),
                        (int)(centerY - sideHeight / 2),
                        (int)sideWidth,
                        (int)sideHeight,
                        Color.SkyBlue
                    );
                    Raylib.DrawRectangleLines(
                        (int)(centerX - mainWidth / 2),
                        (int)(centerY - mainHeight / 2),
                        (int)mainWidth,
                        (int)mainHeight,
                        Color.Blue
                    );
                    int sparkleRadius = Math.Max(1, (int)(Math.Min(width, height) * 0.08f));
                    Raylib.DrawCircle((int)(centerX - width * 0.4f), (int)centerY, sparkleRadius, Color.White);
                    Raylib.DrawCircle((int)(centerX + width * 0.4f), (int)centerY, sparkleRadius, Color.White);
                    Raylib.DrawCircle((int)centerX, (int)(centerY - height * 0.35f), sparkleRadius, Color.White);
                    break;
                case ProjectileType.Wall:
                    for (int x = 0; x < width; x += 8)
                    {
                        int brickWidth = Math.Min(6, width - x);
                        Raylib.DrawRectangle(x, 0, brickWidth, height, Color.Brown);
                        Raylib.DrawRectangleLines(x, 0, brickWidth, height, Color.Maroon);
                    }
                    break;
                case ProjectileType.Explosive:
                    Raylib.DrawCircle(width / 2, height / 2, Math.Min(width, height) / 2 - 1, Color.Orange);
                    Raylib.DrawCircle(width / 2, height / 2, Math.Min(width, height) / 3, Color.Red);
                    Raylib.DrawPixel(width / 2 - 1, height / 2 - 1, Color.Yellow);
                    Raylib.DrawPixel(width / 2 + 1, height / 2 + 1, Color.Yellow);
                    break;
            }
            Raylib.EndTextureMode();
            TextureService.Set(key, renderTexture.Texture);
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
                    Color leftEdge = new(0, 150, 255, alpha);
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

        public static SpriteWrapper CreateAnimatedMagnetShieldSprite(int frameCount = 24, float frameDuration = 0.05f, int width = 200, int height = 20)
        {
            return new SpriteWrapper(
                (position, rotation, drawWidth, drawHeight, scale, frame) =>
                {
                    float pulse = 1f + 0.2f * (float)Math.Sin((frame / (float)frameCount) * Math.PI * 2);
                    int arcHeight = (int)(height * scale * pulse);
                    int arcWidth = (int)(width * scale);
                    int yOffset = (int)(drawHeight / 2 - arcHeight / 2);
                    int xOffset = (int)(drawWidth / 2 - arcWidth / 2);
                    float radians = rotation * (float)Math.PI / 180f;
                    float centerX = position.X;
                    float centerY = position.Y;
                    for (int x = 0; x < arcWidth; x++)
                    {
                        float progress = (float)x / arcWidth;
                        float curve = ((progress - 0.5f) * (progress - 0.5f) * 4f) - 1f;
                        int baseY = (int)(curve * (arcHeight - 10));
                        for (int thickness = 0; thickness < 10; thickness++) // Thicker lines
                        {
                            int y = baseY + thickness + yOffset;
                            int drawX = x + xOffset;
                            float localX = drawX - drawWidth / 2f;
                            float localY = y - drawHeight / 2f;
                            float rotatedX = localX * (float)Math.Cos(radians) - localY * (float)Math.Sin(radians);
                            float rotatedY = localX * (float)Math.Sin(radians) + localY * (float)Math.Cos(radians);
                            int finalX = (int)(centerX + rotatedX);
                            int finalY = (int)(centerY + rotatedY);
                            if (finalX >= 0 && finalX < Raylib.GetScreenWidth() && finalY >= 0 && finalY < Raylib.GetScreenHeight())
                            {
                                Color shieldColor;
                                if (thickness <= 2)
                                    shieldColor = new Color(220, 40, 40, 255); // Less saturated red
                                else if (thickness <= 6)
                                    shieldColor = new Color(120, 30, 60, 200); // Muted dark red
                                else
                                    shieldColor = new Color(120, 180, 255, 230); // Silvery blue
                                Raylib.DrawPixel(finalX, finalY, shieldColor);
                            }
                        }
                    }
                },
                frameCount,
                frameDuration
            );
        }

        #region Particle Sprites

        /// <summary>
        /// Creates a simple circular particle sprite
        /// </summary>
        public static SpriteWrapper CreateCircleParticleSprite(int radius = 3, Color? color = null)
        {
            string key = $"CircleParticle_{radius}_{(color?.R ?? 255)},{(color?.G ?? 255)},{(color?.B ?? 255)},{(color?.A ?? 255)}";
            if (TextureService.TryGetFromKey(key, out Texture2D texture))
                return new SpriteWrapper(texture);

            int size = radius * 2 + 2;
            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(size, size);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            
            Raylib.DrawCircle(size / 2, size / 2, radius, color ?? Color.White);
            
            Raylib.EndTextureMode();
            TextureService.Set(key, renderTexture.Texture);
            return new SpriteWrapper(renderTexture.Texture);
        }

        /// <summary>
        /// Creates a star-shaped particle sprite
        /// </summary>
        public static SpriteWrapper CreateStarParticleSprite(int size = 6, Color? color = null)
        {
            string key = $"StarParticle_{size}_{(color?.R ?? 255)},{(color?.G ?? 255)},{(color?.B ?? 255)},{(color?.A ?? 255)}";
            if (TextureService.TryGetFromKey(key, out Texture2D texture))
                return new SpriteWrapper(texture);

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(size, size);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            
            int centerX = size / 2;
            int centerY = size / 2;
            Color drawColor = color ?? Color.White;
            
            // Draw a simple star shape
            Raylib.DrawLine(centerX, 0, centerX, size - 1, drawColor);
            Raylib.DrawLine(0, centerY, size - 1, centerY, drawColor);
            Raylib.DrawLine(1, 1, size - 2, size - 2, drawColor);
            Raylib.DrawLine(size - 2, 1, 1, size - 2, drawColor);
            
            Raylib.EndTextureMode();
            TextureService.Set(key, renderTexture.Texture);
            return new SpriteWrapper(renderTexture.Texture);
        }

        /// <summary>
        /// Creates a square particle sprite
        /// </summary>
        public static SpriteWrapper CreateSquareParticleSprite(int size = 4, Color? color = null)
        {
            string key = $"SquareParticle_{size}_{(color?.R ?? 255)},{(color?.G ?? 255)},{(color?.B ?? 255)},{(color?.A ?? 255)}";
            if (TextureService.TryGetFromKey(key, out Texture2D texture))
                return new SpriteWrapper(texture);

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(size, size);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            
            Raylib.DrawRectangle(0, 0, size, size, color ?? Color.White);
            
            Raylib.EndTextureMode();
            TextureService.Set(key, renderTexture.Texture);
            return new SpriteWrapper(renderTexture.Texture);
        }

        /// <summary>
        /// Creates a spark particle sprite (elongated diamond)
        /// </summary>
        public static SpriteWrapper CreateSparkParticleSprite(int width = 8, int height = 3, Color? color = null)
        {
            string key = $"SparkParticle_{width}_{height}_{(color?.R ?? 255)},{(color?.G ?? 255)},{(color?.B ?? 255)},{(color?.A ?? 255)}";
            if (TextureService.TryGetFromKey(key, out Texture2D texture))
                return new SpriteWrapper(texture);

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(width, height);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            
            int centerX = width / 2;
            int centerY = height / 2;
            Color drawColor = color ?? Color.Yellow;
            
            // Draw a diamond/spark shape
            Raylib.DrawTriangle(
                new Vector2(0, centerY),
                new Vector2(centerX, 0),
                new Vector2(centerX, height - 1),
                drawColor
            );
            Raylib.DrawTriangle(
                new Vector2(width - 1, centerY),
                new Vector2(centerX, 0),
                new Vector2(centerX, height - 1),
                drawColor
            );
            
            Raylib.EndTextureMode();
            TextureService.Set(key, renderTexture.Texture);
            return new SpriteWrapper(renderTexture.Texture);
        }

        /// <summary>
        /// Creates a smoke particle sprite (soft circle with gradient)
        /// </summary>
        public static SpriteWrapper CreateSmokeParticleSprite(int radius = 8, Color? color = null)
        {
            string key = $"SmokeParticle_{radius}_{(color?.R ?? 100)},{(color?.G ?? 100)},{(color?.B ?? 100)},{(color?.A ?? 150)}";
            if (TextureService.TryGetFromKey(key, out Texture2D texture))
                return new SpriteWrapper(texture);

            int size = radius * 2 + 2;
            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(size, size);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            
            int centerX = size / 2;
            int centerY = size / 2;
            Color baseColor = color ?? new Color(100, 100, 100, 150);
            
            // Draw concentric circles to create a gradient effect
            for (int r = radius; r > 0; r--)
            {
                int alpha = (int)(baseColor.A * (1f - (float)r / radius) * 0.8f);
                Color circleColor = new Color(baseColor.R, baseColor.G, baseColor.B, alpha);
                Raylib.DrawCircle(centerX, centerY, r, circleColor);
            }
            
            Raylib.EndTextureMode();
            TextureService.Set(key, renderTexture.Texture);
            return new SpriteWrapper(renderTexture.Texture);
        }

        /// <summary>
        /// Creates an ice crystal particle sprite
        /// </summary>
        public static SpriteWrapper CreateIceCrystalParticleSprite(int size = 6)
        {
            string key = $"IceCrystalParticle_{size}";
            if (TextureService.TryGetFromKey(key, out Texture2D texture))
                return new SpriteWrapper(texture);

            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(size, size);
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            
            int centerX = size / 2;
            int centerY = size / 2;
            
            // Draw ice crystal shape
            Raylib.DrawPixel(centerX, centerY, Color.White);
            Raylib.DrawPixel(centerX - 1, centerY, Color.SkyBlue);
            Raylib.DrawPixel(centerX + 1, centerY, Color.SkyBlue);
            Raylib.DrawPixel(centerX, centerY - 1, Color.SkyBlue);
            Raylib.DrawPixel(centerX, centerY + 1, Color.SkyBlue);
            
            if (size > 4)
            {
                Raylib.DrawPixel(centerX - 1, centerY - 1, Color.Blue);
                Raylib.DrawPixel(centerX + 1, centerY - 1, Color.Blue);
                Raylib.DrawPixel(centerX - 1, centerY + 1, Color.Blue);
                Raylib.DrawPixel(centerX + 1, centerY + 1, Color.Blue);
            }
            
            Raylib.EndTextureMode();
            TextureService.Set(key, renderTexture.Texture);
            return new SpriteWrapper(renderTexture.Texture);
        }

        #endregion

        #region Dot-based Particle Sprites

        /// <summary>
        /// Creates a dot-based particle sprite using pre-made feathered dot images
        /// </summary>
        /// <param name="intensity">1-5, corresponds to dot_1.png through dot_5.png</param>
        /// <param name="color">Optional color tint to apply to the dot</param>
        public static SpriteWrapper CreateDotParticleSprite(int intensity = 3, Color? color = null)
        {
            // Clamp intensity to valid range
            intensity = Math.Clamp(intensity, 1, 5);
            
            string spritePath = $"Sprites/Particles/dot_{intensity}.png";
            
            if (color.HasValue)
            {
                // Use palette swap to apply color tint
                return new SpriteWrapper(spritePath, color.Value);
            }
            else
            {
                // Use original white dot
                return new SpriteWrapper(spritePath);
            }
        }

        /// <summary>
        /// Creates a soft particle sprite (dot_1 or dot_2) - good for smoke, gentle effects
        /// </summary>
        public static SpriteWrapper CreateSoftParticleSprite(Color? color = null)
        {
            return CreateDotParticleSprite(Game.Random.Next(1, 3), color); // dot_1 or dot_2
        }

        /// <summary>
        /// Creates a medium particle sprite (dot_3) - good for general purpose particles
        /// </summary>
        public static SpriteWrapper CreateMediumParticleSprite(Color? color = null)
        {
            return CreateDotParticleSprite(3, color); // dot_3
        }

        /// <summary>
        /// Creates a hard particle sprite (dot_4 or dot_5) - good for sparks, explosions
        /// </summary>
        public static SpriteWrapper CreateHardParticleSprite(Color? color = null)
        {
            return CreateDotParticleSprite(Game.Random.Next(4, 6), color); // dot_4 or dot_5
        }

        /// <summary>
        /// Creates a random intensity dot particle sprite - good for varied effects
        /// </summary>
        public static SpriteWrapper CreateRandomDotParticleSprite(Color? color = null)
        {
            return CreateDotParticleSprite(Game.Random.Next(1, 6), color); // any dot_1 through dot_5
        }

        /// <summary>
        /// Creates fire-colored dot particles with random intensity for realistic fire
        /// </summary>
        public static SpriteWrapper CreateFireDotParticleSprite()
        {
            // Fire colors: orange to yellow to white (hottest)
            Color[] fireColors = {
                Color.Orange,
                new Color(255, 140, 0, 255),  // Dark orange
                Color.Yellow,
                new Color(255, 255, 100, 255), // Light yellow
                new Color(255, 255, 200, 255)  // Near white (hottest)
            };
            
            Color fireColor = fireColors[Game.Random.Next(fireColors.Length)];
            int intensity = Game.Random.Next(2, 5); // Medium to hard for fire
            
            return CreateDotParticleSprite(intensity, fireColor);
        }

        /// <summary>
        /// Creates smoke-colored dot particles with soft edges
        /// </summary>
        public static SpriteWrapper CreateSmokeDotParticleSprite()
        {
            // Smoke colors: gray variations
            Color[] smokeColors = {
                new Color(100, 100, 100, 180),
                new Color(120, 120, 120, 160),
                new Color(80, 80, 80, 200),
                new Color(140, 140, 140, 140)
            };
            
            Color smokeColor = smokeColors[Game.Random.Next(smokeColors.Length)];
            return CreateDotParticleSprite(Game.Random.Next(1, 3), smokeColor); // Soft dots for smoke
        }

        #endregion
    }
}