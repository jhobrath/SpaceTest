using GalagaFighter.CharacterScreen.Models;
using GalagaFighter.CharacterScreen.Services;
using GalagaFighter.Core;
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
            float uniformScale = GetUniformScale();
            int actualWidth = (int)(_screenWidth * uniformScale);
            int actualHeight = (int)(_screenHeight * uniformScale);
            Raylib.DrawRectangleGradientV(0, 0, actualWidth, actualHeight, 
                new Color(20, 20, 40, 255), new Color(5, 5, 15, 255));
            var random = new Random(42);
            for (int i = 0; i < 100; i++)
            {
                var x = random.Next(0, actualWidth);
                var y = random.Next(0, actualHeight);
                var brightness = random.Next(100, 255);
                Raylib.DrawPixel(x, y, new Color(brightness, brightness, brightness, 255));
            }
        }

        public void DrawTitle(string title = "SHIP SELECTION")
        {
            float uniformScale = GetUniformScale();
            int fontSize = (int)(72 * uniformScale);
            int textWidth = Raylib.MeasureText(title, fontSize);
            float centerX = (_screenWidth * uniformScale) / 2f;
            int posX = (int)(centerX - textWidth / 2f);
            int posY = (int)(50 * uniformScale);
            // Draw title with glow effect, both centered
            Raylib.DrawText(title, posX + 2, posY + 2, fontSize, Color.DarkBlue);
            Raylib.DrawText(title, posX, posY, fontSize, Color.White);
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
            float uniformScale = GetUniformScale();
            int baseScreenWidth = 1920;
            int baseScreenHeight = 1080;
            // Move ships up and away from center horizontally
            int startX = isPlayer1
                ? (int)((baseScreenWidth / 2 - 670) * uniformScale) // Player 1 left of center
                : (int)((baseScreenWidth / 2 + 250) * uniformScale); // Player 2 right of center
            int centerY = (int)((baseScreenHeight / 2.25) * uniformScale); // Move up vertically
            int itemSpacing = (int)((baseScreenHeight * 0.17f) * uniformScale);

            string playerLabel = isPlayer1 ? "PLAYER 1" : "PLAYER 2";
            int labelFontSize = (int)(40 * uniformScale);
            Color playerColor = isPlayer1 ? Color.Blue : Color.Red;
            Raylib.DrawText(playerLabel, startX + 110, (int)(200 * uniformScale), labelFontSize, playerColor);

            int count = characters.Count;
            int prevIndex = (selectedIndex - 1 + count) % count;
            int nextIndex = (selectedIndex + 1) % count;

            int shipWidth = (int)(220 * uniformScale);
            int shipHeight = (int)(140 * uniformScale);

            DrawShipItem(characters[prevIndex], startX, centerY - itemSpacing, false, 0.4f, playerColor, selection, isReady, false, shipWidth, shipHeight, uniformScale);
            DrawShipItem(characters[selectedIndex], startX, centerY, true, 1.0f, playerColor, selection, isReady, true, shipWidth, shipHeight, uniformScale);
            DrawShipItem(characters[nextIndex], startX, centerY + itemSpacing, false, 0.4f, playerColor, selection, isReady, false, shipWidth, shipHeight, uniformScale);

            // --- Ship selection indicator squares ---
            int indicatorCount = 10;
            int indicatorSize = (int)(22 * uniformScale);
            int indicatorSpacing = 0;
            int indicatorAreaHeight = itemSpacing * 2 + shipHeight;
            if (indicatorCount > 1)
                indicatorSpacing = (indicatorAreaHeight - indicatorSize) / (indicatorCount - 1);
            int indicatorStartY = centerY - itemSpacing;
            int indicatorX = isPlayer1
                ? startX - (int)(40 * uniformScale)
                : startX + shipWidth + (int)(300 * uniformScale); // was 260, now 300 for +40 units
            for (int i = 0; i < indicatorCount; i++)
            {
                int y = indicatorStartY + i * indicatorSpacing;
                Color baseColor = characters[i].ShipTintColor;
                bool isSelectedIndicator = (i == selectedIndex);
                Color fillColor;
                if (isSelectedIndicator)
                {
                    fillColor = MakeColor(baseColor, 0.71f); // semi-transparent, matches alpha 180
                }
                else
                {
                    // Less saturated and more translucent for non-selected
                    // Manual HSV conversion using ColorExtensions
                    float h, s, v;
                    GalagaFighter.Core.ColorExtensions.RgbToHsv(baseColor.R, baseColor.G, baseColor.B, out h, out s, out v);
                    s = 0.25f; // Lower saturation
                    byte r, g, b;
                    GalagaFighter.Core.ColorExtensions.HsvToRgb(h, s, v, out r, out g, out b);
                    var lessSaturated = new Color(r, g, b, baseColor.A);
                    fillColor = MakeColor(lessSaturated, 0.47f); // alpha ~120
                }
                Raylib.DrawRectangle(indicatorX, y, indicatorSize, indicatorSize, fillColor);
                if (isSelectedIndicator)
                {
                    Color borderColor = isPlayer1 ? Color.Blue : Color.Red;
                    Raylib.DrawRectangleLines(indicatorX, y, indicatorSize, indicatorSize, borderColor);
                }
            }

            if (isReady && selection != null)
            {
                string readyText = "READY!";
                Vector2 readySize = Raylib.MeasureTextEx(_textFont, readyText, (int)(32 * uniformScale), 1);
                Vector2 readyPos = new Vector2(startX + (shipWidth - readySize.X) / 2, centerY + (int)(itemSpacing * 1.1f));
                Raylib.DrawTextEx(_textFont, readyText, readyPos, (int)(32 * uniformScale), 1, Color.Green);
            }
        }

        private void DrawShipItem(Character character, int x, int y, bool isSelected, float alpha, Color playerColor, Character? selection, bool isReady, bool isCenter, int shipWidth, int shipHeight, float uniformScale)
        {
            Color fadedColor = MakeColor(255, 255, 255, alpha);
            Color textColor = isSelected ? playerColor : Color.White;
            Color bgColor = isSelected ? MakeColor(playerColor, 0.31f * alpha) : MakeColor(255, 255, 255, 0.12f * alpha);

            // Make the filled rectangle cover the entire ship selection area (sprite + text)
            int fillPad = (int)(8 * uniformScale); // Equal margin on all sides
            int extraPad = (int)(60 * uniformScale); // Additional width to ensure full coverage
            int textOffset = (int)(18 * uniformScale);
            int textWidth = (int)(180 * uniformScale);
            int fillWidth = shipWidth + textOffset + textWidth + extraPad + fillPad * 2;
            int fillHeight = shipHeight + fillPad * 2;
            int fillX = x - fillPad;
            int fillY = y - fillPad;
            Raylib.DrawRectangle(fillX, fillY, fillWidth, fillHeight, bgColor);

            // Draw border rectangle around the ship sprite only
            Raylib.DrawRectangleLines(x, y, shipWidth, shipHeight, fadedColor);

            if (_shipPortraits.ContainsKey(character.Id))
            {
                var shipTexture = _shipPortraits[character.Id];
                float scale = Math.Min((float)shipWidth / shipTexture.Width, (float)shipHeight / shipTexture.Height);
                int drawWidth = (int)(shipTexture.Width * scale);
                int drawHeight = (int)(shipTexture.Height * scale);
                // Center the ship in the rectangle: destination rect top-left is (x + (shipWidth-drawWidth)/2, y + (shipHeight-drawHeight)/2)
                float destX = x + (shipWidth - drawWidth) / 2f;
                float destY = y + (shipHeight - drawHeight) / 2f;
                Vector2 origin = new Vector2(drawWidth / 2f, drawHeight / 2f);
                Raylib.DrawTexturePro(
                    shipTexture,
                    new Rectangle(0, 0, shipTexture.Width, shipTexture.Height),
                    new Rectangle(destX + drawWidth / 2f, destY + drawHeight / 2f, drawWidth, drawHeight),
                    origin,
                    180f,
                    fadedColor
                );
            }
            else
            {
                Raylib.DrawRectangle(x, y, shipWidth, shipHeight, character.ShipTintColor);
                Raylib.DrawRectangleLines(x, y, shipWidth, shipHeight, fadedColor);
            }

            if (isSelected && !isReady)
            {
                Raylib.DrawRectangleLines(x - (int)(4 * uniformScale), y - (int)(4 * uniformScale), shipWidth + (int)(8 * uniformScale), shipHeight + (int)(8 * uniformScale), playerColor);
            }
            else if (selection == character && isReady)
            {
                Raylib.DrawRectangleLines(x - (int)(4 * uniformScale), y - (int)(4 * uniformScale), shipWidth + (int)(8 * uniformScale), shipHeight + (int)(8 * uniformScale), Color.Green);
            }

            Raylib.DrawText(character.Name, x + shipWidth + (int)(18 * uniformScale), y + (int)(8 * uniformScale), (int)(22 * uniformScale), textColor);
            Raylib.DrawText($"Type: {character.Type}", x + shipWidth + (int)(18 * uniformScale), y + (int)(32 * uniformScale), (int)(16 * uniformScale), Color.LightGray);
            DrawStatsPreview(character.Stats, x + shipWidth + (int)(18 * uniformScale), y + (int)(54 * uniformScale), textColor);
            // Increase font size and wrap description
            DrawWrappedText(character.Description, x + shipWidth + (int)(18 * uniformScale), y + (int)(76 * uniformScale), (int)(180 * uniformScale), (int)(18 * uniformScale), Color.LightGray);
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
            // Improved word wrapping
            string[] words = text.Split(' ');
            string line = "";
            int lineHeight = fontSize + 2;
            int curY = y;
            foreach (var word in words)
            {
                string testLine = line.Length == 0 ? word : line + " " + word;
                int testWidth = Raylib.MeasureText(testLine, fontSize);
                if (testWidth > maxWidth && line.Length > 0)
                {
                    Raylib.DrawText(line, x, curY, fontSize, color);
                    curY += lineHeight;
                    line = word;
                }
                else
                {
                    line = testLine;
                }
            }
            if (line.Length > 0)
            {
                Raylib.DrawText(line, x, curY, fontSize, color);
            }
        }

        public void DrawShipInstructions()
        {
            float uniformScale = GetUniformScale();
            string[] instructions = {
                "PLAYER 1: W/S to navigate, D to select, A to cancel",
                "PLAYER 2: UP/DOWN to navigate, LEFT to select, RIGHT to cancel"
            };
            int instructionY = (int)(_screenHeight * uniformScale - 100 * uniformScale);
            int fontSize = (int)(20 * uniformScale);
            for (int i = 0; i < instructions.Length; i++)
            {
                int textWidth = Raylib.MeasureText(instructions[i], fontSize);
                float centerX = (_screenWidth * uniformScale) / 2f;
                int posX = (int)(centerX - textWidth / 2f);
                int posY = instructionY + (int)(i * 30 * uniformScale);
                Raylib.DrawText(instructions[i], posX, posY, fontSize, Color.LightGray);
            }
        }

        public void DrawEffectInstructions()
        {
            float uniformScale = GetUniformScale();
            string[] instructions = {
                "Select your offensive augment effect",
                "PLAYER 1: W/S to navigate, D to select, A to cancel",
                "PLAYER 2: UP/DOWN to navigate, LEFT to select, RIGHT to cancel"
            };
            int instructionY = (int)(_screenHeight * uniformScale - 120 * uniformScale);
            int fontSize = (int)(20 * uniformScale);
            for (int i = 0; i < instructions.Length; i++)
            {
                int textWidth = Raylib.MeasureText(instructions[i], fontSize);
                float centerX = (_screenWidth * uniformScale) / 2f;
                int posX = (int)(centerX - textWidth / 2f);
                int posY = instructionY + (int)(i * 30 * uniformScale);
                Color color = i == 0 ? Color.Yellow : Color.LightGray;
                Raylib.DrawText(instructions[i], posX, posY, fontSize, color);
            }
        }

        public void DrawStartPrompt()
        {
            float uniformScale = GetUniformScale();
            string prompt = "BOTH PLAYERS READY - PRESS SPACE TO START";
            int fontSize = (int)(32 * uniformScale);
            Vector2 textSize = Raylib.MeasureTextEx(_textFont, prompt, fontSize, 1);
            Vector2 position = new Vector2((_screenWidth * uniformScale - textSize.X) / 2, _screenHeight * uniformScale - 100 * uniformScale);
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

        // Helper to create Color with correct overload
        private Color MakeColor(int r, int g, int b, float alpha)
        {
            return new Color((byte)r, (byte)g, (byte)b, (byte)(255 * alpha));
        }
        private Color MakeColor(Color baseColor, float alpha)
        {
            return new Color(baseColor.R, baseColor.G, baseColor.B, (byte)(255 * alpha));
        }

        public float GetUniformScale()
        {
            int actualWidth = Raylib.GetScreenWidth();
            int actualHeight = Raylib.GetScreenHeight();
            return Math.Min(actualWidth / 1920f, actualHeight / 1080f);
        }
    }
}