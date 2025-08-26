using GalagaFighter.CharacterScreen.Models;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace GalagaFighter.CharacterScreen.Services
{
    public interface ICommandLineArgumentService
    {
        string FormatPlayerArgument(PlayerSelection playerSelection);
        string GetColorName(Color color);
    }

    public class CommandLineArgumentService : ICommandLineArgumentService
    {
        private readonly Dictionary<Color, string> _colorMapping;

        public CommandLineArgumentService()
        {
            _colorMapping = InitializeColorMapping();
        }

        public string FormatPlayerArgument(PlayerSelection playerSelection)
        {
            if (!playerSelection.IsComplete || playerSelection.SelectedCharacter == null || playerSelection.SelectedEffect == null)
            {
                throw new ArgumentException("Player selection is not complete");
            }

            var character = playerSelection.SelectedCharacter;
            var effect = playerSelection.SelectedEffect;
            var stats = character.Stats;

            var colorName = GetColorName(character.ShipTintColor);
            var augmentName = GetAugmentName(effect);

            // Format: <ColorName>,<Augment>,<Health>,<Speed>,<FireRate>,<Damage>,<Shield>
            return $"{colorName},{augmentName},{stats.Health.ToString("F1", CultureInfo.InvariantCulture)},{stats.Speed.ToString("F1", CultureInfo.InvariantCulture)},{stats.FireRate.ToString("F1", CultureInfo.InvariantCulture)},{stats.Damage.ToString("F1", CultureInfo.InvariantCulture)},{stats.Shield.ToString("F1", CultureInfo.InvariantCulture)}";
        }

        public string GetColorName(Color color)
        {
            // Try to find exact match first
            foreach (var kvp in _colorMapping)
            {
                if (ColorsEqual(kvp.Key, color))
                {
                    return kvp.Value;
                }
            }

            // If no exact match, return a formatted representation
            return $"RGB_{color.R}_{color.G}_{color.B}";
        }

        private string GetAugmentName(OffensiveEffect effect)
        {
            // Extract the simple name from the effect class name
            // e.g., "GalagaFighter.Core.Models.Effects.RicochetEffect" -> "Ricochet"
            var className = effect.EffectClassName;
            if (string.IsNullOrEmpty(className))
            {
                return effect.Name.Replace(" ", "");
            }

            var lastDotIndex = className.LastIndexOf('.');
            if (lastDotIndex >= 0 && lastDotIndex < className.Length - 1)
            {
                var simpleClassName = className.Substring(lastDotIndex + 1);
                // Remove "Effect" suffix if present
                if (simpleClassName.EndsWith("Effect"))
                {
                    simpleClassName = simpleClassName.Substring(0, simpleClassName.Length - 6);
                }
                return simpleClassName;
            }

            return effect.Name.Replace(" ", "");
        }

        private bool ColorsEqual(Color c1, Color c2)
        {
            return c1.R == c2.R && c1.G == c2.G && c1.B == c2.B && c1.A == c2.A;
        }

        private Dictionary<Color, string> InitializeColorMapping()
        {
            return new Dictionary<Color, string>
            {
                { Color.SkyBlue, "SkyBlue" },
                { Color.Red, "Red" },
                { Color.Lime, "Lime" },
                { Color.Purple, "Purple" },
                { Color.Orange, "Orange" },
                { new Color(0, 255, 255, 255), "Cyan" }, // Manual cyan definition
                { Color.Yellow, "Yellow" },
                { Color.White, "White" },
                { Color.Pink, "Pink" },
                { Color.DarkGray, "DarkGray" },
                { Color.Blue, "Blue" },
                { Color.Green, "Green" },
                { Color.Magenta, "Magenta" },
                { Color.Brown, "Brown" },
                { Color.Black, "Black" },
                { Color.Gray, "Gray" },
                { Color.LightGray, "LightGray" },
                { Color.Gold, "Gold" },
                { Color.Violet, "Violet" },
                { Color.Beige, "Beige" },
                { Color.Maroon, "Maroon" },
                { Color.RayWhite, "RayWhite" }
            };
        }
    }
}