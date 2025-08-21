using GalagaFighter.Core.Models.Players;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Core
{
    public static class DebugWriter
    {
#if DEBUG
        public static void DrawPlayerDebug(Player player, Vector2 healthBarPosition)
        {
            var rect = player.CurrentFrameRect;
            var rotation = player.CurrentFrameRotation;
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
#endif
    }
}
