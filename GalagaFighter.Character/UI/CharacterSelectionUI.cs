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
        private Font _font14;
        private Font _font16;
        private Font _font18;
        private Font _font22;
        private Font _font28;
        private Font _font40;
        private Font _font72;

        private static Dictionary<string, Texture2D> _effectIcons = new Dictionary<string, Texture2D>();

        public CharacterSelectionUI()
        {
            LoadFonts();
            _shipPortraits = new Dictionary<string, Texture2D>();
        }

        private void LoadFonts()
        {
            _font14 = Raylib.LoadFontEx(@"Fonts\Roboto-Regular.ttf", 14, [], 0);
            _font16 = Raylib.LoadFontEx(@"Fonts\Roboto-Regular.ttf", 16, [], 0);
            _font18 = Raylib.LoadFontEx(@"Fonts\Roboto-Regular.ttf", 18, [], 0);
            _font22 = Raylib.LoadFontEx(@"Fonts\Roboto-Regular.ttf", 22, [], 0);
            _font28 = Raylib.LoadFontEx(@"Fonts\Roboto-Regular.ttf", 28, [], 0);
            _font40 = Raylib.LoadFontEx(@"Fonts\Roboto-Regular.ttf", 40, [], 0);
            _font72 = Raylib.LoadFontEx(@"Fonts\Roboto-Regular.ttf", 72, [], 0);
            Raylib.SetTextureFilter(_font14.Texture, TextureFilter.Point);
            Raylib.SetTextureFilter(_font16.Texture, TextureFilter.Point);
            Raylib.SetTextureFilter(_font18.Texture, TextureFilter.Point);
            Raylib.SetTextureFilter(_font22.Texture, TextureFilter.Point);
            Raylib.SetTextureFilter(_font28.Texture, TextureFilter.Point);
            Raylib.SetTextureFilter(_font40.Texture, TextureFilter.Point);
            Raylib.SetTextureFilter(_font72.Texture, TextureFilter.Point);

            var effectService = new EffectService();
            var effects = effectService.GetAvailableEffects();
            foreach(var effect in effects)
            {
                _effectIcons.Add(effect.IconPath, Raylib.LoadTexture(effect.IconPath));
            }
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
            var textDimensions = Raylib.MeasureTextEx(_font72, title, fontSize, 1);
            float centerX = (_screenWidth * uniformScale) / 2f;
            int posX = (int)(centerX - textDimensions.X/2);
            int posY = (int)(50 * uniformScale);
            // Draw title with glow effect, both centered
            DrawTextEx( title, new Vector2(posX + 2, posY + 2), fontSize, 1, Color.DarkBlue);
            DrawTextEx(title,new Vector2( posX, posY), fontSize,1, Color.White);
        }

        public void DrawPlayerShipSelection(List<Character> characters, int selectedIndex, Character? selection, bool isReady, bool isPlayer1)
        {
            DrawPlayerShipSelectionInternal(characters, selectedIndex, selection, isReady, isPlayer1);
        }

        public void DrawPlayerEffectSelection(List<OffensiveEffect> effects, int selectedIndex, OffensiveEffect? selection, bool isReady, bool isPlayer1)
        {
            DrawPlayerEffectSelectionInternal(effects, selectedIndex, selection, isReady, isPlayer1);
        }

        private void DrawPlayerShipSelectionInternal(List<Character> characters, int selectedIndex, Character? selection, bool isReady, bool isPlayer1)
        {
            float uniformScale = GetUniformScale();
            int baseScreenWidth = 1920;
            int baseScreenHeight = 1080;
            // Move ships up and away from center horizontally
            int startX = isPlayer1
                ? (int)((baseScreenWidth / 2 - 670) * uniformScale) // Player 1 left of center
                : (int)((baseScreenWidth / 2 + 175) * uniformScale); // Player 2 right of center
            int centerY = (int)((baseScreenHeight / 2.25) * uniformScale); // Move up vertically
            int itemSpacing = (int)((baseScreenHeight * 0.17f) * uniformScale);

            string playerLabel = isPlayer1 ? "PLAYER 1" : "PLAYER 2";
            int labelFontSize = (int)(40 * uniformScale);
            Color playerColor = isPlayer1 ? Color.Blue : Color.Red;
            DrawTextEx( playerLabel, new Vector2(startX + 150 + (isPlayer1 ? 0 : 50), (int)(200 * uniformScale)), labelFontSize, 1, playerColor);

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
                    fillColor = MakeColor(baseColor, 0.84f); // semi-transparent, matches alpha 180
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

            DrawTextEx(character.Name,new Vector2( x + shipWidth + (int)(18 * uniformScale), y + (int)(8 * uniformScale)), (int)(22 * uniformScale),1, textColor);
            DrawTextEx($"Type: {character.Type}",new Vector2( x + shipWidth + (int)(18 * uniformScale), y + (int)(32 * uniformScale)), (int)(16 * uniformScale),1, Color.LightGray);
            DrawStatsPreview(character.Stats, x + shipWidth + (int)(18 * uniformScale), y + (int)(54 * uniformScale), textColor);
            // Increase font size and wrap description
            DrawWrappedText(character.Description, x + shipWidth + (int)(18 * uniformScale), y + (int)(76 * uniformScale), (int)(180 * uniformScale), (int)(18 * uniformScale), Color.LightGray);
        }

        private void DrawPlayerEffectSelectionInternal(List<OffensiveEffect> effects, int selectedIndex, OffensiveEffect? selection, bool isReady, bool isPlayer1)
        {
            string playerLabel = isPlayer1 ? "PLAYER 1" : "PLAYER 2";
            int startX = isPlayer1 ? 200 : _screenWidth - 600;
            int startY = 300;
            
            // Draw player label
            int labelFontSize = 36;
            Color playerColor = isPlayer1 ? Color.Blue : Color.Red;
            DrawTextEx(playerLabel,new Vector2( startX, startY), labelFontSize,1, playerColor);
            
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
                Raylib.DrawTexturePro(_effectIcons[effect.IconPath], 
                    new Rectangle(0, 0, _effectIcons[effect.IconPath].Width, _effectIcons[effect.IconPath].Height),
                    new Rectangle(startX, itemY, 80, 80), new Vector2(0, 0), 0f, Color.White);// new Vector2(startX, startY), 0f, .33f, Color.White);
                
                // Draw effect name and category
                DrawTextEx(effect.Name,new Vector2( startX + 90, itemY), 22,1, textColor);
                DrawTextEx($"Category: {effect.Category}",new Vector2( startX + 90, itemY + 25), 16,1, Color.LightGray);
                
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
                DrawTextEx("P1 Ship:",new Vector2( 50, 50), 20,1, Color.Blue);
                if (_shipPortraits.ContainsKey(player1Ship.Id))
                {
                    var texture = _shipPortraits[player1Ship.Id];
                    Raylib.DrawTextureEx(texture, new Vector2(50, 80), 0, 0.5f, Color.White);
                }
                DrawTextEx(player1Ship.Name,new Vector2( 50, 130), 16,1, Color.White);
            }
            
            if (player2Ship != null)
            {
                DrawTextEx("P2 Ship:",new Vector2( _screenWidth - 150, 50), 20,1, Color.Red);
                if (_shipPortraits.ContainsKey(player2Ship.Id))
                {
                    var texture = _shipPortraits[player2Ship.Id];
                    Raylib.DrawTextureEx(texture, new Vector2(_screenWidth - 150, 80), 0, 0.5f, Color.White);
                }
                DrawTextEx(player2Ship.Name,new Vector2( _screenWidth - 150, 130), 16,1, Color.White);
            }
        }

        public void DrawFinalSelections(PlayerSelection player1, PlayerSelection player2)
        {
            int centerX = _screenWidth / 2;
            int centerY = _screenHeight / 2;
            
            // Player 1 final selection
            if (player1.SelectedCharacter != null && player1.SelectedEffect != null)
            {
                DrawTextEx("PLAYER 1",new Vector2( centerX - 415, centerY - 200), 36,1, Color.Blue);
                
                // Ship
                if (_shipPortraits.ContainsKey(player1.SelectedCharacter.Id))
                {
                    var texture = _shipPortraits[player1.SelectedCharacter.Id];
                    Raylib.DrawTexture(texture, centerX - 400, centerY - 150, Color.White);
                }
                DrawTextEx(player1.SelectedCharacter.Name,new Vector2( centerX - 400, centerY - 40), 24,1, Color.White);
                
                // Effect
                //Raylib.DrawRectangle(centerX - 400, centerY + 20, 80, 80, new Color(60, 60, 60, 255));
                //Raylib.DrawRectangleLines(centerX - 400, centerY + 20, 80, 80, Color.Blue);
                //DrawTextEx("FX",new Vector2( centerX - 375, centerY + 50), 20,1, Color.Blue);
                //DrawTextEx(player1.SelectedEffect.Name,new Vector2( centerX - 390, centerY + 110), 22,1, Color.White);
                // Draw effect icon placeholder
                Raylib.DrawTexturePro(_effectIcons[player1.SelectedEffect.IconPath],
                    new Rectangle(0, 0, _effectIcons[player1.SelectedEffect.IconPath].Width, _effectIcons[player1.SelectedEffect.IconPath].Height),
                    new Rectangle(centerX - 390, centerY + 20, 80, 80), new Vector2(0, 0), 0f, Color.White);// new Vector2(startX, startY), 0f, .33f, Color.White);

            }

            // Player 2 final selection
            if (player2.SelectedCharacter != null && player2.SelectedEffect != null)
            {
                DrawTextEx("PLAYER 2",new Vector2( centerX + 270, centerY - 200), 36,1, Color.Red);
                
                // Ship
                if (_shipPortraits.ContainsKey(player2.SelectedCharacter.Id))
                {
                    var texture = _shipPortraits[player2.SelectedCharacter.Id];
                    Raylib.DrawTexture(texture, centerX + 290, centerY - 150, Color.White);
                }
                DrawTextEx(player2.SelectedCharacter.Name,new Vector2( centerX + 290, centerY - 40), 22,1, Color.White);

                // Effect
                //Raylib.DrawRectangle(centerX + 200, centerY + 20, 80, 80, new Color(60, 60, 60, 255));
                //Raylib.DrawRectangleLines(centerX + 200, centerY + 20, 80, 80, Color.Red);
                //DrawTextEx("FX",new Vector2( centerX + 225, centerY + 50), 20,1, Color.Red);
                //DrawTextEx(player2.SelectedEffect.Name,new Vector2( centerX + 200, centerY + 110), 22,1, Color.White);
                Raylib.DrawTexturePro(_effectIcons[player2.SelectedEffect.IconPath],
                    new Rectangle(0, 0, _effectIcons[player2.SelectedEffect.IconPath].Width, _effectIcons[player1.SelectedEffect.IconPath].Height),
                    new Rectangle(centerX + 300, centerY + 20, 80, 80), new Vector2(0, 0), 0f, Color.White);// new Vector2(startX, startY), 0f, .33f, Color.White);
            }
        }

        private void DrawStatsPreview(CharacterStats stats, int x, int y, Color color)
        {
            int fontSize = 14;
            Color statColor = new Color((int)color.R, (int)color.G, (int)color.B, 180);
            
            DrawTextEx($"HP:{stats.Health:F0} SPD:{stats.Speed:F1} FR:{stats.FireRate:F1} DMG:{stats.Damage:F1}",new Vector2( x, y), fontSize,1, statColor);
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
                    DrawTextEx(line,new Vector2( x, curY), fontSize,1, color);
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
                DrawTextEx(line,new Vector2( x, curY), fontSize,1, color);
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
            int fontSize = (int)(22 * uniformScale);
            for (int i = 0; i < instructions.Length; i++)
            {
                var textWidth = Raylib.MeasureTextEx(_font22, instructions[i], fontSize, 1f).X;
                float centerX = (_screenWidth * uniformScale) / 2f;
                int posX = (int)(centerX - textWidth / 2f);
                int posY = instructionY + (int)(i * 35 * uniformScale);
                DrawTextEx(instructions[i],new Vector2( posX, posY), fontSize,1, Color.LightGray);
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
            int fontSize = (int)(22 * uniformScale);
            for (int i = 0; i < instructions.Length; i++)
            {
                var textWidth = Raylib.MeasureTextEx(_font22, instructions[i], fontSize, 1f).X;
                float centerX = (_screenWidth * uniformScale) / 2f;
                int posX = (int)(centerX - textWidth / 2f);
                int posY = instructionY + (int)(i * 35 * uniformScale);
                Color color = i == 0 ? Color.Yellow : Color.LightGray;
                DrawTextEx(instructions[i],new Vector2( posX, posY), fontSize,1, color);
            }
        }

        public void DrawStartPrompt()
        {
            float uniformScale = GetUniformScale();
            string prompt = "BOTH PLAYERS READY - PRESS SPACE TO START";
            int fontSize = (int)(32 * uniformScale);
            Vector2 textSize = Raylib.MeasureTextEx(_textFont, prompt, fontSize, 1);
            Vector2 position = new Vector2((_screenWidth * uniformScale - textSize.X) / 3, _screenHeight * uniformScale - 100 * uniformScale);
            float pulse = (float)Math.Sin(Raylib.GetTime() * 4) * 0.3f + 0.7f;
            Color promptColor = new Color((int)(255 * pulse), (int)(255 * pulse), 0, 255);
            DrawTextEx(prompt, position, fontSize, 1, promptColor);
        }

        public void Cleanup()
        {
            // Unload all ship portrait textures
            foreach (var texture in _shipPortraits.Values)
            {
                Raylib.UnloadTexture(texture);
            }
            _shipPortraits.Clear();
            Raylib.UnloadFont(_font14);
            Raylib.UnloadFont(_font16);
            Raylib.UnloadFont(_font18);
            Raylib.UnloadFont(_font22);
            Raylib.UnloadFont(_font28);
            Raylib.UnloadFont(_font40);
            Raylib.UnloadFont(_font72);
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

        private void DrawTextEx(string prompt, Vector2 position, float fontSize, float fontSpacing, Color color)
        {
            Font font;
            if (fontSize >= 72)
                font = _font72;
            else if (fontSize >= 40)
                font = _font40;
            else if (fontSize >= 28)
                font = _font28;
            else if (fontSize >= 22)
                font = _font22;
            else if (fontSize >= 18)
                font = _font18;
            else if (fontSize >= 16)
                font = _font16;
            else if (fontSize >= 14)
                font = _font14;
            else
                font = _font14;

                Raylib.DrawTextEx(font, prompt, position, fontSize, fontSpacing, color);
        }
    }
}