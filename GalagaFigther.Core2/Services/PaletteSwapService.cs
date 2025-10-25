using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System;

namespace GalagaFighter.Core2.Services
{
    /// <summary>
    /// Service for creating palette-swapped textures with caching support
    /// </summary>
    public static class PaletteSwapService
    {
        /// <summary>
        /// Creates a palette-swapped texture from the source texture using the provided palette swap configuration.
        /// Results are automatically cached for performance.
        /// </summary>
        public static Texture2D CreatePaletteSwappedTexture(Texture2D sourceTexture, PaletteSwap paletteSwap)
        {
            // Create cache key that includes both source texture and palette swap info
            string key = $"{sourceTexture.Id}|{paletteSwap.ToCacheKey()}";
            
            // Check if we already have this palette-swapped texture cached
            if (TextureCache.TryGetValue(key, out Texture2D cachedTexture))
                return cachedTexture;

            // Get the image data from the texture
            Image sourceImage = Raylib.LoadImageFromTexture(sourceTexture);
            
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
                    
                    // Check if this pixel is in any of the palette swap ranges
                    bool isTargetPixel = false;
                    float bestBrightness = 1.0f;
                    
                    foreach (var range in paletteSwap.SourceRanges)
                    {
                        if (IsColorInRange(originalPixel, range.BaseColor, range.Tolerance))
                        {
                            isTargetPixel = true;
                            // Calculate the brightness of the original pixel
                            bestBrightness = GetPixelBrightness(originalPixel);
                            break;
                        }
                    }
                    
                    if (isTargetPixel)
                    {
                        // Replace with the new color, adjusting brightness to match original
                        newPixel = AdjustColorBrightness(paletteSwap.TargetColor, bestBrightness);
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
            
            // Cache the result for future use
            TextureCache.Set(key, newTexture);
            
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

            // General catch-all for dark reds: R > 80, R much greater than G/B
            // (This matches the original system's logic for red detection)
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
            return new Color(
                (byte)(baseColor.R * brightness),
                (byte)(baseColor.G * brightness),
                (byte)(baseColor.B * brightness),
                baseColor.A
            );
        }
    }
}