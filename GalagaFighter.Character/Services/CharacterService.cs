using GalagaFighter.CharacterScreen.Models;
using System.Collections.Generic;
using Raylib_cs;

namespace GalagaFighter.CharacterScreen.Services
{
    public interface ICharacterService
    {
        List<Character> GetAvailableCharacters();
        Character? GetCharacterById(string id);
    }

    public class CharacterService : ICharacterService
    {
        private readonly List<Character> _characters;

        public CharacterService()
        {
            _characters = InitializeShipVariants();
        }

        public List<Character> GetAvailableCharacters()
        {
            return _characters;
        }

        public Character? GetCharacterById(string id)
        {
            return _characters.Find(c => c.Id == id);
        }

        private List<Character> InitializeShipVariants()
        {
            return new List<Character>
            {
                new Character
                {
                    Id = "ship_blue",
                    Name = "Azure Wing",
                    Description = "Classic blue fighter with balanced performance",
                    SpritePath = "ship_blue",
                    PortraitPath = "ship_blue_portrait",
                    Type = CharacterType.Balanced,
                    Stats = new CharacterStats
                    {
                        Health = 100f,
                        Speed = 1.0f,
                        FireRate = 1.0f,
                        Damage = 1.0f,
                        Shield = 1.0f
                    },
                    StartingEffects = new List<string>(),
                    ShipTintColor = Color.SkyBlue,
                    VisualEffect = ShipEffectType.Magnet // Add magnetic effects!
                },
                new Character
                {
                    Id = "ship_red",
                    Name = "Crimson Hawk",
                    Description = "Aggressive red fighter with enhanced firepower",
                    SpritePath = "ship_red",
                    PortraitPath = "ship_red_portrait",
                    Type = CharacterType.Power,
                    Stats = new CharacterStats
                    {
                        Health = 90f,
                        Speed = 1.0f,
                        FireRate = 1.2f,
                        Damage = 1.1f,
                        Shield = 0.9f
                    },
                    StartingEffects = new List<string> { "FireBoost" },
                    ShipTintColor = Color.Red
                },
                new Character
                {
                    Id = "ship_green",
                    Name = "Emerald Dart",
                    Description = "Swift green fighter built for speed",
                    SpritePath = "ship_green",
                    PortraitPath = "ship_green_portrait",
                    Type = CharacterType.Speed,
                    Stats = new CharacterStats
                    {
                        Health = 85f,
                        Speed = 1.3f,
                        FireRate = 1.0f,
                        Damage = 0.9f,
                        Shield = 0.9f
                    },
                    StartingEffects = new List<string> { "SpeedBoost" },
                    ShipTintColor = Color.Lime,
                    VisualEffect = ShipEffectType.Wood // Add wooden armor effects!
                },
                new Character
                {
                    Id = "ship_purple",
                    Name = "Void Hunter",
                    Description = "Mysterious purple fighter with stealth capabilities",
                    SpritePath = "ship_purple",
                    PortraitPath = "ship_purple_portrait",
                    Type = CharacterType.Special,
                    Stats = new CharacterStats
                    {
                        Health = 95f,
                        Speed = 1.1f,
                        FireRate = 0.9f,
                        Damage = 1.0f,
                        Shield = 1.1f
                    },
                    StartingEffects = new List<string> { "Stealth" },
                    ShipTintColor = Color.Purple
                },
                new Character
                {
                    Id = "ship_orange",
                    Name = "Solar Flare",
                    Description = "Bright orange fighter powered by solar energy",
                    SpritePath = "ship_orange",
                    PortraitPath = "ship_orange_portrait",
                    Type = CharacterType.Power,
                    Stats = new CharacterStats
                    {
                        Health = 105f,
                        Speed = 0.9f,
                        FireRate = 1.1f,
                        Damage = 1.2f,
                        Shield = 1.0f
                    },
                    StartingEffects = new List<string> { "SolarCharge" },
                    ShipTintColor = Color.Orange,
                    VisualEffect = ShipEffectType.Explosive // Flame trails and explosive effects!
                },
                new Character
                {
                    Id = "ship_cyan",
                    Name = "Ice Phantom",
                    Description = "Cool cyan fighter with ice-based weapons",
                    SpritePath = "ship_cyan",
                    PortraitPath = "ship_cyan_portrait",
                    Type = CharacterType.Special,
                    Stats = new CharacterStats
                    {
                        Health = 100f,
                        Speed = 1.0f,
                        FireRate = 0.8f,
                        Damage = 1.0f,
                        Shield = 1.2f
                    },
                    StartingEffects = new List<string> { "IceShots" },
                    ShipTintColor = new Color(0, 255, 255, 255), // Cyan color
                    VisualEffect = ShipEffectType.Ice // Ice crystals and frost effects!
                },
                new Character
                {
                    Id = "ship_yellow",
                    Name = "Lightning Strike",
                    Description = "Electric yellow fighter with rapid-fire capability",
                    SpritePath = "ship_yellow",
                    PortraitPath = "ship_yellow_portrait",
                    Type = CharacterType.Speed,
                    Stats = new CharacterStats
                    {
                        Health = 80f,
                        Speed = 1.2f,
                        FireRate = 1.4f,
                        Damage = 0.8f,
                        Shield = 0.8f
                    },
                    StartingEffects = new List<string> { "RapidFire" },
                    ShipTintColor = Color.Yellow,
                    VisualEffect = ShipEffectType.Mud // Add battle damage effects!
                },
                new Character
                {
                    Id = "ship_white",
                    Name = "Ghost Ship",
                    Description = "Pristine white fighter with defensive capabilities",
                    SpritePath = "ship_white",
                    PortraitPath = "ship_white_portrait",
                    Type = CharacterType.Defense,
                    Stats = new CharacterStats
                    {
                        Health = 120f,
                        Speed = 0.8f,
                        FireRate = 0.9f,
                        Damage = 0.9f,
                        Shield = 1.3f
                    },
                    StartingEffects = new List<string> { "Shield" },
                    ShipTintColor = Color.White
                },
                new Character
                {
                    Id = "ship_pink",
                    Name = "Rose Phantom",
                    Description = "Elegant pink fighter with precision targeting",
                    SpritePath = "ship_pink",
                    PortraitPath = "ship_pink_portrait",
                    Type = CharacterType.Special,
                    Stats = new CharacterStats
                    {
                        Health = 95f,
                        Speed = 1.1f,
                        FireRate = 1.0f,
                        Damage = 1.1f,
                        Shield = 1.0f
                    },
                    StartingEffects = new List<string> { "Precision" },
                    ShipTintColor = Color.Pink
                },
                new Character
                {
                    Id = "ship_dark",
                    Name = "Shadow Reaper",
                    Description = "Dark fighter with overwhelming destructive power",
                    SpritePath = "ship_dark",
                    PortraitPath = "ship_dark_portrait",
                    Type = CharacterType.Power,
                    Stats = new CharacterStats
                    {
                        Health = 85f,
                        Speed = 0.9f,
                        FireRate = 0.8f,
                        Damage = 1.4f,
                        Shield = 0.8f
                    },
                    StartingEffects = new List<string> { "DarkPower" },
                    ShipTintColor = Color.DarkGray,
                    VisualEffect = ShipEffectType.Ninja // Stealth effects and shadow trails!
                }
            };
        }
    }
}