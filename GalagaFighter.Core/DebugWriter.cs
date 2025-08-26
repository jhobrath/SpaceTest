using GalagaFighter.Core.Models.Players;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core
{
    public static class DebugWriter
    {
#if DEBUG
        public static void DrawPlayerDebug(Player player, Vector2 healthBarPosition)
        {
            return;
            var rect = player.Rect;
            var rotation = player.Rotation;
            string debugText = $"Size: {rect.Size}\nPos: {rect.Position}\nRot: {rotation:F2}";
            int fontSize = 12;
            int lineHeight = 14;
            int x = (int)healthBarPosition.X;
            int y = (int)healthBarPosition.Y + 20; // Offset below health bar
            foreach (var line in debugText.Split('\n'))
            {
                Raylib.DrawText(line, x, y, fontSize, Color.LightGray);
                y += lineHeight;
            }
        }

        internal static void Write(string text)
        {
            return;
            int fontSize = 12;
            int lineHeight = 14;
            int x = 400;
            int y = (int)(Game.Height - 100); // Offset below health bar
            foreach (var line in text.Split('\n'))
            {
                Raylib.DrawText(line, x, y, fontSize, Color.LightGray);
                y += lineHeight;
            }
        }
#endif
    }
}
