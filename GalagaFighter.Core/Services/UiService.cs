using GalagaFighter.Core.Models.Players;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public static class UiService
    {
        public static void DrawUi(Player player1, Player player2)
        {
            int healthTextSize = (int)(24 * Game.UniformScale);
            int controlTextSize = (int)(20 * Game.UniformScale);
            int winnerTextSize = (int)(50 * Game.UniformScale);
            int statusTextSize = (int)(16 * Game.UniformScale);
            int margin = (int)(15 * Game.UniformScale);

            var player1Health = player1.Health;
            var player2Health = player2.Health;

            // Health and bullet capacity display
            Raylib.DrawText($"P1 Health: {player1Health}", margin, margin, healthTextSize, Color.White);
            Raylib.DrawText($"P2 Health: {player2Health}", (int)Game.Width - (int)(250 * Game.UniformScale), margin, healthTextSize, Color.White);

            // Bullet capacity display
            int bulletStatusY = margin + (int)(30 * Game.UniformScale);

            // Winner display
            if (player1.Health <= 0 || player2.Health <= 0)
            {
                string winner = player1.Health > 0 ? "Player 1 Wins!" : "Player 2 Wins!";
                Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), winner, winnerTextSize, 1);
                Raylib.DrawText(winner, (int)(Game.Width / 2 - textSize.X / 2), (int)(Game.Width / 2 - textSize.Y / 2), winnerTextSize, Color.Gold);
            }
        }
    }
}
