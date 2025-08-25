using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using GalagaFighter.CharacterScreen.Utilities;

namespace GalagaFighter.CharacterScreen.Services
{
    public enum ShipEffectType
    {
        None,
        Ice,
        Explosive,
        Wood,
        Ninja,
        Mud,
        Magnet
    }

    public static class ShipSpriteService
    {
        private static Texture2D? _baseShipTexture = null;
        private static readonly Dictionary<(Color color, int size, ShipEffectType effect), Texture2D> _shipTextureCache = new();
        
        private static Texture2D GetBaseShipTexture()
        {
            if (_baseShipTexture == null || _baseShipTexture.Value.Id == 0)
            {
                // Try multiple possible paths for MainShip.png
                string[] possiblePaths = {
                    "Sprites/Ships/MainShip.png",
                    "./Sprites/Ships/MainShip.png",
                    "bin/Debug/net8.0/Sprites/Ships/MainShip.png",
                    "../GalagaFighter.Core/Sprites/Ships/MainShip.png"
                };
                
                Console.WriteLine("🔍 Searching for MainShip.png...");
                Console.WriteLine($"Working Directory: {System.IO.Directory.GetCurrentDirectory()}");
                
                foreach (string path in possiblePaths)
                {
                    Console.WriteLine($"Trying: {path}");
                    if (System.IO.File.Exists(path))
                    {
                        Console.WriteLine($"✅ Found MainShip.png at: {path}");
                        _baseShipTexture = Raylib.LoadTexture(path);
                        
                        if (_baseShipTexture.Value.Id != 0)
                        {
                            // Apply high-quality filtering to the base texture
                            Raylib.SetTextureFilter(_baseShipTexture.Value, TextureFilter.Bilinear);
                            Console.WriteLine($"✅ Successfully loaded MainShip.png ({_baseShipTexture.Value.Width}x{_baseShipTexture.Value.Height}) with filtering");
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"❌ Failed to load texture from {path}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"❌ File not found: {path}");
                    }
                }
                
                if (_baseShipTexture == null || _baseShipTexture.Value.Id == 0)
                {
                    Console.WriteLine("⚠️ MainShip.png not found, using fallback sprites");
                    Console.WriteLine("🎨 Fallback sprites will still show visual effects!");
                }
            }
            return _baseShipTexture ?? new Texture2D();
        }

        // Restored working version with texture filtering
        public static Texture2D GetShipTexture(Color newRedColor, int targetSize, ShipEffectType effectType = ShipEffectType.None)
        {
            // Create cache key
            var cacheKey = (newRedColor, targetSize, effectType);
            
            // Check if we already have this exact texture cached
            if (_shipTextureCache.TryGetValue(cacheKey, out var cachedTexture))
            {
                return cachedTexture;
            }
            
            // Create the ship sprite using the original method but with filtering
            var baseTexture = GetBaseShipTexture();
            
            Texture2D finalTexture;
            if (baseTexture.Id == 0)
            {
                // Use fallback sprite
                finalTexture = CreateFallbackShipSprite(newRedColor, targetSize, targetSize, effectType);
            }
            else
            {
                // Create palette-swapped texture
                var paletteSwappedTexture = CreatePaletteSwappedTexture(baseTexture, newRedColor);
                
                // Apply texture filtering for better quality
                Raylib.SetTextureFilter(paletteSwappedTexture, TextureFilter.Bilinear);
                
                // Resize if needed
                if (targetSize != paletteSwappedTexture.Width || targetSize != paletteSwappedTexture.Height)
                {
                    finalTexture = ResizeTexture(paletteSwappedTexture, targetSize, targetSize);
                    Raylib.SetTextureFilter(finalTexture, TextureFilter.Bilinear);
                    Raylib.UnloadTexture(paletteSwappedTexture);
                }
                else
                {
                    finalTexture = paletteSwappedTexture;
                }
            }
            
            // Cache it for future use
            _shipTextureCache[cacheKey] = finalTexture;
            
            Console.WriteLine($"🎨 Created and cached ship texture: {newRedColor} at {targetSize}px with {effectType}");
            
            return finalTexture;
        }
        
        private static Texture2D ResizeTexture(Texture2D sourceTexture, int width, int height)
        {
            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(width, height);
            
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            
            // Calculate scaling to fit while maintaining aspect ratio
            float scaleX = (float)width / sourceTexture.Width;
            float scaleY = (float)height / sourceTexture.Height;
            float scale = Math.Min(scaleX, scaleY);
            
            // Calculate centered position
            int scaledWidth = (int)(sourceTexture.Width * scale);
            int scaledHeight = (int)(sourceTexture.Height * scale);
            int x = (width - scaledWidth) / 2;
            int y = (height - scaledHeight) / 2;
            
            // Draw the source texture with high quality
            Rectangle sourceRect = new Rectangle(0, 0, sourceTexture.Width, sourceTexture.Height);
            Rectangle destRect = new Rectangle(x, y, scaledWidth, scaledHeight);
            Vector2 origin = new Vector2(0, 0);
            
            Raylib.DrawTexturePro(sourceTexture, sourceRect, destRect, origin, 0f, Color.White);
            
            Raylib.EndTextureMode();
            
            return renderTexture.Texture;
        }
        
        private static Texture2D CreateShipSpriteAtSize(Color newRedColor, int targetSize, ShipEffectType effectType)
        {
            var baseTexture = GetBaseShipTexture();
            
            // If the base texture failed to load, create a fallback procedural sprite
            if (baseTexture.Id == 0)
            {
                return CreateFallbackShipSprite(newRedColor, targetSize, targetSize, effectType);
            }
            
            // Create high-quality render texture at exact target size
            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(targetSize, targetSize);
            
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            
            // First, create palette-swapped texture at original size for quality
            var paletteSwappedTexture = CreatePaletteSwappedTexture(baseTexture, newRedColor);
            
            // Calculate scaling to fit target size while maintaining aspect ratio
            float scale = (float)targetSize / Math.Max(paletteSwappedTexture.Width, paletteSwappedTexture.Height);
            
            // Calculate centered position
            int scaledWidth = (int)(paletteSwappedTexture.Width * scale);
            int scaledHeight = (int)(paletteSwappedTexture.Height * scale);
            int x = (targetSize - scaledWidth) / 2;
            int y = (targetSize - scaledHeight) / 2;
            
            // Draw with high quality scaling
            Rectangle sourceRect = new Rectangle(0, 0, paletteSwappedTexture.Width, paletteSwappedTexture.Height);
            Rectangle destRect = new Rectangle(x, y, scaledWidth, scaledHeight);
            
            Raylib.DrawTexturePro(paletteSwappedTexture, sourceRect, destRect, Vector2.Zero, 0f, Color.White);
            
            // Clean up intermediate texture
            Raylib.UnloadTexture(paletteSwappedTexture);
            
            Raylib.EndTextureMode();
            
            // Apply high-quality filtering to final texture
            Raylib.SetTextureFilter(renderTexture.Texture, TextureFilter.Bilinear);
            
            return renderTexture.Texture;
        }
        
        // Legacy method - now uses the cached approach
        public static Texture2D CreateShipSprite(Color newRedColor, int width = 80, int height = 120, ShipEffectType effectType = ShipEffectType.None)
        {
            // Use the larger dimension as target size for square texture
            int targetSize = Math.Max(width, height);
            return GetShipTexture(newRedColor, targetSize, effectType);
        }
        
        public static Texture2D CreateShipPortrait(Color newRedColor, int size = 120, ShipEffectType effectType = ShipEffectType.None)
        {
            return GetShipTexture(newRedColor, size, effectType);
        }

        private static Texture2D CreatePaletteSwappedTexture(Texture2D sourceTexture, Color newRedColor)
        {
            // Get the image data from the texture
            Image sourceImage = Raylib.LoadImageFromTexture(sourceTexture);
            
            // Define the color ranges we want to replace
            // These are the red color ranges in the original image
            var redColorRanges = new[]
            {
                // Pure red and variations
                new ColorRange(new Color(255, 0, 0, 255), 30),     // Pure red with tolerance
                new ColorRange(new Color(200, 0, 0, 255), 30),     // Dark red (increased tolerance)
                new ColorRange(new Color(255, 50, 50, 255), 35),   // Light red
                new ColorRange(new Color(220, 20, 20, 255), 30),   // Medium red
                new ColorRange(new Color(180, 0, 0, 255), 25),     // Very dark red (increased tolerance)
                new ColorRange(new Color(255, 100, 100, 255), 40), // Pink-ish red
                new ColorRange(new Color(150, 0, 0, 255), 25),     // Even darker red
                new ColorRange(new Color(120, 0, 0, 255), 25),     // Very dark red
                new ColorRange(new Color(100, 0, 0, 255), 20),     // Deepest red
            };
            
            // Create a new image with the same dimensions
            Image newImage = Raylib.GenImageColor(sourceImage.Width, sourceImage.Height, Color.Blank);
            
            unsafe
            {
                Color* sourcePixels = (Color*)sourceImage.Data;
                Color* newPixels = (Color*)newImage.Data;
                
                int pixelCount = sourceImage.Width * sourceImage.Height;
                
                for (int i = 0; i < pixelCount; i++)
                {
                    Color originalPixel = sourcePixels[i];
                    Color newPixel = originalPixel;
                    
                    // Check if this pixel is in any of our red color ranges
                    bool isRedPixel = false;
                    float bestBrightness = 1.0f;
                    
                    foreach (var range in redColorRanges)
                    {
                        if (IsColorInRange(originalPixel, range.BaseColor, range.Tolerance))
                        {
                            isRedPixel = true;
                            // Calculate the brightness of the original red pixel
                            bestBrightness = GetPixelBrightness(originalPixel);
                            break;
                        }
                    }
                    
                    if (isRedPixel)
                    {
                        // Replace with the new color, adjusting brightness to match original
                        newPixel = AdjustColorBrightness(newRedColor, bestBrightness);
                        newPixel.A = originalPixel.A; // Preserve alpha
                    }
                    
                    newPixels[i] = newPixel;
                }
            }
            
            // Convert the new image to a texture
            Texture2D newTexture = Raylib.LoadTextureFromImage(newImage);
            
            // Clean up the images
            Raylib.UnloadImage(sourceImage);
            Raylib.UnloadImage(newImage);
            
            return newTexture;
        }
        
        // Fallback procedural sprite generation if MainShip.png fails to load
        private static Texture2D CreateFallbackShipSprite(Color primaryColor, int width, int height, ShipEffectType effectType)
        {
            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(width, height);
            
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            
            Console.WriteLine($"🎨 Creating fallback sprite with {effectType} effects");
            
            // Simple fallback ship design
            float scaleX = width / 80f;
            float scaleY = height / 120f;
            
            // Draw ship body
            int bodyWidth = (int)(24 * scaleX);
            int bodyHeight = (int)(60 * scaleY);
            int bodyX = width / 2 - bodyWidth / 2;
            int bodyY = height / 2 - bodyHeight / 2;
            Raylib.DrawRectangle(bodyX, bodyY, bodyWidth, bodyHeight, primaryColor);
            
            // Draw nose
            float noseScale = Math.Min(scaleX, scaleY);
            int noseY = (int)(10 * scaleY);
            Raylib.DrawTriangle(
                new Vector2(width / 2, noseY),
                new Vector2(width / 2 - 12 * noseScale, bodyY),
                new Vector2(width / 2 + 12 * noseScale, bodyY),
                primaryColor
            );
            
            // Draw wings
            int wingWidth = (int)(50 * scaleX);
            int wingHeight = (int)(20 * scaleY);
            int wingY = height / 2 - wingHeight / 2;
            int wingX = (width - wingWidth) / 2;
            Raylib.DrawRectangle(wingX, wingY, wingWidth, wingHeight, primaryColor);
            
            // Apply DRAMATIC effects to fallback sprite
            if (effectType != ShipEffectType.None)
            {
                Console.WriteLine($"✨ Applying {effectType} effects to fallback sprite");
                switch (effectType)
                {
                    case ShipEffectType.Ice:
                        // Large ice crystals on wings
                        for (int i = 0; i < 4; i++)
                        {
                            float x = wingX + i * wingWidth / 3f;
                            float y = wingY + wingHeight / 2f;
                            DrawLargeIceCrystal(x, y, 8 * scaleX, new Color(173, 216, 230, 200));
                        }
                        // Frost overlay
                        Raylib.DrawRectangle(wingX, wingY, wingWidth, wingHeight, new Color(255, 255, 255, 80));
                        break;
                        
                    case ShipEffectType.Explosive:
                        // Large flame effects
                        float engineY = bodyY + bodyHeight;
                        for (int i = 0; i < 3; i++)
                        {
                            float flameY = engineY + i * 8 * scaleY;
                            Color flameColor = i == 0 ? Color.Red : i == 1 ? Color.Orange : Color.Yellow;
                            Raylib.DrawCircle(width / 2, (int)flameY, (int)(12 * scaleX - i * 3), flameColor);
                        }
                        // Glowing weapon ports
                        Raylib.DrawCircle(bodyX + bodyWidth / 4, bodyY + bodyHeight / 3, (int)(6 * scaleX), Color.Yellow);
                        Raylib.DrawCircle(bodyX + 3 * bodyWidth / 4, bodyY + bodyHeight / 3, (int)(6 * scaleX), Color.Yellow);
                        break;
                        
                    case ShipEffectType.Wood:
                        // Wooden armor plating
                        Color woodColor = new Color(139, 69, 19, 180);
                        Raylib.DrawRectangle(wingX - 5, wingY - 5, wingWidth + 10, wingHeight + 10, woodColor);
                        // Rivets
                        for (int i = 0; i < 6; i++)
                        {
                            float rivetX = wingX + i * wingWidth / 5f;
                            Raylib.DrawCircle((int)rivetX, wingY + wingHeight / 2, 2, Color.Gray);
                        }
                        break;
                        
                    case ShipEffectType.Ninja:
                        // Stealth shimmer effect
                        Color stealthColor = new Color(138, 43, 226, 100);
                        Raylib.DrawRectangle(bodyX - 10, bodyY - 10, bodyWidth + 20, bodyHeight + 20, stealthColor);
                        // Smoke trails
                        for (int i = 0; i < 3; i++)
                        {
                            float smokeY = bodyY + bodyHeight + i * 10 * scaleY;
                            Raylib.DrawCircle(bodyX + bodyWidth / 4, (int)smokeY, (int)(8 * scaleX - i * 2), new Color(75, 0, 130, 150 - i * 40));
                            Raylib.DrawCircle(bodyX + 3 * bodyWidth / 4, (int)smokeY, (int)(8 * scaleX - i * 2), new Color(75, 0, 130, 150 - i * 40));
                        }
                        break;
                        
                    case ShipEffectType.Mud:
                        // Mud splatters
                        Color mudColor = new Color(101, 67, 33, 180);
                        for (int i = 0; i < 5; i++)
                        {
                            float mudX = bodyX + (i * bodyWidth / 4f);
                            float mudY = bodyY + bodyHeight * 0.7f;
                            Raylib.DrawCircle((int)mudX, (int)mudY, (int)(6 * scaleX), mudColor);
                        }
                        // Grime streaks
                        Raylib.DrawRectangle(bodyX, bodyY + bodyHeight / 2, bodyWidth, bodyHeight / 4, new Color(85, 107, 47, 120));
                        break;
                        
                    case ShipEffectType.Magnet:
                        // Magnetic field lines
                        Color magnetColor = new Color(0, 191, 255, 150);
                        for (int ring = 1; ring <= 3; ring++)
                        {
                            float radius = ring * 15 * scaleX;
                            Raylib.DrawCircleLines(width / 2, height / 2, radius, magnetColor);
                        }
                        // Energy cores
                        Raylib.DrawCircle(bodyX + bodyWidth / 4, bodyY + bodyHeight / 2, (int)(8 * scaleX), Color.Blue);
                        Raylib.DrawCircle(bodyX + 3 * bodyWidth / 4, bodyY + bodyHeight / 2, (int)(8 * scaleX), Color.Blue);
                        break;
                }
            }
            
            Raylib.EndTextureMode();
            
            // Apply filtering to fallback texture too
            Raylib.SetTextureFilter(renderTexture.Texture, TextureFilter.Bilinear);
            
            return renderTexture.Texture;
        }
        
        private static void DrawLargeIceCrystal(float x, float y, float size, Color color)
        {
            // Draw a prominent six-pointed ice crystal
            Vector2 center = new Vector2(x, y);
            
            Vector2[] points = new Vector2[6];
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f * (float)Math.PI / 180f;
                points[i] = new Vector2(
                    center.X + (float)Math.Cos(angle) * size,
                    center.Y + (float)Math.Sin(angle) * size
                );
            }
            
            // Draw crystal with thick lines
            for (int i = 0; i < 6; i++)
            {
                int next = (i + 1) % 6;
                Raylib.DrawLineEx(points[i], points[next], 3f, color);
            }
            
            // Draw bright center
            Raylib.DrawCircle((int)center.X, (int)center.Y, size * 0.4f, new Color(255, 255, 255, 180));
        }
        
        private static bool IsColorInRange(Color pixel, Color targetColor, int tolerance)
        {
            // Skip transparent pixels
            if (pixel.A == 0) return false;

            int rDiff = Math.Abs(pixel.R - targetColor.R);
            int gDiff = Math.Abs(pixel.G - targetColor.G);
            int bDiff = Math.Abs(pixel.B - targetColor.B);

            // Use Euclidean distance for better color matching
            double distance = Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);

            // General catch-all for dark reds: R > 80, R much greater than G/B
            bool isGeneralDarkRed = pixel.R > 80 && pixel.R > pixel.G + 40 && pixel.R > pixel.B + 40;

            return distance <= tolerance || isGeneralDarkRed;
        }
        
        private static float GetPixelBrightness(Color pixel)
        {
            // Convert to HSV to get the brightness (value) component
            float r = pixel.R / 255f;
            float g = pixel.G / 255f;
            float b = pixel.B / 255f;
            
            return Math.Max(r, Math.Max(g, b)); // V component of HSV
        }
        
        private static Color AdjustColorBrightness(Color baseColor, float brightness)
        {
            // Apply brightness adjustment while preserving hue and saturation
            return baseColor.AdjustLightness(brightness);
        }
        
        public static void ClearCache()
        {
            // Clean up all cached textures
            foreach (var texture in _shipTextureCache.Values)
            {
                Raylib.UnloadTexture(texture);
            }
            _shipTextureCache.Clear();
            Console.WriteLine("🧹 Cleared ship texture cache");
        }
        
        public static void Cleanup()
        {
            if (_baseShipTexture != null && _baseShipTexture.Value.Id != 0)
            {
                Raylib.UnloadTexture(_baseShipTexture.Value);
                _baseShipTexture = null;
            }
            
            ClearCache();
        }
        
        // Helper struct for defining color ranges
        private struct ColorRange
        {
            public Color BaseColor { get; }
            public int Tolerance { get; }
            
            public ColorRange(Color baseColor, int tolerance)
            {
                BaseColor = baseColor;
                Tolerance = tolerance;
            }
        }
    }
}