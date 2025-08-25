using System;
using System.Collections.Generic;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.CharacterScreen.Utilities
{
    public static class TextUtility
    {
        private static Font _font14;
        private static Font _font16;
        private static Font _font18;
        private static Font _font22;
        private static Font _font28;
        private static Font _font40;
        private static Font _font72;
        private static bool _fontsLoaded = false;

        public static void InitializeFonts()
        {
            if (_fontsLoaded) return;
            _font14 = Raylib.LoadFontEx("Fonts/Roboto-Regular.ttf", 14, [], 0);
            _font16 = Raylib.LoadFontEx("Fonts/Roboto-Regular.ttf", 16, [], 0);
            _font18 = Raylib.LoadFontEx("Fonts/Roboto-Regular.ttf", 18, [], 0);
            _font22 = Raylib.LoadFontEx("Fonts/Roboto-Regular.ttf", 22, [], 0);
            _font28 = Raylib.LoadFontEx("Fonts/Roboto-Regular.ttf", 28, [], 0);
            _font40 = Raylib.LoadFontEx("Fonts/Roboto-Regular.ttf", 40, [], 0);
            _font72 = Raylib.LoadFontEx("Fonts/Roboto-Regular.ttf", 72, [], 0);
            Raylib.SetTextureFilter(_font14.Texture, TextureFilter.Point);
            Raylib.SetTextureFilter(_font16.Texture, TextureFilter.Point);
            Raylib.SetTextureFilter(_font18.Texture, TextureFilter.Point);
            Raylib.SetTextureFilter(_font22.Texture, TextureFilter.Point);
            Raylib.SetTextureFilter(_font28.Texture, TextureFilter.Point);
            Raylib.SetTextureFilter(_font40.Texture, TextureFilter.Point);
            Raylib.SetTextureFilter(_font72.Texture, TextureFilter.Point);
            _fontsLoaded = true;
        }

        public static void UnloadFonts()
        {
            if (!_fontsLoaded) return;
            Raylib.UnloadFont(_font14);
            Raylib.UnloadFont(_font16);
            Raylib.UnloadFont(_font18);
            Raylib.UnloadFont(_font22);
            Raylib.UnloadFont(_font28);
            Raylib.UnloadFont(_font40);
            Raylib.UnloadFont(_font72);
            _fontsLoaded = false;
        }

        public static void DrawWrappedText(string text, int x, int y, int maxWidth, float fontSize, Color color)
        {
            string[] words = text.Split(' ');
            string line = "";
            int lineHeight = (int)fontSize + 2;
            int curY = y;
            foreach (var word in words)
            {
                string testLine = line.Length == 0 ? word : line + " " + word;
                int testWidth = Raylib.MeasureText(testLine, (int)fontSize);
                if (testWidth > maxWidth && line.Length > 0)
                {
                    DrawTextAutoFont(line, new Vector2(x, curY), fontSize, 1, color);
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
                DrawTextAutoFont(line, new Vector2(x, curY), fontSize, 1, color);
            }
        }

        public static void DrawTextAutoFont(string text, Vector2 position, float fontSize, float fontSpacing, Color color)
        {
            if (!_fontsLoaded) InitializeFonts();
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

            Raylib.DrawTextEx(font, text, position, fontSize, fontSpacing, color);
        }

        public static Font GetFont(float fontSize)
        {
            if (!_fontsLoaded) InitializeFonts();
            if (fontSize >= 72)
                return _font72;
            else if (fontSize >= 40)
                return _font40;
            else if (fontSize >= 28)
                return _font28;
            else if (fontSize >= 22)
                return _font22;
            else if (fontSize >= 18)
                return _font18;
            else if (fontSize >= 16)
                return _font16;
            else if (fontSize >= 14)
                return _font14;
            else
                return _font14;
        }

        public static Color Desaturate(Color color, float saturation)
        {
            float h, s, v;
            ColorExtensions.RgbToHsv(color.R, color.G, color.B, out h, out s, out v);
            s = saturation;
            byte r, g, b;
            ColorExtensions.HsvToRgb(h, s, v, out r, out g, out b);
            return new Color(r, g, b, color.A);
        }
    }
}
