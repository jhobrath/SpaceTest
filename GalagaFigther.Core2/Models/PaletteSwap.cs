using Raylib_cs;
using System;
using System.Collections.Generic;

namespace GalagaFighter.Core2.Models
{
    /// <summary>
    /// Defines a palette swap configuration with source color ranges and target color
    /// </summary>
    public class PaletteSwap : IEquatable<PaletteSwap>
    {
        /// <summary>
        /// The color ranges to replace
        /// </summary>
        public IReadOnlyList<ColorRange> SourceRanges { get; }

        /// <summary>
        /// The color to replace the source ranges with
        /// </summary>
        public Color TargetColor { get; }

        public PaletteSwap(Color targetColor, IEnumerable<ColorRange> sourceRanges)
        {
            TargetColor = targetColor;
            SourceRanges = new List<ColorRange>(sourceRanges);
        }

        /// <summary>
        /// Creates a palette swap from one color to another, generating appropriate color ranges
        /// </summary>
        public static PaletteSwap CreateSwap(Color fromColor, Color toColor)
        {
            var colorRanges = new List<ColorRange>();

            // Create the base color range
            colorRanges.Add(new ColorRange(fromColor, 30));

            // Generate darker variations of the source color
            for (float brightness = 0.9f; brightness >= 0.4f; brightness -= 0.1f)
            {
                var darkerColor = new Color(
                    (byte)(fromColor.R * brightness),
                    (byte)(fromColor.G * brightness),
                    (byte)(fromColor.B * brightness),
                    fromColor.A
                );
                
                int tolerance = brightness > 0.7f ? 30 : brightness > 0.5f ? 25 : 20;
                colorRanges.Add(new ColorRange(darkerColor, tolerance));
            }

            // Generate lighter variations of the source color
            for (float lightness = 1.1f; lightness <= 1.4f; lightness += 0.1f)
            {
                var lighterColor = new Color(
                    (byte)Math.Min(255, fromColor.R * lightness),
                    (byte)Math.Min(255, fromColor.G * lightness),
                    (byte)Math.Min(255, fromColor.B * lightness),
                    fromColor.A
                );
                
                int tolerance = lightness < 1.3f ? 35 : 40;
                colorRanges.Add(new ColorRange(lighterColor, tolerance));
            }

            return new PaletteSwap(toColor, colorRanges);
        }

        public bool Equals(PaletteSwap? other)
        {
            if (other == null) return false;
            if (!TargetColor.Equals(other.TargetColor)) return false;
            if (SourceRanges.Count != other.SourceRanges.Count) return false;

            for (int i = 0; i < SourceRanges.Count; i++)
            {
                if (!SourceRanges[i].Equals(other.SourceRanges[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object? obj) => Equals(obj as PaletteSwap);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(TargetColor.R);
            hash.Add(TargetColor.G);
            hash.Add(TargetColor.B);
            hash.Add(TargetColor.A);
            
            foreach (var range in SourceRanges)
            {
                hash.Add(range);
            }
            
            return hash.ToHashCode();
        }

        /// <summary>
        /// Generates a cache key string for this palette swap
        /// </summary>
        public string ToCacheKey()
        {
            return $"PS_{TargetColor.R},{TargetColor.G},{TargetColor.B},{TargetColor.A}_{GetHashCode()}";
        }
    }

    /// <summary>
    /// Defines a color range for palette swapping
    /// </summary>
    public struct ColorRange : IEquatable<ColorRange>
    {
        public Color BaseColor { get; }
        public int Tolerance { get; }

        public ColorRange(Color baseColor, int tolerance)
        {
            BaseColor = baseColor;
            Tolerance = tolerance;
        }

        public bool Equals(ColorRange other)
        {
            return BaseColor.Equals(other.BaseColor) && Tolerance == other.Tolerance;
        }

        public override bool Equals(object? obj) => obj is ColorRange other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(BaseColor.R, BaseColor.G, BaseColor.B, BaseColor.A, Tolerance);
    }
}