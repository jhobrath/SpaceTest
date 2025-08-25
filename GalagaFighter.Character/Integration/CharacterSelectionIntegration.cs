using GalagaFighter.CharacterScreen.Models;
using Character = GalagaFighter.CharacterScreen.Models.Character;

namespace GalagaFighter.CharacterScreen.Integration
{
    public class CharacterSelectionResult
    {
        public PlayerSelection? Player1Selection { get; set; }
        public PlayerSelection? Player2Selection { get; set; }
        public bool SelectionCompleted { get; set; }
        public bool SelectionCancelled { get; set; }
        
        // Legacy properties for backwards compatibility
        public Models.Character? Player1Character => Player1Selection?.SelectedCharacter;
        public Models.Character? Player2Character => Player2Selection?.SelectedCharacter;
        
        // New properties for effects
        public OffensiveEffect? Player1Effect => Player1Selection?.SelectedEffect;
        public OffensiveEffect? Player2Effect => Player2Selection?.SelectedEffect;
    }

    public interface ICharacterSelectionIntegration
    {
        CharacterSelectionResult RunCharacterSelection();
    }

    public class CharacterSelectionIntegration : ICharacterSelectionIntegration
    {
        public CharacterSelectionResult RunCharacterSelection()
        {
            var characterSelection = new CharacterSelectionGame();
            characterSelection.Run();
            
            // Return the actual selection results
            return new CharacterSelectionResult
            {
                SelectionCompleted = characterSelection.SelectionComplete,
                SelectionCancelled = !characterSelection.SelectionComplete,
                Player1Selection = characterSelection.Player1Selection,
                Player2Selection = characterSelection.Player2Selection
            };
        }
    }
}