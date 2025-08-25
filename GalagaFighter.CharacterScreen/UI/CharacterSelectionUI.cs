using GalagaFighter.CharacterScreen.Models;
using GalagaFighter.CharacterScreen.Services;
using GalagaFighter.CharacterScreen.UI;
using GalagaFighter.CharacterScreen.Utilities;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using Character = GalagaFighter.CharacterScreen.Models.Character;

namespace GalagaFighter.CharacterScreen.UI
{
    public class CharacterSelectionUI
    {
        private readonly int _screenWidth = 1920;
        private readonly int _screenHeight = 1080;
        private readonly Dictionary<string, Texture2D> _shipPortraits;
        private static Dictionary<string, Texture2D> _effectIcons = new Dictionary<string, Texture2D>();
        private readonly SelectionLayoutHelper _layoutHelper;
        private readonly SelectionDrawHelper _drawHelper;

        public CharacterSelectionUI()
        {
            TextUtility.InitializeFonts();
            _shipPortraits = new Dictionary<string, Texture2D>();
            _layoutHelper = new SelectionLayoutHelper(_screenWidth, _screenHeight, GetUniformScale());
            _drawHelper = new SelectionDrawHelper();
            LoadEffectIcons();
        }

        private void LoadEffectIcons()
        {
            var effectService = new EffectService();
            var effects = effectService.GetAvailableEffects();
            foreach(var effect in effects)
            {
                _effectIcons.Add(effect.IconPath, Raylib.LoadTexture(effect.IconPath));
            }
        }

        public void PreloadShipSprites(List<Models.Character> characters)
        {
            float uniformScale = GetUniformScale();
            int shipHeight = (int)(140 * uniformScale);
            int targetSize = shipHeight + 10; // This matches what we actually draw
            
            foreach (var character in characters)
            {
                if (!_shipPortraits.ContainsKey(character.Id))
                {
                    // Use the cached texture approach with the exact target size
                    _shipPortraits[character.Id] = ShipSpriteService.GetShipTexture(
                        character.ShipTintColor, 
                        targetSize,
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
            var textDimensions = Raylib.MeasureTextEx(TextUtility.GetFont(fontSize), title, fontSize, 1);
            float centerX = (_screenWidth * uniformScale) / 2f;
            int posX = (int)(centerX - textDimensions.X/2);
            int posY = (int)(50 * uniformScale);
            TextUtility.DrawTextAutoFont(title, new Vector2(posX + 2, posY + 2), fontSize, 1, Color.DarkBlue);
            TextUtility.DrawTextAutoFont(title, new Vector2(posX, posY), fontSize, 1, Color.White);
        }

        public void DrawPlayerShipSelection(List<Models.Character> characters, int selectedIndex, Models.Character? selection, bool isReady, bool isPlayer1)
        {
            DrawPlayerShipSelectionInternal(characters, selectedIndex, selection, isReady, isPlayer1);
        }

        public void DrawPlayerEffectSelection(List<OffensiveEffect> effects, int selectedIndex, OffensiveEffect? selection, bool isReady, bool isPlayer1)
        {
            DrawPlayerEffectSelectionInternal(effects, selectedIndex, selection, isReady, isPlayer1);
        }

        private void DrawPlayerShipSelectionInternal(List<Models.Character> characters, int selectedIndex, Models.Character? selection, bool isReady, bool isPlayer1)
        {
            float uniformScale = GetUniformScale();
            var (startX, centerY, itemSpacing) = _layoutHelper.GetShipPanelLayout(isPlayer1);
            string playerLabel = isPlayer1 ? "PLAYER 1" : "PLAYER 2";
            int labelFontSize = (int)(40 * uniformScale);
            Color playerColor = isPlayer1 ? Color.Blue : Color.Red;
            TextUtility.DrawTextAutoFont(playerLabel, new Vector2(startX + 150 + (isPlayer1 ? 0 : 50), (int)(200 * uniformScale)), labelFontSize, 1, playerColor);

            int count = characters.Count;
            int prevIndex = (selectedIndex - 1 + count) % count;
            int nextIndex = (selectedIndex + 1) % count;
            int shipWidth = (int)(220 * uniformScale);
            int shipHeight = (int)(140 * uniformScale);

            DrawShipItem(characters[prevIndex], startX, centerY - itemSpacing, false, 0.4f, playerColor, selection, isReady, false, shipWidth, shipHeight, uniformScale);
            DrawShipItem(characters[selectedIndex], startX, centerY, true, 1.0f, playerColor, selection, isReady, true, shipWidth, shipHeight, uniformScale);
            DrawShipItem(characters[nextIndex], startX, centerY + itemSpacing, false, 0.4f, playerColor, selection, isReady, false, shipWidth, shipHeight, uniformScale);

            var (indicatorX, indicatorStartY, indicatorSize, indicatorSpacing) = _layoutHelper.GetIndicatorLayout(isPlayer1, shipWidth, itemSpacing, centerY);
            for (int i = 0; i < count; i++)
            {
                int y = indicatorStartY + i * indicatorSpacing;
                Color baseColor = characters[i].ShipTintColor;
                bool isSelectedIndicator = (i == selectedIndex);
                Color fillColor = isSelectedIndicator
                    ? MakeColor(baseColor, 0.84f)
                    : MakeColor(TextUtility.Desaturate(baseColor, 0.25f), 0.47f);
                Color borderColor = isPlayer1 ? Color.Blue : Color.Red;
                SelectionDrawHelper.DrawIndicator(indicatorX, y, indicatorSize, fillColor, isSelectedIndicator, borderColor);
            }

            if (isReady && selection != null)
            {
                string readyText = "READY!";
                Vector2 readySize = Raylib.MeasureTextEx(TextUtility.GetFont(32 * uniformScale), readyText, (int)(32 * uniformScale), 1);
                Vector2 readyPos = new Vector2(startX + (shipWidth - readySize.X) / 2, centerY + (int)(itemSpacing * 1.1f));
                TextUtility.DrawTextAutoFont(readyText, readyPos, (int)(32 * uniformScale), 1, Color.Green);
            }
        }

        private void DrawShipItem(Models.Character character, int x, int y, bool isSelected, float alpha, Color playerColor, Models.Character? selection, bool isReady, bool isCenter, int shipWidth, int shipHeight, float uniformScale)
        {
            Color fadedColor = MakeColor(255, 255, 255, alpha);
            Color textColor = isSelected ? playerColor : Color.White;
            Color bgColor = isSelected ? MakeColor(playerColor, 0.31f * alpha) : MakeColor(255, 255, 255, 0.12f * alpha);

            int fillPad = (int)(8 * uniformScale);
            int extraPad = (int)(60 * uniformScale);
            int textOffset = (int)(18 * uniformScale);
            int textWidth = (int)(180 * uniformScale);
            int fillWidth = shipWidth + textOffset + textWidth + extraPad + fillPad * 2;
            int fillHeight = shipHeight + fillPad * 2;
            Vector2 fillCenter = new Vector2(x + shipWidth / 2f + textOffset + textWidth / 2f + extraPad / 2f, y + shipHeight / 2f);
            RaylibUiUtility.DrawRectangleCentered(fillCenter, fillWidth, fillHeight, bgColor);

            Raylib.DrawRectangleLines(x, y, shipWidth, shipHeight, fadedColor);

            if (_shipPortraits.ContainsKey(character.Id))
            {
                var shipTexture = _shipPortraits[character.Id];
                RaylibUiUtility.DrawTextureCentered(
                    shipTexture,
                    new Vector2(x + shipWidth / 2f, y + shipHeight / 2f + 5),
                    shipHeight+10,
                    shipHeight+10,
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

            TextUtility.DrawTextAutoFont(
                character.Name,
                new Vector2(x + shipWidth + (int)(18 * uniformScale), y + (int)(8 * uniformScale)),
                (int)(22 * uniformScale),
                1,
                textColor
            );
            TextUtility.DrawTextAutoFont(
                $"Type: {character.Type}",
                new Vector2(x + shipWidth + (int)(18 * uniformScale), y + (int)(32 * uniformScale)),
                (int)(16 * uniformScale),
                1,
                Color.LightGray
            );
            DrawStatsPreview(character.Stats, x + shipWidth + (int)(18 * uniformScale), y + (int)(54 * uniformScale), textColor);
            DrawWrappedText(character.Description, x + shipWidth + (int)(18 * uniformScale), y + (int)(76 * uniformScale), (int)(180 * uniformScale), (int)(18 * uniformScale), Color.LightGray);
        }

        private void DrawPlayerEffectSelectionInternal(List<OffensiveEffect> effects, int selectedIndex, OffensiveEffect? selection, bool isReady, bool isPlayer1)
        {
            var (startX, startY) = _layoutHelper.GetEffectPanelLayout(isPlayer1);
            int labelFontSize = 36;
            Color playerColor = isPlayer1 ? Color.Blue : Color.Red;
            TextUtility.DrawTextAutoFont(isPlayer1 ? "PLAYER 1" : "PLAYER 2", new Vector2(startX, startY), labelFontSize, 1, playerColor);

            for (int i = 0; i < effects.Count; i++)
            {
                var effect = effects[i];
                int itemY = startY + 80 + (i * 120);
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
                if (backgroundColor.A > 0)
                {
                    Raylib.DrawRectangle(startX - 10, itemY - 10, 400, 120, backgroundColor);
                }
                SelectionDrawHelper.DrawIcon(_effectIcons[effect.IconPath],
                    new Rectangle(0, 0, _effectIcons[effect.IconPath].Width, _effectIcons[effect.IconPath].Height),
                    new Rectangle(startX, itemY, 80, 80), new Vector2(0, 0), 0f, Color.White);
                TextUtility.DrawTextAutoFont(effect.Name, new Vector2(startX + 90, itemY), 22, 1, textColor);
                TextUtility.DrawTextAutoFont($"Category: {effect.Category}", new Vector2(startX + 90, itemY + 25), 16, 1, Color.LightGray);
                DrawWrappedText(effect.Description, startX + 90, itemY + 50, 280, 14, Color.LightGray);
            }
            if (isReady && selection != null)
            {
                string readyText = "EFFECT READY!";
                Vector2 readySize = Raylib.MeasureTextEx(TextUtility.GetFont(32),  readyText, 32, 1);
                Vector2 readyPos = new Vector2(startX + (380 - readySize.X) / 2, startY + 80 + (effects.Count * 120) + 20);
                TextUtility.DrawTextAutoFont(readyText, readyPos, 32, 1, Color.Green);
            }
        }

        public void DrawSelectedShips(Models.Character? player1Ship, Models.Character? player2Ship)
        {
            if (player1Ship != null)
            {
                TextUtility.DrawTextAutoFont("P1 Ship:", new Vector2(50, 50), 20, 1, Color.Blue);
                if (_shipPortraits.ContainsKey(player1Ship.Id))
                {
                    var texture = _shipPortraits[player1Ship.Id];
                    Raylib.DrawTextureEx(texture, new Vector2(50, 80), 0, 0.5f, Color.White);
                }
                TextUtility.DrawTextAutoFont(player1Ship.Name, new Vector2(50, 130), 16, 1, Color.White);
            }
            if (player2Ship != null)
            {
                TextUtility.DrawTextAutoFont("P2 Ship:", new Vector2(_screenWidth - 150, 50), 20, 1, Color.Red);
                if (_shipPortraits.ContainsKey(player2Ship.Id))
                {
                    var texture = _shipPortraits[player2Ship.Id];
                    Raylib.DrawTextureEx(texture, new Vector2(_screenWidth - 150, 80), 0, 0.5f, Color.White);
                }
                TextUtility.DrawTextAutoFont(player2Ship.Name, new Vector2(_screenWidth - 150, 130), 16, 1, Color.White);
            }
        }

        public void DrawFinalSelections(PlayerSelection player1, PlayerSelection player2)
        {
            int centerX = _screenWidth / 2;
            int centerY = _screenHeight / 2;
            if (player1.SelectedCharacter != null && player1.SelectedEffect != null)
            {
                TextUtility.DrawTextAutoFont("PLAYER 1", new Vector2(centerX - 415, centerY - 200), 36, 1, Color.Blue);
                if (_shipPortraits.ContainsKey(player1.SelectedCharacter.Id))
                {
                    var texture = _shipPortraits[player1.SelectedCharacter.Id];
                    Raylib.DrawTexture(texture, centerX - 400, centerY - 150, Color.White);
                }
                TextUtility.DrawTextAutoFont(player1.SelectedCharacter.Name, new Vector2(centerX - 400, centerY - 40), 24, 1, Color.White);
                Raylib.DrawTexturePro(_effectIcons[player1.SelectedEffect.IconPath],
                    new Rectangle(0, 0, _effectIcons[player1.SelectedEffect.IconPath].Width, _effectIcons[player1.SelectedEffect.IconPath].Height),
                    new Rectangle(centerX - 390, centerY + 20, 80, 80), new Vector2(0, 0), 0f, Color.White);
            }
            if (player2.SelectedCharacter != null && player2.SelectedEffect != null)
            {
                TextUtility.DrawTextAutoFont("PLAYER 2", new Vector2(centerX + 270, centerY - 200), 36, 1, Color.Red);
                if (_shipPortraits.ContainsKey(player2.SelectedCharacter.Id))
                {
                    var texture = _shipPortraits[player2.SelectedCharacter.Id];
                    Raylib.DrawTexture(texture, centerX + 290, centerY - 150, Color.White);
                }
                TextUtility.DrawTextAutoFont(player2.SelectedCharacter.Name, new Vector2(centerX + 290, centerY - 40), 22, 1, Color.White);
                Raylib.DrawTexturePro(_effectIcons[player2.SelectedEffect.IconPath],
                    new Rectangle(0, 0, _effectIcons[player2.SelectedEffect.IconPath].Width, _effectIcons[player1.SelectedEffect.IconPath].Height),
                    new Rectangle(centerX + 300, centerY + 20, 80, 80), new Vector2(0, 0), 0f, Color.White);
            }
        }

        private void DrawStatsPreview(CharacterStats stats, int x, int y, Color color)
        {
            int fontSize = 14;
            Color statColor = new Color((int)color.R, (int)color.G, (int)color.B, 180);
            TextUtility.DrawTextAutoFont($"HP:{stats.Health:F0} SPD:{stats.Speed:F1} FR:{stats.FireRate:F1} DMG:{stats.Damage:F1}", new Vector2(x, y), fontSize, 1, statColor);
        }

        private void DrawWrappedText(string text, int x, int y, int maxWidth, int fontSize, Color color)
        {
            TextUtility.DrawWrappedText(text, x, y, maxWidth, fontSize, color);
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
                var textWidth = Raylib.MeasureTextEx(TextUtility.GetFont(fontSize), instructions[i], fontSize, 1f).X;
                float centerX = (_screenWidth * uniformScale) / 2f;
                int posX = (int)(centerX - textWidth / 2f);
                int posY = instructionY + (int)(i * 35 * uniformScale);
                TextUtility.DrawTextAutoFont(instructions[i], new Vector2(posX, posY), fontSize, 1, Color.LightGray);
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
                var textWidth = Raylib.MeasureTextEx(TextUtility.GetFont(fontSize), instructions[i], fontSize, 1f).X;
                float centerX = (_screenWidth * uniformScale) / 2f;
                int posX = (int)(centerX - textWidth / 2f);
                int posY = instructionY + (int)(i * 35 * uniformScale);
                Color color = i == 0 ? Color.Yellow : Color.LightGray;
                TextUtility.DrawTextAutoFont(instructions[i], new Vector2(posX, posY), fontSize, 1, color);
            }
        }

        public void DrawStartPrompt()
        {
            float uniformScale = GetUniformScale();
            string prompt = "BOTH PLAYERS READY - PRESS SPACE TO START";
            int fontSize = (int)(32 * uniformScale);
            Vector2 textSize = Raylib.MeasureTextEx(TextUtility.GetFont(fontSize), prompt, fontSize, 1);
            Vector2 position = new Vector2((_screenWidth * uniformScale - textSize.X) / 3, _screenHeight * uniformScale - 100 * uniformScale);
            float pulse = (float)Math.Sin(Raylib.GetTime() * 4) * 0.3f + 0.7f;
            Color promptColor = new Color((int)(255 * pulse), (int)(255 * pulse), 0, 255);
            TextUtility.DrawTextAutoFont(prompt, position, fontSize, 1, promptColor);
        }

        public void Cleanup()
        {
            foreach (var texture in _shipPortraits.Values)
            {
                Raylib.UnloadTexture(texture);
            }
            _shipPortraits.Clear();
            TextUtility.UnloadFonts();
        }

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