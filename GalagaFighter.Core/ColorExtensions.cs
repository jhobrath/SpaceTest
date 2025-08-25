using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core
{
    public static class ColorExtensions
    {
        public static Color ApplyBlue(this Color color, float blueAlpha)
        {
            var newColor = new Color(
                (byte)(color.R * (1 - blueAlpha)),
                (byte)(color.G * (1 - blueAlpha)),
                (byte)(color.B + (255 - color.B) * blueAlpha),
                color.A);

            return newColor;
        }

        public static Color ApplyGreen(this Color color, float greenAlpha)
        {
            var newColor = new Color(
                (byte)(color.R * (1 - greenAlpha)),
                (byte)(color.G + (255 - color.G) * greenAlpha),
                (byte)(color.B * (1 - greenAlpha)),
                color.A);
            return newColor;

        }

        public static Color ApplyRed(this Color color, float redAlpha)
        {
            var newColor = new Color(
                (byte)(color.R + (255 - color.R) * redAlpha),
                (byte)(color.G * (1 - redAlpha)),
                (byte)(color.B * (1 - redAlpha)),
                color.A);
            return newColor;
        }

        public static Color ApplyAlpha(this Color color, float alpha)
        {
            var newColor = new Color(color.R/255f, color.G/255f, color.B/255f, alpha);
            return newColor;
        }

        /// <summary>
        /// Shifts the hue of a Raylib Color by the specified amount (in degrees)
        /// </summary>
        /// <param name="color">The original color</param>
        /// <param name="hueShift">Hue shift in degrees (-360 to 360)</param>
        /// <returns>Color with shifted hue</returns>
        public static Color ShiftHue(this Color color, float hueShift)
        {
            // Convert RGB to HSV
            RgbToHsv(color.R, color.G, color.B, out float h, out float s, out float v);

            // Shift hue and wrap around
            h = (h + hueShift) % 360f;
            if (h < 0) h += 360f;

            // Convert back to RGB
            HsvToRgb(h, s, v, out byte r, out byte g, out byte b);

            return new Color(r, g, b, color.A); // Preserve alpha
        }

        /// <summary>
        /// Adjusts the lightness (brightness/value) of a color by a percentage
        /// </summary>
        /// <param name="color">The original color</param>
        /// <param name="lightnessFactor">Lightness multiplier (0.0 = black, 1.0 = no change, 2.0 = twice as bright)</param>
        /// <returns>Color with adjusted lightness</returns>
        public static Color AdjustLightness(this Color color, float lightnessFactor)
        {
            // Convert RGB to HSV
            RgbToHsv(color.R, color.G, color.B, out float h, out float s, out float v);

            // Adjust lightness (value in HSV) and clamp
            v = Math.Clamp(v * lightnessFactor, 0f, 1f);

            // Convert back to RGB
            HsvToRgb(h, s, v, out byte r, out byte g, out byte b);

            return new Color(r, g, b, color.A); // Preserve alpha
        }

        /// <summary>
        /// Sets the lightness (brightness/value) of a color to a specific value
        /// </summary>
        /// <param name="color">The original color</param>
        /// <param name="lightness">Lightness value (0.0 = black, 1.0 = full brightness)</param>
        /// <returns>Color with set lightness</returns>
        public static Color SetLightness(this Color color, float lightness)
        {
            // Convert RGB to HSV
            RgbToHsv(color.R, color.G, color.B, out float h, out float s, out float v);

            // Set lightness (value in HSV) and clamp
            v = Math.Clamp(lightness, 0f, 1f);

            // Convert back to RGB
            HsvToRgb(h, s, v, out byte r, out byte g, out byte b);

            return new Color(r, g, b, color.A); // Preserve alpha
        }

        /// <summary>
        /// Makes a color lighter by adding to its lightness value
        /// </summary>
        /// <param name="color">The original color</param>
        /// <param name="amount">Amount to lighten (0.0 = no change, 1.0 = maximum lightening)</param>
        /// <returns>Lighter color</returns>
        public static Color Lighten(this Color color, float amount)
        {
            return color.AdjustLightness(1f + amount);
        }

        /// <summary>
        /// Makes a color darker by reducing its lightness value
        /// </summary>
        /// <param name="color">The original color</param>
        /// <param name="amount">Amount to darken (0.0 = no change, 1.0 = black)</param>
        /// <returns>Darker color</returns>
        public static Color Darken(this Color color, float amount)
        {
            return color.AdjustLightness(1f - amount);
        }

        /// <summary>
        /// Convert RGB to HSV
        /// </summary>
        public static void RgbToHsv(byte r, byte g, byte b, out float h, out float s, out float v)
        {
            float rf = r / 255f;
            float gf = g / 255f;
            float bf = b / 255f;

            float max = Math.Max(rf, Math.Max(gf, bf));
            float min = Math.Min(rf, Math.Min(gf, bf));
            float delta = max - min;

            // Hue
            if (delta == 0)
                h = 0;
            else if (max == rf)
                h = ((gf - bf) / delta) % 6;
            else if (max == gf)
                h = (bf - rf) / delta + 2;
            else
                h = (rf - gf) / delta + 4;

            h *= 60;
            if (h < 0) h += 360;

            // Saturation
            s = max == 0 ? 0 : delta / max;

            // Value
            v = max;
        }

        /// <summary>
        /// Convert HSV to RGB
        /// </summary>
        public static void HsvToRgb(float h, float s, float v, out byte r, out byte g, out byte b)
        {
            float c = v * s;
            float x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            float m = v - c;

            float rf, gf, bf;

            if (h < 60)
            {
                rf = c; gf = x; bf = 0;
            }
            else if (h < 120)
            {
                rf = x; gf = c; bf = 0;
            }
            else if (h < 180)
            {
                rf = 0; gf = c; bf = x;
            }
            else if (h < 240)
            {
                rf = 0; gf = x; bf = c;
            }
            else if (h < 300)
            {
                rf = x; gf = 0; bf = c;
            }
            else
            {
                rf = c; gf = 0; bf = x;
            }

            r = (byte)((rf + m) * 255);
            g = (byte)((gf + m) * 255);
            b = (byte)((bf + m) * 255);
        }

        /// <summary>
        /// Creates a colored tint from a hue value for texture rendering
        /// </summary>
        /// <param name="hue">Hue in degrees (0-360)</param>
        /// <param name="saturation">Color saturation (0.0 = white/no tint, 1.0 = full color)</param>
        /// <param name="brightness">Brightness/value (0.0 = black, 1.0 = full brightness)</param>
        /// <param name="alpha">Alpha transparency (0-255)</param>
        /// <returns>Color tint for texture rendering</returns>
        public static Color FromHue(float hue, float saturation = 1.0f, float brightness = 1.0f, byte alpha = 255)
        {
            // Normalize hue to 0-360 range
            hue = hue % 360f;
            if (hue < 0) hue += 360f;

            // Clamp saturation and brightness
            saturation = Math.Clamp(saturation, 0f, 1f);
            brightness = Math.Clamp(brightness, 0f, 1f);

            // Convert HSV to RGB
            HsvToRgb(hue, saturation, brightness, out byte r, out byte g, out byte b);

            return new Color(r, g, b, alpha);
        }

        /// <summary>
        /// Creates a hue-shifted tint color for texture rendering (starts from a saturated base color)
        /// </summary>
        /// <param name="color">Base color (if white, will use red as starting hue)</param>
        /// <param name="hueShift">Hue shift in degrees</param>
        /// <param name="saturation">Override saturation (null = keep original, 0.0-1.0 = set specific)</param>
        /// <returns>Tinted color for texture rendering</returns>
        public static Color ShiftHueForTexture(this Color color, float hueShift, float? saturation = null)
        {
            // Convert RGB to HSV
            RgbToHsv(color.R, color.G, color.B, out float h, out float s, out float v);

            // If the color is grayscale (white/gray/black), start with red hue
            if (s < 0.01f) // Very low saturation means grayscale
            {
                h = 0f; // Start with red
                s = saturation ?? 1.0f; // Use full saturation unless specified
            }
            else
            {
                s = saturation ?? s; // Keep original or override
            }

            // Shift hue and wrap around
            h = (h + hueShift) % 360f;
            if (h < 0) h += 360f;

            // Convert back to RGB
            HsvToRgb(h, s, v, out byte r, out byte g, out byte b);

            return new Color(r, g, b, color.A);
        }
    }
}
