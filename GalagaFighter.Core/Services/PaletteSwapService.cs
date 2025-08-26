using Raylib_cs;
using System;

namespace GalagaFighter.Core.Services
{
    public static class PaletteSwapService
    {
        public static Texture2D CreatePaletteSwappedTexture(Texture2D sourceTexture, Color newRedColor)
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
            
            // Process each pixel
            for (int y = 0; y < sourceImage.Height; y++)
            {
                for (int x = 0; x < sourceImage.Width; x++)
                {
                    Color originalPixel = Raylib.GetImageColor(sourceImage, x, y);
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
                    
                    Raylib.ImageDrawPixel(ref newImage, x, y, newPixel);
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
            return new Color(
                (byte)(baseColor.R * brightness),
                (byte)(baseColor.G * brightness),
                (byte)(baseColor.B * brightness),
                baseColor.A
            );
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