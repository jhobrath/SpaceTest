using GalagaFighter.CharacterScreen.Models;
using GalagaFighter.CharacterScreen.Services;
using Raylib_cs;
using System;

namespace GalagaFighter.CharacterScreen.Tests
{
    public class CommandLineArgumentTest
    {
        public static void TestArgumentFormatting()
        {
            Console.WriteLine("?? Testing Command Line Argument Formatting");
            Console.WriteLine("==========================================");

            var characterService = new CharacterService();
            var effectService = new EffectService();
            var argumentService = new CommandLineArgumentService();

            var characters = characterService.GetAvailableCharacters();
            var effects = effectService.GetAvailableEffects();

            // Test with first character and first effect
            if (characters.Count > 0 && effects.Count > 0)
            {
                var testSelection = new PlayerSelection
                {
                    SelectedCharacter = characters[0],
                    SelectedEffect = effects[0],
                    CharacterReady = true,
                    EffectReady = true
                };

                try
                {
                    var formattedArg = argumentService.FormatPlayerArgument(testSelection);
                    Console.WriteLine($"? Test Character: {testSelection.SelectedCharacter.Name}");
                    Console.WriteLine($"? Test Effect: {testSelection.SelectedEffect.Name}");
                    Console.WriteLine($"? Formatted Argument: {formattedArg}");
                    
                    // Test with different character
                    if (characters.Count > 1)
                    {
                        testSelection.SelectedCharacter = characters[1];
                        var formattedArg2 = argumentService.FormatPlayerArgument(testSelection);
                        Console.WriteLine($"? Second Character: {testSelection.SelectedCharacter.Name}");
                        Console.WriteLine($"? Second Formatted Argument: {formattedArg2}");
                    }

                    Console.WriteLine($"?? Example command line:");
                    Console.WriteLine($"   GalagaFighter.Core.exe --player1 \"{formattedArg}\" --player2 \"{formattedArg}\"");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"? Error formatting arguments: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("? No characters or effects available for testing");
            }

            Console.WriteLine("?? Test completed!");
        }
    }
}