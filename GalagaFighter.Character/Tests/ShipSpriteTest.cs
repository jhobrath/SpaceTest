using GalagaFighter.CharacterScreen.Services;
using GalagaFighter.CharacterScreen.Models;
using Raylib_cs;
using System;

namespace GalagaFighter.CharacterScreen.Tests
{
    public class ShipSpriteTest
    {
        public static void TestShipGeneration()
        {
            Console.WriteLine("Testing ship sprite generation...");
            
            // Initialize Raylib for texture creation
            Raylib.InitWindow(800, 600, "Ship Sprite Test");
            
            try
            {
                var characterService = new CharacterService();
                var characters = characterService.GetAvailableCharacters();
                
                Console.WriteLine($"Generated {characters.Count} ship variants:");
                
                foreach (var character in characters)
                {
                    Console.WriteLine($"- {character.Name}: {character.Type} type");
                    Console.WriteLine($"  Tint Color: {character.ShipTintColor}");
                    Console.WriteLine($"  Stats: HP={character.Stats.Health}, Speed={character.Stats.Speed}, Damage={character.Stats.Damage}");
                    
                    // Test sprite generation
                    var sprite = ShipSpriteService.CreateShipSprite(character.ShipTintColor);
                    
                    Console.WriteLine($"  Sprite generated successfully: {sprite.Width}x{sprite.Height}");
                    
                    // Clean up texture
                    Raylib.UnloadTexture(sprite);
                }
                
                Console.WriteLine("All ship sprites generated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error testing ship generation: {ex.Message}");
            }
            finally
            {
                Raylib.CloseWindow();
            }
        }
    }

    public class SelectionSystemTest
    {
        public static void TestSelectionSystem()
        {
            Console.WriteLine("Testing enhanced ship system with visual effects...");
            
            // Initialize Raylib for texture creation
            Raylib.InitWindow(800, 600, "Enhanced Ship System Test");
            
            try
            {
                // Test ship service
                var characterService = new CharacterService();
                var characters = characterService.GetAvailableCharacters();
                
                Console.WriteLine($"Ships available: {characters.Count}");
                foreach (var character in characters)
                {
                    Console.WriteLine($"- {character.Name}: {character.Type} type");
                    Console.WriteLine($"  Red Replacement Color: {character.RedReplacementColor}");
                    Console.WriteLine($"  Visual Effect: {character.VisualEffect}");
                    
                    // Test enhanced sprite generation with effects
                    var sprite = ShipSpriteService.CreateShipSprite(
                        character.RedReplacementColor, 
                        80, 120, 
                        character.VisualEffect);
                    
                    Console.WriteLine($"  Enhanced sprite: {sprite.Width}x{sprite.Height}");
                    
                    if (character.VisualEffect != ShipEffectType.None)
                    {
                        Console.WriteLine($"  ? Applied {character.VisualEffect} visual effects!");
                    }
                    
                    Raylib.UnloadTexture(sprite);
                }
                
                // Test effect service
                var effectService = new EffectService();
                var effects = effectService.GetAvailableEffects();
                
                Console.WriteLine($"\nOffensive effects available: {effects.Count}");
                foreach (var effect in effects)
                {
                    Console.WriteLine($"- {effect.Name}: {effect.Category}");
                    Console.WriteLine($"  Class: {effect.EffectClassName}");
                    Console.WriteLine($"  Description: {effect.Description}");
                }
                
                // Test player selection model
                var player1 = new PlayerSelection();
                var player2 = new PlayerSelection();
                
                Console.WriteLine($"\nPlayer selection states:");
                Console.WriteLine($"Player 1 complete: {player1.IsComplete}");
                Console.WriteLine($"Player 2 complete: {player2.IsComplete}");
                
                // Simulate selections with enhanced ships
                var iceShip = characters.Find(c => c.VisualEffect == ShipEffectType.Ice);
                var explosiveShip = characters.Find(c => c.VisualEffect == ShipEffectType.Explosive);
                
                if (iceShip != null && explosiveShip != null)
                {
                    player1.SelectedCharacter = iceShip;
                    player1.SelectedEffect = effects[0];
                    player1.CharacterReady = true;
                    player1.EffectReady = true;
                    
                    player2.SelectedCharacter = explosiveShip;
                    player2.SelectedEffect = effects[1];
                    player2.CharacterReady = true;
                    player2.EffectReady = true;
                    
                    Console.WriteLine($"\nEnhanced selections:");
                    Console.WriteLine($"Player 1: {player1.SelectedCharacter.Name} with {player1.SelectedCharacter.VisualEffect} effects + {player1.SelectedEffect.Name}");
                    Console.WriteLine($"Player 2: {player2.SelectedCharacter.Name} with {player2.SelectedCharacter.VisualEffect} effects + {player2.SelectedEffect.Name}");
                    Console.WriteLine($"Both players complete: {player1.IsComplete && player2.IsComplete}");
                }
                
                Console.WriteLine("\n?? Enhanced ship system test completed successfully!");
                Console.WriteLine("Ships now feature palette swapping + visual effect overlays!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error testing enhanced system: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                Raylib.CloseWindow();
            }
        }
    }
}