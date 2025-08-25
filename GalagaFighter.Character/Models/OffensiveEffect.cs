using System.Collections.Generic;

namespace GalagaFighter.CharacterScreen.Models
{
    public class OffensiveEffect
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public string EffectClassName { get; set; } = string.Empty; // Full class name for instantiation
        public EffectCategory Category { get; set; } = EffectCategory.Offensive;
    }

    public enum EffectCategory
    {
        Offensive,
        Defensive,
        Utility,
        Special
    }

    public class PlayerSelection
    {
        public Character? SelectedCharacter { get; set; }
        public OffensiveEffect? SelectedEffect { get; set; }
        public bool CharacterReady { get; set; }
        public bool EffectReady { get; set; }
        public bool IsComplete => CharacterReady && EffectReady && SelectedCharacter != null && SelectedEffect != null;
    }
}