using Raylib_cs;
using GalagaFighter.Core;
using System;
using System.Collections.Generic;
using System.Numerics;

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
                
                Console.WriteLine("?? Searching for MainShip.png...");
                Console.WriteLine($"Working Directory: {System.IO.Directory.GetCurrentDirectory()}");
                
                foreach (string path in possiblePaths)
                {
                    Console.WriteLine($"Trying: {path}");
                    if (System.IO.File.Exists(path))
                    {
                        Console.WriteLine($"? Found MainShip.png at: {path}");
                        _baseShipTexture = Raylib.LoadTexture(path);
                        
                        if (_baseShipTexture.Value.Id != 0)
                        {
                            Console.WriteLine($"? Successfully loaded MainShip.png ({_baseShipTexture.Value.Width}x{_baseShipTexture.Value.Height})");
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"? Failed to load texture from {path}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"? File not found: {path}");
                    }
                }
                
                if (_baseShipTexture == null || _baseShipTexture.Value.Id == 0)
                {
                    Console.WriteLine("?? MainShip.png not found, using fallback sprites");
                    Console.WriteLine("?? Fallback sprites will still show visual effects!");
                }
            }
            return _baseShipTexture ?? new Texture2D();
        }
        
        public static Texture2D CreateShipSprite(Color newRedColor, int width = 80, int height = 120, ShipEffectType effectType = ShipEffectType.None)
        {
            var baseTexture = GetBaseShipTexture();
            
            // If the base texture failed to load, create a fallback procedural sprite
            if (baseTexture.Id == 0)
            {
                return CreateFallbackShipSprite(newRedColor, width, height, effectType);
            }
            
            // Create palette-swapped texture
            var paletteSwappedTexture = CreatePaletteSwappedTexture(baseTexture, newRedColor);
            
            // Apply visual effects if specified
            Texture2D finalTexture;
            //if (effectType != ShipEffectType.None)
            //{
            //    finalTexture = ApplyVisualEffects(paletteSwappedTexture, effectType, width, height);
            //    Raylib.UnloadTexture(paletteSwappedTexture); // Clean up intermediate texture
            //}
            //else
            //{
                finalTexture = paletteSwappedTexture;
            //}
            
            // If we need to resize, do that now
            if (width != finalTexture.Width || height != finalTexture.Height)
            {
                var resizedTexture = ResizeTexture(finalTexture, width, height);
                if (finalTexture.Id != paletteSwappedTexture.Id) // Don't double-unload
                {
                    Raylib.UnloadTexture(finalTexture);
                }
                return resizedTexture;
            }
            
            return finalTexture;
        }
        
        private static Texture2D ApplyVisualEffects(Texture2D baseTexture, ShipEffectType effectType, int targetWidth, int targetHeight)
        {
            // Create a render texture for compositing effects
            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(baseTexture.Width, baseTexture.Height);
            
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            
            // Draw the base ship first
            Raylib.DrawTexture(baseTexture, 0, 0, Color.White);
            
            // Apply effect-specific visual enhancements
            switch (effectType)
            {
                case ShipEffectType.Ice:
                    DrawIceEffects(baseTexture.Width, baseTexture.Height);
                    break;
                case ShipEffectType.Explosive:
                    DrawExplosiveEffects(baseTexture.Width, baseTexture.Height);
                    break;
                case ShipEffectType.Wood:
                    DrawWoodEffects(baseTexture.Width, baseTexture.Height);
                    break;
                case ShipEffectType.Ninja:
                    DrawNinjaEffects(baseTexture.Width, baseTexture.Height);
                    break;
                case ShipEffectType.Mud:
                    DrawMudEffects(baseTexture.Width, baseTexture.Height);
                    break;
                case ShipEffectType.Magnet:
                    DrawMagnetEffects(baseTexture.Width, baseTexture.Height);
                    break;
            }
            
            Raylib.EndTextureMode();
            
            return renderTexture.Texture;
        }
        
        private static void DrawIceEffects(int width, int height)
        {
            // Draw realistic ice crystals with sharp, defined edges
            Color iceBlue = new Color(200, 230, 255, 255); // Solid ice blue
            Color iceDark = new Color(150, 200, 230, 255); // Darker ice
            Color iceLight = new Color(240, 250, 255, 255); // Bright ice highlights
            
            // Left wing ice formations - sharp crystalline structures
            DrawDetailedIceCrystal(width * 0.15f, height * 0.45f, 8, iceBlue, iceDark, iceLight);
            DrawDetailedIceCrystal(width * 0.05f, height * 0.55f, 6, iceBlue, iceDark, iceLight);
            DrawDetailedIceCrystal(width * 0.25f, height * 0.65f, 7, iceBlue, iceDark, iceLight);
            
            // Right wing ice formations
            DrawDetailedIceCrystal(width * 0.85f, height * 0.45f, 8, iceBlue, iceDark, iceLight);
            DrawDetailedIceCrystal(width * 0.95f, height * 0.55f, 6, iceBlue, iceDark, iceLight);
            DrawDetailedIceCrystal(width * 0.75f, height * 0.65f, 7, iceBlue, iceDark, iceLight);
            
            // Ice spikes along wing edges
            DrawIceSpikes(width * 0.1f, height * 0.4f, width * 0.3f, iceBlue, iceDark);
            DrawIceSpikes(width * 0.6f, height * 0.4f, width * 0.3f, iceBlue, iceDark);
        }
        
        private static void DrawDetailedIceCrystal(float x, float y, float size, Color baseColor, Color darkColor, Color lightColor)
        {
            Vector2 center = new Vector2(x, y);
            
            // Draw main crystal body as a diamond
            Vector2[] crystalBody = new Vector2[]
            {
                new Vector2(center.X, center.Y - size),           // Top
                new Vector2(center.X + size * 0.6f, center.Y),   // Right
                new Vector2(center.X, center.Y + size),           // Bottom
                new Vector2(center.X - size * 0.6f, center.Y)    // Left
            };
            
            // Fill the crystal body
            for (int i = 0; i < 4; i++)
            {
                int next = (i + 1) % 4;
                Raylib.DrawTriangle(center, crystalBody[i], crystalBody[next], baseColor);
            }
            
            // Add dark edges for definition
            for (int i = 0; i < 4; i++)
            {
                int next = (i + 1) % 4;
                Raylib.DrawLineEx(crystalBody[i], crystalBody[next], 1.5f, darkColor);
            }
            
            // Add bright highlight on top faces
            Raylib.DrawTriangle(center, crystalBody[0], crystalBody[1], lightColor);
            Raylib.DrawTriangle(center, crystalBody[3], crystalBody[0], lightColor);
            
            // Central bright core
            Raylib.DrawCircle((int)center.X, (int)center.Y, size * 0.25f, Color.White);
        }
        
        private static void DrawIceSpikes(float x, float y, float width, Color baseColor, Color darkColor)
        {
            // Draw a series of small ice spikes along the edge
            int spikeCount = (int)(width / 8);
            for (int i = 0; i < spikeCount; i++)
            {
                float spikeX = x + i * (width / spikeCount);
                float spikeHeight = 3 + (i % 3) * 2; // Varying heights
                
                Vector2[] spike = new Vector2[]
                {
                    new Vector2(spikeX, y),
                    new Vector2(spikeX + 3, y + spikeHeight),
                    new Vector2(spikeX - 3, y + spikeHeight)
                };
                
                Raylib.DrawTriangle(spike[0], spike[1], spike[2], baseColor);
                Raylib.DrawLineEx(spike[0], spike[1], 1f, darkColor);
                Raylib.DrawLineEx(spike[0], spike[2], 1f, darkColor);
            }
        }
        
        private static void DrawExplosiveEffects(int width, int height)
        {
            // Draw realistic flame effects with proper flame shapes
            Color flameCore = new Color(255, 255, 100, 255);   // Bright yellow core
            Color flameOrange = new Color(255, 160, 0, 255);   // Orange flames
            Color flameRed = new Color(255, 80, 0, 255);       // Red outer flames
            
            // Enhanced engine flames
            float engineY = height * 0.85f;
            float engineCenterX = width * 0.5f;
            
            // Draw multiple flame layers for realistic fire effect
            DrawRealisticFlame(engineCenterX, engineY, 12, flameRed, flameOrange, flameCore);
            
            // Side exhaust ports with smaller flames
            DrawRealisticFlame(width * 0.2f, height * 0.6f, 6, flameRed, flameOrange, flameCore);
            DrawRealisticFlame(width * 0.8f, height * 0.6f, 6, flameRed, flameOrange, flameCore);
            
            // Glowing weapon barrels
            DrawGlowingBarrel(width * 0.3f, height * 0.35f, 4, flameCore);
            DrawGlowingBarrel(width * 0.7f, height * 0.35f, 4, flameCore);
            
            // Heat distortion lines (simple representation)
            DrawHeatLines(engineCenterX, engineY, 8, new Color(255, 100, 0, 180));
        }
        
        private static void DrawRealisticFlame(float x, float y, float size, Color outerColor, Color midColor, Color coreColor)
        {
            // Draw flame in layers from outside to inside
            
            // Outer flame layer
            Vector2[] outerFlame = new Vector2[]
            {
                new Vector2(x, y),
                new Vector2(x - size * 0.6f, y + size * 1.5f),
                new Vector2(x - size * 0.3f, y + size * 2.2f),
                new Vector2(x, y + size * 2.5f),
                new Vector2(x + size * 0.3f, y + size * 2.2f),
                new Vector2(x + size * 0.6f, y + size * 1.5f)
            };
            
            for (int i = 0; i < outerFlame.Length - 1; i++)
            {
                Raylib.DrawTriangle(new Vector2(x, y), outerFlame[i], outerFlame[i + 1], outerColor);
            }
            
            // Middle flame layer (smaller)
            Vector2[] midFlame = new Vector2[]
            {
                new Vector2(x, y + size * 0.3f),
                new Vector2(x - size * 0.4f, y + size * 1.2f),
                new Vector2(x, y + size * 1.8f),
                new Vector2(x + size * 0.4f, y + size * 1.2f)
            };
            
            for (int i = 0; i < midFlame.Length - 1; i++)
            {
                Raylib.DrawTriangle(new Vector2(x, y + size * 0.3f), midFlame[i], midFlame[i + 1], midColor);
            }
            
            // Core flame (brightest, smallest)
            Raylib.DrawCircle((int)x, (int)(y + size * 0.8f), size * 0.3f, coreColor);
        }
        
        private static void DrawGlowingBarrel(float x, float y, float size, Color glowColor)
        {
            // Draw weapon barrel with glow effect
            Raylib.DrawCircle((int)x, (int)y, size + 2, new Color((int)glowColor.R, (int)glowColor.G, (int)glowColor.B, 100));
            Raylib.DrawCircle((int)x, (int)y, size, glowColor);
            Raylib.DrawCircle((int)x, (int)y, size * 0.6f, Color.White);
        }
        
        private static void DrawHeatLines(float x, float y, float length, Color color)
        {
            // Draw wavy heat distortion lines
            for (int i = 0; i < 5; i++)
            {
                float lineX = x + (i - 2) * 3;
                float waveOffset = (float)Math.Sin(i * 0.5f) * 2;
                Raylib.DrawLineEx(
                    new Vector2(lineX + waveOffset, y), 
                    new Vector2(lineX - waveOffset, y + length), 
                    1f, color);
            }
        }
        
        private static void DrawWoodEffects(int width, int height)
        {
            // Draw wooden armor plating and reinforcements
            Color woodBrown = new Color(139, 69, 19, 180);
            Color woodTan = new Color(210, 180, 140, 160);
            
            // Wooden armor plates on wings
            DrawWoodenPlate(width * 0.1f, height * 0.4f, width * 0.25f, height * 0.2f, woodBrown);
            DrawWoodenPlate(width * 0.65f, height * 0.4f, width * 0.25f, height * 0.2f, woodBrown);
            
            // Wood grain details
            DrawWoodGrain(width * 0.1f, height * 0.4f, width * 0.25f, height * 0.2f, woodTan);
            DrawWoodGrain(width * 0.65f, height * 0.4f, width * 0.25f, height * 0.2f, woodTan);
            
            // Reinforcement bands
            DrawReinforcementBand(width * 0.2f, height * 0.35f, width * 0.6f, height * 0.05f, new Color(101, 67, 33, 200));
            DrawReinforcementBand(width * 0.2f, height * 0.6f, width * 0.6f, height * 0.05f, new Color(101, 67, 33, 200));
        }
        
        private static void DrawWoodenPlate(float x, float y, float width, float height, Color color)
        {
            Rectangle plate = new Rectangle(x, y, width, height);
            Raylib.DrawRectangleRec(plate, color);
            Raylib.DrawRectangleLinesEx(plate, 1, new Color(101, 67, 33, 255));
        }
        
        private static void DrawWoodGrain(float x, float y, float width, float height, Color color)
        {
            // Draw wood grain lines
            for (int i = 0; i < 3; i++)
            {
                float lineY = y + height * 0.3f + i * height * 0.2f;
                Raylib.DrawLineEx(new Vector2(x + width * 0.1f, lineY), new Vector2(x + width * 0.9f, lineY), 1, color);
            }
        }
        
        private static void DrawReinforcementBand(float x, float y, float width, float height, Color color)
        {
            Rectangle band = new Rectangle(x, y, width, height);
            Raylib.DrawRectangleRec(band, color);
            
            // Add rivets
            for (int i = 0; i < 4; i++)
            {
                float rivetX = x + width * 0.2f + i * width * 0.2f;
                Raylib.DrawCircle((int)rivetX, (int)(y + height * 0.5f), 2, new Color(169, 169, 169, 255));
            }
        }
        
        private static void DrawNinjaEffects(int width, int height)
        {
            // Draw stealth/shadow effects
            Color shadowColor = new Color(75, 0, 130, 120); // Dark purple shadow
            Color stealthColor = new Color(138, 43, 226, 80); // Blue-violet stealth
            
            // Shadow outline effect
            DrawShadowOutline(width, height, shadowColor);
            
            // Stealth field shimmer
            DrawStealthShimmer(width * 0.2f, height * 0.3f, width * 0.6f, height * 0.4f, stealthColor);
            
            // Ninja smoke trails
            DrawSmokeTrail(width * 0.1f, height * 0.7f, 6, shadowColor);
            DrawSmokeTrail(width * 0.9f, height * 0.7f, 6, shadowColor);
        }
        
        private static void DrawShadowOutline(int width, int height, Color color)
        {
            // Draw a subtle shadow outline around the ship
            for (int offset = 1; offset <= 3; offset++)
            {
                Color fadeColor = new Color(color.R, color.G, color.B, (byte)(color.A / offset));
                // This would need the original ship outline data to work properly
                // For now, draw a simple glow effect
                Raylib.DrawCircle(width / 2, height / 2, width * 0.6f + offset * 2, fadeColor);
            }
        }
        
        private static void DrawStealthShimmer(float x, float y, float width, float height, Color color)
        {
            // Draw a shimmering stealth field
            Rectangle field = new Rectangle(x, y, width, height);
            Raylib.DrawRectangleRec(field, color);
            
            // Add shimmer lines
            Random rand = new Random(123);
            for (int i = 0; i < 8; i++)
            {
                float lineX = x + rand.NextSingle() * width;
                float lineY1 = y + rand.NextSingle() * height;
                float lineY2 = lineY1 + height * 0.2f;
                Raylib.DrawLineEx(new Vector2(lineX, lineY1), new Vector2(lineX, lineY2), 1, new Color(255, 255, 255, 100));
            }
        }
        
        private static void DrawSmokeTrail(float x, float y, float size, Color color)
        {
            // Draw smoke puffs
            for (int i = 0; i < 3; i++)
            {
                float puffY = y + i * size * 1.5f;
                float puffSize = size - i;
                Color puffColor = new Color(color.R, color.G, color.B, (byte)(color.A - i * 30));
                Raylib.DrawCircle((int)x, (int)puffY, puffSize, puffColor);
            }
        }
        
        private static void DrawMudEffects(int width, int height)
        {
            // Draw mud splatters and grime
            Color mudBrown = new Color(101, 67, 33, 160);
            Color grimeDark = new Color(85, 107, 47, 140);
            
            // Mud splatters on lower hull
            DrawMudSplatter(width * 0.2f, height * 0.7f, 12, mudBrown);
            DrawMudSplatter(width * 0.8f, height * 0.75f, 10, mudBrown);
            DrawMudSplatter(width * 0.5f, height * 0.8f, 8, mudBrown);
            
            // Grime streaks
            DrawGrimeStreak(width * 0.3f, height * 0.6f, width * 0.1f, height * 0.2f, grimeDark);
            DrawGrimeStreak(width * 0.7f, height * 0.65f, width * 0.08f, height * 0.15f, grimeDark);
        }
        
        private static void DrawMudSplatter(float x, float y, float size, Color color)
        {
            // Draw an irregular mud splatter
            Raylib.DrawCircle((int)x, (int)y, size, color);
            
            // Add irregular edges
            Random rand = new Random((int)(x + y));
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f * (float)Math.PI / 180f;
                float distance = size * (0.8f + rand.NextSingle() * 0.6f);
                float blobX = x + (float)Math.Cos(angle) * distance;
                float blobY = y + (float)Math.Sin(angle) * distance;
                Raylib.DrawCircle((int)blobX, (int)blobY, size * 0.4f, color);
            }
        }
        
        private static void DrawGrimeStreak(float x, float y, float width, float height, Color color)
        {
            // Draw a grime streak
            Rectangle streak = new Rectangle(x, y, width, height);
            Raylib.DrawRectangleRec(streak, color);
            
            // Add texture
            Random rand = new Random((int)(x * y));
            for (int i = 0; i < 5; i++)
            {
                float dotX = x + rand.NextSingle() * width;
                float dotY = y + rand.NextSingle() * height;
                Raylib.DrawCircle((int)dotX, (int)dotY, 1, new Color(139, 69, 19, 200));
            }
        }
        
        private static void DrawMagnetEffects(int width, int height)
        {
            // Draw magnetic field lines and energy effects
            Color magnetBlue = new Color(0, 191, 255, 120);
            Color energyWhite = new Color(255, 255, 255, 180);
            
            // Magnetic field lines around the ship
            DrawMagneticField(width / 2, height / 2, width * 0.7f, magnetBlue);
            
            // Energy cores on sides
            DrawEnergyCore(width * 0.2f, height * 0.4f, 8, energyWhite);
            DrawEnergyCore(width * 0.8f, height * 0.4f, 8, energyWhite);
            
            // Magnetic pull indicators
            DrawMagneticPull(width * 0.1f, height * 0.3f, 4, magnetBlue);
            DrawMagneticPull(width * 0.9f, height * 0.3f, 4, magnetBlue);
        }
        
        private static void DrawMagneticField(float centerX, float centerY, float radius, Color color)
        {
            // Draw circular magnetic field lines
            for (int i = 1; i <= 3; i++)
            {
                float fieldRadius = radius * i / 3f;
                Color fieldColor = new Color(color.R, color.G, color.B, (byte)(color.A / i));
                Raylib.DrawCircleLines((int)centerX, (int)centerY, fieldRadius, fieldColor);
            }
        }
        
        private static void DrawEnergyCore(float x, float y, float size, Color color)
        {
            // Draw a pulsing energy core
            Raylib.DrawCircle((int)x, (int)y, size, color);
            Raylib.DrawCircle((int)x, (int)y, size * 0.6f, new Color(0, 191, 255, 255));
            Raylib.DrawCircle((int)x, (int)y, size * 0.3f, Color.White);
        }
        
        private static void DrawMagneticPull(float x, float y, float size, Color color)
        {
            // Draw magnetic pull effect (small particles being drawn in)
            for (int i = 0; i < 3; i++)
            {
                float particleX = x + (i - 1) * size;
                Raylib.DrawCircle((int)particleX, (int)y, size * 0.3f, color);
            }
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
            
            // Draw the source texture
            Rectangle sourceRect = new Rectangle(0, 0, sourceTexture.Width, sourceTexture.Height);
            Rectangle destRect = new Rectangle(x, y, scaledWidth, scaledHeight);
            Vector2 origin = new Vector2(0, 0);
            
            Raylib.DrawTexturePro(sourceTexture, sourceRect, destRect, origin, 0f, Color.White);
            
            Raylib.EndTextureMode();
            
            return renderTexture.Texture;
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
                new ColorRange(new Color(200, 0, 0, 255), 25),     // Dark red
                new ColorRange(new Color(255, 50, 50, 255), 35),   // Light red
                new ColorRange(new Color(220, 20, 20, 255), 30),   // Medium red
                new ColorRange(new Color(180, 0, 0, 255), 20),     // Very dark red
                new ColorRange(new Color(255, 100, 100, 255), 40), // Pink-ish red
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
        
        private static bool IsColorInRange(Color pixel, Color targetColor, int tolerance)
        {
            // Skip transparent pixels
            if (pixel.A == 0) return false;
            
            int rDiff = Math.Abs(pixel.R - targetColor.R);
            int gDiff = Math.Abs(pixel.G - targetColor.G);
            int bDiff = Math.Abs(pixel.B - targetColor.B);
            
            // Use Euclidean distance for better color matching
            double distance = Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
            
            return distance <= tolerance;
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
        
        public static Texture2D CreateShipPortrait(Color newRedColor, int size = 120, ShipEffectType effectType = ShipEffectType.None)
        {
            return CreateShipSprite(newRedColor, size, size, effectType);
        }
        
        // Fallback procedural sprite generation if MainShip.png fails to load
        private static Texture2D CreateFallbackShipSprite(Color primaryColor, int width, int height, ShipEffectType effectType)
        {
            RenderTexture2D renderTexture = Raylib.LoadRenderTexture(width, height);
            
            Raylib.BeginTextureMode(renderTexture);
            Raylib.ClearBackground(Color.Blank);
            
            Console.WriteLine($"?? Creating fallback sprite with {effectType} effects");
            
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
                Console.WriteLine($"? Applying {effectType} effects to fallback sprite");
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
        
        public static void Cleanup()
        {
            if (_baseShipTexture != null && _baseShipTexture.Value.Id != 0)
            {
                Raylib.UnloadTexture(_baseShipTexture.Value);
                _baseShipTexture = null;
            }
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