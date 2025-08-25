using GalagaFighter.CharacterScreen.Models;
using GalagaFighter.CharacterScreen.Services;
using GalagaFighter.CharacterScreen.UI;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using Character = GalagaFighter.CharacterScreen.Models.Character;

namespace GalagaFighter.CharacterScreen
{
    public enum SelectionPhase
    {
        ShipSelection,
        EffectSelection,
        Complete
    }

    public class CharacterSelectionGame
    {
        private readonly ICharacterService _characterService;
        private readonly IEffectService _effectService;
        private readonly CharacterSelectionUI _ui;
        private readonly List<Models.Character> _availableCharacters;
        private readonly List<OffensiveEffect> _availableEffects;
        
        private PlayerSelection _player1Selection = new PlayerSelection();
        private PlayerSelection _player2Selection = new PlayerSelection();
        private int _player1Index = 0;
        private int _player2Index = 0;
        
        private SelectionPhase _currentPhase = SelectionPhase.ShipSelection;
        private bool _gameComplete = false;

        public CharacterSelectionGame()
        {
            _characterService = new CharacterService();
            _effectService = new EffectService();
            _availableCharacters = _characterService.GetAvailableCharacters();
            _availableEffects = _effectService.GetAvailableEffects();
            InitializeWindow();
            _ui = new CharacterSelectionUI();
            
            InitializeSprites();
        }

        private void InitializeWindow()
        {
            int screenWidth = 1920;
            int screenHeight = 1080;
            
            Raylib.InitWindow(screenWidth, screenHeight, "Galaga Fighter - Ship & Effect Selection");
            Raylib.SetTargetFPS(60);
        }

        private void InitializeSprites()
        {
            // Preload all ship sprites for smooth UI experience
            _ui.PreloadShipSprites(_availableCharacters);
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose() && !_gameComplete)
            {
                Update();
                Draw();
            }

            Cleanup();
        }

        private void Update()
        {
            switch (_currentPhase)
            {
                case SelectionPhase.ShipSelection:
                    HandleShipSelection();
                    break;
                case SelectionPhase.EffectSelection:
                    HandleEffectSelection();
                    break;
                case SelectionPhase.Complete:
                    HandleGameStart();
                    break;
            }
        }

        private void HandlePlayerShipInput(int playerIndex, PlayerSelection selection, ref int index, KeyboardKey upKey, KeyboardKey downKey, KeyboardKey selectKey, KeyboardKey cancelKey)
        {
            if (selection.CharacterReady) 
            {
                if (Raylib.IsKeyPressed(cancelKey))
                {
                    selection.CharacterReady = false;
                    selection.SelectedCharacter = null;
                    PlayNavigationSound();
                }
                return;
            }
            if (Raylib.IsKeyPressed(upKey))
            {
                index = (index - 1 + _availableCharacters.Count) % _availableCharacters.Count;
                PlayNavigationSound();
            }
            else if (Raylib.IsKeyPressed(downKey))
            {
                index = (index + 1) % _availableCharacters.Count;
                PlayNavigationSound();
            }
            if (Raylib.IsKeyPressed(selectKey))
            {
                selection.SelectedCharacter = _availableCharacters[index];
                selection.CharacterReady = true;
                PlaySelectionSound();
            }
        }

        private void HandlePlayerEffectInput(int playerIndex, PlayerSelection selection, ref int index, KeyboardKey upKey, KeyboardKey downKey, KeyboardKey selectKey, KeyboardKey cancelKey)
        {
            if (selection.EffectReady) 
            {
                if (Raylib.IsKeyPressed(cancelKey))
                {
                    selection.EffectReady = false;
                    selection.SelectedEffect = null;
                    PlayNavigationSound();
                }
                return;
            }
            if (Raylib.IsKeyPressed(upKey))
            {
                index = (index - 1 + _availableEffects.Count) % _availableEffects.Count;
                PlayNavigationSound();
            }
            else if (Raylib.IsKeyPressed(downKey))
            {
                index = (index + 1) % _availableEffects.Count;
                PlayNavigationSound();
            }
            if (Raylib.IsKeyPressed(selectKey))
            {
                selection.SelectedEffect = _availableEffects[index];
                selection.EffectReady = true;
                PlaySelectionSound();
            }
        }

        private void HandleShipSelection()
        {
            HandlePlayerShipInput(1, _player1Selection, ref _player1Index, KeyboardKey.W, KeyboardKey.S, KeyboardKey.D, KeyboardKey.A);
            HandlePlayerShipInput(2, _player2Selection, ref _player2Index, KeyboardKey.Up, KeyboardKey.Down, KeyboardKey.Left, KeyboardKey.Right);
            if (_player1Selection.CharacterReady && _player2Selection.CharacterReady)
            {
                _currentPhase = SelectionPhase.EffectSelection;
                _player1Index = 0;
                _player2Index = 0;
                PlaySelectionSound();
            }
        }

        private void HandleEffectSelection()
        {
            HandlePlayerEffectInput(1, _player1Selection, ref _player1Index, KeyboardKey.W, KeyboardKey.S, KeyboardKey.D, KeyboardKey.A);
            HandlePlayerEffectInput(2, _player2Selection, ref _player2Index, KeyboardKey.Up, KeyboardKey.Down, KeyboardKey.Left, KeyboardKey.Right);
            if (_player1Selection.EffectReady && _player2Selection.EffectReady)
            {
                _currentPhase = SelectionPhase.Complete;
            }
        }

        private void HandleGameStart()
        {
            // Check for start command
            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                StartMainGame();
            }
        }

        private void Draw()
        {
            Raylib.BeginDrawing();
            _ui.DrawBackground();
            switch (_currentPhase)
            {
                case SelectionPhase.ShipSelection:
                    _ui.DrawTitle("SHIP SELECTION");
                    _ui.DrawPlayerShipSelection(_availableCharacters, _player1Index, _player1Selection.SelectedCharacter, _player1Selection.CharacterReady, true);
                    _ui.DrawPlayerShipSelection(_availableCharacters, _player2Index, _player2Selection.SelectedCharacter, _player2Selection.CharacterReady, false);
                    _ui.DrawShipInstructions();
                    break;
                case SelectionPhase.EffectSelection:
                    _ui.DrawTitle("EFFECT SELECTION");
                    _ui.DrawPlayerEffectSelection(_availableEffects, _player1Index, _player1Selection.SelectedEffect, _player1Selection.EffectReady, true);
                    _ui.DrawPlayerEffectSelection(_availableEffects, _player2Index, _player2Selection.SelectedEffect, _player2Selection.EffectReady, false);
                    _ui.DrawEffectInstructions();
                    _ui.DrawSelectedShips(_player1Selection.SelectedCharacter, _player2Selection.SelectedCharacter);
                    break;
                case SelectionPhase.Complete:
                    _ui.DrawTitle("READY TO LAUNCH");
                    _ui.DrawFinalSelections(_player1Selection, _player2Selection);
                    _ui.DrawStartPrompt();
                    break;
            }
            Raylib.EndDrawing();
        }

        private void StartMainGame()
        {
            // For now, just mark the game as complete
            // In a full implementation, this would transition to the main game
            // with the selected ships and effects
            
            Console.WriteLine($"Starting game with:");
            Console.WriteLine($"Player 1: {_player1Selection.SelectedCharacter?.Name} + {_player1Selection.SelectedEffect?.Name}");
            Console.WriteLine($"Player 2: {_player2Selection.SelectedCharacter?.Name} + {_player2Selection.SelectedEffect?.Name}");
            
            _gameComplete = true;
        }

        private void PlayNavigationSound()
        {
            // Disable navigation sound for up/down on character screen
            // (was: Console.Beep(400, 50);)
        }

        private void PlaySelectionSound()
        {
            try
            {
                //Console.Beep(600, 100);
            }
            catch
            {
                // Ignore if beep is not supported
            }
        }

        private void Cleanup()
        {
            _ui.Cleanup();
            ShipSpriteService.Cleanup();
            Raylib.CloseWindow();
        }

        // Public properties to access selection results (for integration)
        public PlayerSelection Player1Selection => _player1Selection;
        public PlayerSelection Player2Selection => _player2Selection;
        public bool SelectionComplete => _gameComplete && _player1Selection.IsComplete && _player2Selection.IsComplete;
    }
}