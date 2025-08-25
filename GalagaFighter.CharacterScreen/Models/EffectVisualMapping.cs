using GalagaFighter.CharacterScreen.Services;
using Raylib_cs;
using System.Collections.Generic;

namespace GalagaFighter.CharacterScreen.Models
{
    /// <summary>
    /// Maps ProjectileEffect class names to their corresponding visual effects
    /// </summary>
    public static class EffectVisualMapping
    {
        private static readonly Dictionary<string, ShipEffectType> _effectMappings = new()
        {
            { "GalagaFighter.Core.Models.Effects.IceShotEffect", ShipEffectType.Ice },
            { "GalagaFighter.Core.Models.Effects.ExplosiveShotEffect", ShipEffectType.Explosive },
            { "GalagaFighter.Core.Models.Effects.WoodShotEffect", ShipEffectType.Wood },
            { "GalagaFighter.Core.Models.Effects.NinjaShotEffect", ShipEffectType.Ninja },
            { "GalagaFighter.Core.Models.Effects.MudShotEffect", ShipEffectType.Mud },
            { "GalagaFighter.Core.Models.Effects.MagnetEffect", ShipEffectType.Magnet }
        };

        /// <summary>
        /// Gets the visual effect type for a ProjectileEffect class name
        /// </summary>
        public static ShipEffectType GetVisualEffect(string effectClassName)
        {
            return _effectMappings.TryGetValue(effectClassName, out var effect) ? effect : ShipEffectType.None;
        }

        /// <summary>
        /// Gets the visual effect type for an OffensiveEffect
        /// </summary>
        public static ShipEffectType GetVisualEffect(OffensiveEffect effect)
        {
            return GetVisualEffect(effect.EffectClassName);
        }

        /// <summary>
        /// Creates an enhanced ship sprite with both palette swapping and visual effects
        /// </summary>
        public static Texture2D CreateEnhancedShipSprite(Character character, OffensiveEffect? selectedEffect = null, int width = 80, int height = 120)
        {
            // Start with the character's base visual effect
            ShipEffectType visualEffect = character.VisualEffect;
            
            // If a ProjectileEffect is selected, use its visual effect instead
            if (selectedEffect != null)
            {
                var effectVisual = GetVisualEffect(selectedEffect);
                if (effectVisual != ShipEffectType.None)
                {
                    visualEffect = effectVisual;
                }
            }
            
            return ShipSpriteService.CreateShipSprite(
                character.ShipTintColor, 
                width, 
                height, 
                visualEffect);
        }

        /// <summary>
        /// Gets all available visual effect types
        /// </summary>
        public static ShipEffectType[] GetAllVisualEffects()
        {
            return new[]
            {
                ShipEffectType.None,
                ShipEffectType.Ice,
                ShipEffectType.Explosive,
                ShipEffectType.Wood,
                ShipEffectType.Ninja,
                ShipEffectType.Mud,
                ShipEffectType.Magnet
            };
        }

        /// <summary>
        /// Gets a description of what each visual effect looks like
        /// </summary>
        public static string GetEffectDescription(ShipEffectType effectType)
        {
            return effectType switch
            {
                ShipEffectType.Ice => "Ice crystals growing on wings, frost overlay",
                ShipEffectType.Explosive => "Flame trails, glowing weapon ports, explosive vents",
                ShipEffectType.Wood => "Wooden armor plating, reinforcement bands with rivets",
                ShipEffectType.Ninja => "Stealth shimmer, shadow effects, smoke trails",
                ShipEffectType.Mud => "Mud splatters, grime streaks, dirt accumulation", 
                ShipEffectType.Magnet => "Magnetic field lines, energy cores, pull indicators",
                ShipEffectType.None => "Clean ship appearance, no visual effects",
                _ => "Unknown effect type"
            };
        }
    }
}