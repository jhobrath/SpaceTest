using System.Collections.Generic;
using Raylib_cs;
using GalagaFighter.CharacterScreen.Services;

namespace GalagaFighter.CharacterScreen.Models
{
    public class Character
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SpritePath { get; set; } = string.Empty;
        public string PortraitPath { get; set; } = string.Empty;
        public CharacterStats Stats { get; set; } = new CharacterStats();
        public List<string> StartingEffects { get; set; } = new List<string>();
        public CharacterType Type { get; set; } = CharacterType.Balanced;
        
        // Ship color for palette swapping the red parts of MainShip.png
        public Color ShipTintColor { get; set; } = Color.White;
        
        // Visual effect overlay for ProjectileEffects
        public ShipEffectType VisualEffect { get; set; } = ShipEffectType.None;
        
        // Alternative property name for clarity - maps to ShipTintColor
        public Color RedReplacementColor 
        { 
            get => ShipTintColor; 
            set => ShipTintColor = value; 
        }
    }

    public enum CharacterType
    {
        Speed,
        Power,
        Defense,
        Balanced,
        Special
    }

    public class CharacterStats
    {
        public float Health { get; set; } = 100f;
        public float Speed { get; set; } = 1.0f;
        public float FireRate { get; set; } = 1.0f;
        public float Damage { get; set; } = 1.0f;
        public float Shield { get; set; } = 1.0f;
    }
}