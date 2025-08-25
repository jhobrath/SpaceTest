using System;
using System.Numerics;
using Raylib_cs;
using GalagaFighter.CharacterScreen.Models;

namespace GalagaFighter.CharacterScreen.UI
{
    public class SelectionDrawHelper
    {
        public static void DrawIndicator(int x, int y, int size, Color fillColor, bool isSelected, Color borderColor)
        {
            Raylib.DrawRectangle(x, y, size, size, fillColor);
            if (isSelected)
            {
                Raylib.DrawRectangleLines(x, y, size, size, borderColor);
            }
        }

        public static void DrawReadyText(string text, Vector2 position, int fontSize, Font font, Color color)
        {
            Raylib.DrawTextEx(font, text, position, fontSize, 1, color);
        }

        public static void DrawIcon(Texture2D icon, Rectangle src, Rectangle dest, Vector2 origin, float rotation, Color color)
        {
            Raylib.DrawTexturePro(icon, src, dest, origin, rotation, color);
        }
    }
}
