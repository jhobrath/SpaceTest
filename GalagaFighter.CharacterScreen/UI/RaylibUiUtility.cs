using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.CharacterScreen.UI
{
    public static class RaylibUiUtility
    {
        public static void DrawTextureCentered(Texture2D texture, Vector2 center, int width, int height, float rotation, Color color)
        {
            Rectangle src = new Rectangle(0, 0, texture.Width, texture.Height);
            Rectangle dest = new Rectangle(center.X, center.Y, width, height);
            Vector2 origin = new Vector2(width / 2f, height / 2f);
            Raylib.DrawTexturePro(texture, src, dest, origin, rotation, color);
        }

        public static void DrawTextCentered(Font font, string text, Vector2 center, float fontSize, float fontSpacing, Color color)
        {
            Vector2 textSize = Raylib.MeasureTextEx(font, text, fontSize, fontSpacing);
            Vector2 position = new Vector2(center.X - textSize.X / 2f, center.Y - textSize.Y / 2f);
            Raylib.DrawTextEx(font, text, position, fontSize, fontSpacing, color);
        }

        public static void DrawRectangleCentered(Vector2 center, int width, int height, Color color)
        {
            int x = (int)(center.X - width / 2f);
            int y = (int)(center.Y - height / 2f);
            Raylib.DrawRectangle(x, y, width, height, color);
        }
    }
}
