using GalagaFighter.CharacterScreen;
using GalagaFighter.CharacterScreen.Services;
using Raylib_cs;
using System;

namespace GalagaFighter.CharacterScreen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("?? Enhanced Ship Selection System");
            Console.WriteLine("=================================");
            
            // Debug: Show which ships have visual effects
            var characterService = new CharacterService();
            var characters = characterService.GetAvailableCharacters();
            
            Console.WriteLine("Ships with visual effects:");
            foreach (var character in characters)
            {
                if (character.VisualEffect != ShipEffectType.None)
                {
                    Console.WriteLine($"? {character.Name} ? {character.VisualEffect} effects");
                }
                else
                {
                    Console.WriteLine($"   {character.Name} ? No special effects");
                }
            }
            
            Console.WriteLine("\nLook for ships marked with ? - they should have visual effects!");
            Console.WriteLine("Press any key to start the selection screen...");
            Console.ReadKey();
            
            var characterSelection = new CharacterSelectionGame();
            characterSelection.Run();
        }
    }
}