using GalagaFighter.CharacterScreen.Models;
using GalagaFighter.CharacterScreen.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.CharacterScreen.UI
{
    public class CharacterSelectionUI
    {
        private readonly int _screenWidth = 1920;
        private readonly int _screenHeight = 1080;
        private readonly Font _titleFont;
        private readonly Font _textFont;
        private readonly Dictionary<string, Texture2D> _shipPortraits;

        public CharacterSelectionUI()
        {
            _titleFont = Raylib.GetFontDefault();
            _textFont = Raylib.GetFontDefault();
            _shipPortraits = new Dictionary<string, Texture2D>();
        }

        public void PreloadShipSprites(List<Character> characters)
        {
            // Generate and cache ship portraits for all characters
            foreach (var character in characters)
            {
                if (!_shipPortraits.ContainsKey(character.Id))
                {
                    _shipPortraits[character.Id] = ShipSpriteService.CreateShipPortrait(
                        character.ShipTintColor, 
                        100,
                        character.VisualEffect);
                }
            }
        }

        public void DrawBackground()
        {
            // Draw a gradient background
            Raylib.DrawRectangleGradientV(0, 0, _screenWidth, _screenHeight, 
                new Color(20, 20, 40, 255), new Color(5, 5, 15, 255));
            
            // Draw some decorative stars
            var random = new Random(42); // Fixed seed for consistent star positions
            for (int i = 0; i < 100; i++)
            {
                var x = random.Next(0, _screenWidth);
                var y = random.Next(0, _screenHeight);
                var brightness = random.Next(100, 255);
                Raylib.DrawPixel(x, y, new Color(brightness, brightness, brightness, 255));
            }
        }

        public void DrawTitle(string title = "SHIP SELECTION")
        {
            int fontSize = 72;
            Vector2 textSize = Raylib.MeasureTextEx(_titleFont, title, fontSize, 2);
            Vector2 position = new Vector2((_screenWidth - textSize.X) / 2, 50);
            
            // Draw title with glow effect
            Raylib.DrawTextEx(_titleFont, title, new Vector2(position.X + 2, position.Y + 2), fontSize, 2, Color.DarkBlue);
            Raylib.DrawTextEx(_titleFont, title, position, fontSize, 2, Color.White);
        }

        public void DrawPlayer1ShipSelection(List<Character> characters, int selectedIndex, Character? selection, bool isReady)
        {
            DrawPlayerShipSelection(characters, selectedIndex, selection, isReady, true);
        }

        public void DrawPlayer2ShipSelection(List<Character> characters, int selectedIndex, Character? selection, bool isReady)
        {
            DrawPlayerShipSelection(characters, selectedIndex, selection, isReady, false);
        }

        private void DrawPlayerShipSelection(List<Character> characters, int selectedIndex, Character? selection, bool isReady, bool isPlayer1)
        {
            string playerLabel = isPlayer1 ? "PLAYER 1" : "PLAYER 2";
            int startX = isPlayer1 ? 100 : _screenWidth - 500;
            int startY = 200;
            
            // Draw player label
            int labelFontSize = 36;
            Color playerColor = isPlayer1 ? Color.Blue : Color.Red;
            Raylib.DrawText(playerLabel, startX, startY, labelFontSize, playerColor);
            
            // Draw character list
            for (int i = 0; i < characters.Count; i++)
            {
                var character = characters[i];
                int itemY = startY + 80 + (i * 130);

                // Highlight selected character
                Color backgroundColor = new Color(255, 255, 255, 0);
                Color textColor = Color.White;
                
                if (i == selectedIndex && !isReady)
                {
                    backgroundColor = new Color((int)playerColor.R, (int)playerColor.G, (int)playerColor.B, 50);
                    textColor = playerColor;
                    // Draw selection glow around ship
                    Raylib.DrawRectangleLines(startX - 5, itemY - 5, 110, 110, playerColor);
                }
                else if (selection == character && isReady)
                {
                    backgroundColor = new Color(0, 255, 0, 50);
                    textColor = Color.Green;
                    // Draw ready glow around ship
                    Raylib.DrawRectangleLines(startX - 5, itemY - 5, 110, 110, Color.Green);
                }
                
                // Draw background
                if (backgroundColor.A > 0)
                {
                    Raylib.DrawRectangle(startX - 10, itemY - 10, 400, 120, backgroundColor);
                }
                
                // Draw ship portrait
                if (_shipPortraits.ContainsKey(character.Id))
                {
                    var shipTexture = _shipPortraits[character.Id];
                    Raylib.DrawTexture(shipTexture, startX, itemY, Color.White);
                }
                else
                {
                    // Fallback rectangle if texture not loaded
                    Raylib.DrawRectangle(startX, itemY, 100, 100, character.ShipTintColor);
                    Raylib.DrawRectangleLines(startX, itemY, 100, 100, textColor);
                }
                
                // Draw character name and type
                Raylib.DrawText(character.Name, startX + 120, itemY, 24, textColor);
                Raylib.DrawText($"Type: {character.Type}", startX + 120, itemY + 25, 16, Color.LightGray);
                
                // Draw stats preview
                DrawStatsPreview(character.Stats, startX + 120, itemY + 50, textColor);
                
                // Draw description
                DrawWrappedText(character.Description, startX + 120, itemY + 75, 260, 14, Color.LightGray);
            }
            
            // Draw selection status
            if (isReady && selection != null)
            {
                string readyText = "READY!";
                Vector2 readySize = Raylib.MeasureTextEx(_textFont, readyText, 32, 1);
                Vector2 readyPos = new Vector2(startX + (400 - readySize.X) / 2, startY + 80 + (characters.Count * 130) + 20);
                Raylib.DrawTextEx(_textFont, readyText, readyPos, 32, 1, Color.Green);
            }
        }

        public void DrawPlayer1EffectSelection(List<OffensiveEffect> effects, int selectedIndex, OffensiveEffect? selection, bool isReady)
        {
            DrawPlayerEffectSelection(effects, selectedIndex, selection, isReady, true);
        }

        public void DrawPlayer2EffectSelection(List<OffensiveEffect> effects, int selectedIndex, OffensiveEffect? selection, bool isReady)
        {
            DrawPlayerEffectSelection(effects, selectedIndex, selection, isReady, false);
        }

        private void DrawPlayerEffectSelection(List<OffensiveEffect> effects, int selectedIndex, OffensiveEffect? selection, bool isReady, bool isPlayer1)
        {
            string playerLabel = isPlayer1 ? "PLAYER 1" : "PLAYER 2";
            int startX = isPlayer1 ? 200 : _screenWidth - 600;
            int startY = 300;
            
            // Draw player label
            int labelFontSize = 36;
            Color playerColor = isPlayer1 ? Color.Blue : Color.Red;
            Raylib.DrawText(playerLabel, startX, startY, labelFontSize, playerColor);
            
            // Draw effect list
            for (int i = 0; i < effects.Count; i++)
            {
                var effect = effects[i];
                int itemY = startY + 80 + (i * 120);

                // Highlight selected effect
                Color backgroundColor = new Color(255, 255, 255, 0);
                Color textColor = Color.White;
                
                if (i == selectedIndex && !isReady)
                {
                    backgroundColor = new Color((int)playerColor.R, (int)playerColor.G, (int)playerColor.B, 50);
                    textColor = playerColor;
                    Raylib.DrawRectangleLines(startX - 5, itemY - 5, 385, 105, playerColor);
                }
                else if (selection == effect && isReady)
                {
                    backgroundColor = new Color(0, 255, 0, 50);
                    textColor = Color.Green;
                    Raylib.DrawRectangleLines(startX - 5, itemY - 5, 385, 105, Color.Green);
                }
                
                // Draw background
                if (backgroundColor.A > 0)
                {
                    Raylib.DrawRectangle(startX - 10, itemY - 10, 400, 120, backgroundColor);
                }
                
                // Draw effect icon placeholder
                Raylib.DrawRectangle(startX, itemY, 80, 80, new Color(60, 60, 60, 255));
                Raylib.DrawRectangleLines(startX, itemY, 80, 80, textColor);
                Raylib.DrawText("FX", startX + 25, itemY + 30, 20, textColor);
                
                // Draw effect name and category
                Raylib.DrawText(effect.Name, startX + 90, itemY, 24, textColor);
                Raylib.DrawText($"Category: {effect.Category}", startX + 90, itemY + 25, 16, Color.LightGray);
                
                // Draw description
                DrawWrappedText(effect.Description, startX + 90, itemY + 50, 280, 14, Color.LightGray);
            }
            
            // Draw selection status
            if (isReady && selection != null)
            {
                string readyText = "EFFECT READY!";
                Vector2 readySize = Raylib.MeasureTextEx(_textFont, readyText, 32, 1);
                Vector2 readyPos = new Vector2(startX + (380 - readySize.X) / 2, startY + 80 + (effects.Count * 120) + 20);
                Raylib.DrawTextEx(_textFont, readyText, readyPos, 32, 1, Color.Green);
            }
        }

        public void DrawSelectedShips(Character? player1Ship, Character? player2Ship)
        {
            // Draw small previews of selected ships in corners during effect selection
            if (player1Ship != null)
            {
                Raylib.DrawText("P1 Ship:", 50, 50, 20, Color.Blue);
                if (_shipPortraits.ContainsKey(player1Ship.Id))
                {
                    var texture = _shipPortraits[player1Ship.Id];
                    Raylib.DrawTextureEx(texture, new Vector2(50, 80), 0, 0.5f, Color.White);
                }
                Raylib.DrawText(player1Ship.Name, 50, 130, 16, Color.White);
            }
            
            if (player2Ship != null)
            {
                Raylib.DrawText("P2 Ship:", _screenWidth - 150, 50, 20, Color.Red);
                if (_shipPortraits.ContainsKey(player2Ship.Id))
                {
                    var texture = _shipPortraits[player2Ship.Id];
                    Raylib.DrawTextureEx(texture, new Vector2(_screenWidth - 150, 80), 0, 0.5f, Color.White);
                }
                Raylib.DrawText(player2Ship.Name, _screenWidth - 150, 130, 16, Color.White);
            }
        }

        public void DrawFinalSelections(PlayerSelection player1, PlayerSelection player2)
        {
            int centerX = _screenWidth / 2;
            int centerY = _screenHeight / 2;
            
            // Player 1 final selection
            if (player1.SelectedCharacter != null && player1.SelectedEffect != null)
            {
                Raylib.DrawText("PLAYER 1", centerX - 400, centerY - 200, 36, Color.Blue);
                
                // Ship
                if (_shipPortraits.ContainsKey(player1.SelectedCharacter.Id))
                {
                    var texture = _shipPortraits[player1.SelectedCharacter.Id];
                    Raylib.DrawTexture(texture, centerX - 400, centerY - 150, Color.White);
                }
                Raylib.DrawText(player1.SelectedCharacter.Name, centerX - 400, centerY - 40, 24, Color.White);
                
                // Effect
                Raylib.DrawRectangle(centerX - 400, centerY + 20, 80, 80, new Color(60, 60, 60, 255));
                Raylib.DrawRectangleLines(centerX - 400, centerY + 20, 80, 80, Color.Blue);
                Raylib.DrawText("FX", centerX - 375, centerY + 50, 20, Color.Blue);
                Raylib.DrawText(player1.SelectedEffect.Name, centerX - 400, centerY + 110, 20, Color.White);
            }
            
            // Player 2 final selection
            if (player2.SelectedCharacter != null && player2.SelectedEffect != null)
            {
                Raylib.DrawText("PLAYER 2", centerX + 200, centerY - 200, 36, Color.Red);
                
                // Ship
                if (_shipPortraits.ContainsKey(player2.SelectedCharacter.Id))
                {
                    var texture = _shipPortraits[player2.SelectedCharacter.Id];
                    Raylib.DrawTexture(texture, centerX + 200, centerY - 150, Color.White);
                }
                Raylib.DrawText(player2.SelectedCharacter.Name, centerX + 200, centerY - 40, 24, Color.White);
                
                // Effect
                Raylib.DrawRectangle(centerX + 200, centerY + 20, 80, 80, new Color(60, 60, 60, 255));
                Raylib.DrawRectangleLines(centerX + 200, centerY + 20, 80, 80, Color.Red);
                Raylib.DrawText("FX", centerX + 225, centerY + 50, 20, Color.Red);
                Raylib.DrawText(player2.SelectedEffect.Name, centerX + 200, centerY + 110, 20, Color.White);
            }
        }

        private void DrawStatsPreview(CharacterStats stats, int x, int y, Color color)
        {
            int fontSize = 14;
            Color statColor = new Color((int)color.R, (int)color.G, (int)color.B, 180);
            
            Raylib.DrawText($"HP:{stats.Health:F0} SPD:{stats.Speed:F1} FR:{stats.FireRate:F1} DMG:{stats.Damage:F1}", x, y, fontSize, statColor);
        }

        private void DrawWrappedText(string text, int x, int y, int maxWidth, int fontSize, Color color)
        {
            // Simple text wrapping - in a real implementation you'd want more sophisticated word wrapping
            if (text.Length > 40)
            {
                text = text.Substring(0, 37) + "...";
            }
            Raylib.DrawText(text, x, y, fontSize, color);
        }

        public void DrawShipInstructions()
        {
            string[] instructions = {
                "PLAYER 1: W/S to navigate, D to select, A to cancel",
                "PLAYER 2: UP/DOWN to navigate, LEFT to select, RIGHT to cancel"
            };
            
            int instructionY = _screenHeight - 100;
            int fontSize = 20;
            
            for (int i = 0; i < instructions.Length; i++)
            {
                Vector2 textSize = Raylib.MeasureTextEx(_textFont, instructions[i], fontSize, 1);
                Vector2 position = new Vector2((_screenWidth - textSize.X) / 2, instructionY + (i * 30));
                Raylib.DrawTextEx(_textFont, instructions[i], position, fontSize, 1, Color.LightGray);
            }
        }

        public void DrawEffectInstructions()
        {
            string[] instructions = {
                "Select your offensive augment effect",
                "PLAYER 1: W/S to navigate, D to select, A to cancel",
                "PLAYER 2: UP/DOWN to navigate, LEFT to select, RIGHT to cancel"
            };
            
            int instructionY = _screenHeight - 120;
            int fontSize = 20;
            
            for (int i = 0; i < instructions.Length; i++)
            {
                Vector2 textSize = Raylib.MeasureTextEx(_textFont, instructions[i], fontSize, 1);
                Vector2 position = new Vector2((_screenWidth - textSize.X) / 2, instructionY + (i * 30));
                Color color = i == 0 ? Color.Yellow : Color.LightGray;
                Raylib.DrawTextEx(_textFont, instructions[i], position, fontSize, 1, color);
            }
        }

        public void DrawStartPrompt()
        {
            string prompt = "BOTH PLAYERS READY - PRESS SPACE TO START";
            int fontSize = 32;
            Vector2 textSize = Raylib.MeasureTextEx(_textFont, prompt, fontSize, 1);
            Vector2 position = new Vector2((_screenWidth - textSize.X) / 2, _screenHeight - 100);
            
            // Pulsing effect
            float pulse = (float)Math.Sin(Raylib.GetTime() * 4) * 0.3f + 0.7f;
            Color promptColor = new Color((int)(255 * pulse), (int)(255 * pulse), 0, 255);
            
            Raylib.DrawTextEx(_textFont, prompt, position, fontSize, 1, promptColor);
        }

        public void Cleanup()
        {
            // Unload all ship portrait textures
            foreach (var texture in _shipPortraits.Values)
            {
                Raylib.UnloadTexture(texture);
            }
            _shipPortraits.Clear();
        }
    }
}