using Raylib_cs;

namespace GalagaFighter.Core2.Models
{
    /// <summary>
    /// Predefined color palettes for common ship variants
    /// </summary>
    public static class PlayerPalettes
    {
        private static Random _random = new Random();
        public static Color Random()
        {
            var index = _random.Next(0, All.Length);
            return GetByIndex(index);
        }

        // Ship Color Variants from CharacterScreen documentation
        public static readonly Color AzureWing = new(135, 206, 235, 255);    // Sky Blue
        public static readonly Color CrimsonHawk = new(220, 20, 20, 255);    // Red (original)
        public static readonly Color EmeraldDart = new(50, 205, 50, 255);    // Lime Green
        public static readonly Color VoidHunter = new(128, 0, 128, 255);     // Purple
        public static readonly Color SolarFlare = new(255, 165, 0, 255);     // Orange
        public static readonly Color IcePhantom = new(0, 255, 255, 255);     // Cyan
        public static readonly Color LightningStrike = new(255, 255, 0, 255); // Yellow
        public static readonly Color GhostShip = new(255, 255, 255, 255);    // White
        public static readonly Color RosePhantom = new(255, 192, 203, 255);  // Pink
        public static readonly Color ShadowReaper = new(64, 64, 64, 255);    // Dark Gray

        /// <summary>
        /// Gets all predefined ship palette colors
        /// </summary>
        public static Color[] All => new[]
        {
            AzureWing, CrimsonHawk, EmeraldDart, VoidHunter, SolarFlare,
            IcePhantom, LightningStrike, GhostShip, RosePhantom, ShadowReaper
        };

        /// <summary>
        /// Gets a palette color by index (wraps around if index is out of bounds)
        /// </summary>
        public static Color GetByIndex(int index)
        {
            var palettes = All;
            return palettes[index % palettes.Length];
        }
    }
}