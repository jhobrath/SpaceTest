using GalagaFighter.Core.Handlers.Players;
using GalagaFighter.Core.Models.Effects.Projectiles;
using GalagaFighter.Core.Models.Effects.Statuses;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Linq;
using System.Numerics;

namespace GalagaFighter.Core.Static
{
    public static class UiService
    {
        private static int _healthTextSize = (int)(24 * Game.UniformScale);
        private static int _controlTextSize = (int)(20 * Game.UniformScale);
        private static int _statusTextSize = (int)(16 * Game.UniformScale);
        private static int _margin = (int)(15 * Game.UniformScale);
        private static IPlayerManagerFactory _playerManagerFactory = Registry.Get<IPlayerManagerFactory>();

        public static void Initialize()
        {
        }

        public static void DrawUi(Player player1, Player player2)
        {
            DrawPlayerHealth(player1, false);
            DrawPlayerHealth(player2, true);
            DrawPlayerResources(player1, false);
            DrawPlayerResources(player2, true);
            DrawWinner(player1, player2);
            DrawEffects(player1, false);
            DrawEffects(player2, true);
#if DEBUG
            DrawPlayerDebugInfo(player1, false);
            DrawPlayerDebugInfo(player2, true);
#endif
        }

        private static void DrawPlayerDebugInfo(Player player, bool reverse)
        {
            // Position debug info near health bar
            var healthBarX = (int)(((reverse 
                ? Game.Width - (_margin + (player.Health * 500 / 100f)) 
                : _margin))*Game.UniformScale);
            var healthBarY = _margin + 35; // Just below health bar
            var healthBarPosition = new Vector2(healthBarX, healthBarY);

#if DEBUG
            DebugWriter.DrawPlayerDebug(player, healthBarPosition);
#endif
        }

        private static void DrawEffects(Player player, bool reverse)
        {
            var effectManager = (IExposedPlayerEffectManager)_playerManagerFactory.GetEffectManager(player);
            var effects = effectManager.Effects;
            var statusEffects = effects.Where(x => !x.IsProjectile);
            var projectiles = effects.Where(x => x.IsProjectile); 

            var iconSize = 30f * Game.UniformScale;
            var startX = reverse
                ? Game.Width - (_margin + iconSize * 6)
                : _margin;

            var start = new Vector2(0f, iconSize);
            var iconVec = new Vector2(iconSize, iconSize);

            var fireRate = statusEffects.Where(x => x.GetType() == typeof(FireRateEffect)).ToList();
            var icons = statusEffects.Concat(projectiles).Except(fireRate).Where(x => x.GetType() != typeof(DefaultShootEffect)).Select(x => x.IconPath).ToList();

            icons.Insert(0, "Sprites/Effects/firerate" + (fireRate.Count+1) + ".png");

            var selected = effectManager.SelectedProjectile;
            var isDefaultEffect = selected != null && selected.GetType() == typeof(DefaultShootEffect);
            
            for(var i =0;i < icons.Count;i++)
            {
                var col = (reverse ? ((6 - (i % 6))-1) : i % 6);
                var row = (int)Math.Floor(i / 6f);

                var texture = new SpriteWrapper(TextureService.Get(icons[i]));
                // Move icons down by adding extra space for resource bar (30 pixels + some padding)
                var position = new Vector2(startX + col * iconSize, _margin + iconSize + row * iconSize + iconSize / 2 + 40);
                var center = new Vector2(position.X + iconSize / 2, position.Y + iconSize / 2);
                texture.Draw(center, 0f, iconSize, iconSize, Color.White);
                if(
                    (isDefaultEffect && i == 0) ||
                    (selected != null && icons[i] == selected.IconPath)
                )
                {
                    Raylib.DrawRectangleLines((int)position.X, (int)position.Y, (int)iconVec.X, (int)iconVec.Y, Color.LightGray);
                }

            }
        }

        public static void DrawPlayerHealth(Player player, bool reverse)
        {
            var baseWidth = 500;
            var baseHeight = 30;

            var remainingHealthPercentage = player.Health / 100f;

            var remainingHealthStartX = GetUiResourceStartX(remainingHealthPercentage, baseWidth, reverse);
            var healthBarLinesStart = GetUiResourceStartX(1f, baseWidth, reverse);

            Raylib.DrawRectangle(remainingHealthStartX, _margin, (int)(remainingHealthPercentage * baseWidth * Game.UniformScale), baseHeight, Color.Red);
            Raylib.DrawRectangleLines(healthBarLinesStart, _margin, (int)(baseWidth * Game.UniformScale), baseHeight, Color.White);
        }

        public static void DrawPlayerResources(Player player, bool reverse)
        {
            var baseWidth = 500;

            var resourceManager = _playerManagerFactory.GetResourceManager(player);
            var currentResources = resourceManager.ShieldMeter;
            var maxResources = PlayerResourceManager.MaxAmount;
            var resourcePercentage = currentResources / maxResources;

            var remainingResourceStartX = GetUiResourceStartX(resourcePercentage, baseWidth, reverse);
            var resourceBarLinesStart = GetUiResourceStartX(1f, baseWidth, reverse);

            // Draw ShieldMeter bar directly under health bar (health bar is at _margin, so resource bar is at _margin + 5)
            var resourceBarY = _margin + 30 + 5; // 30 for health bar height, 5 for spacing
            Raylib.DrawRectangle(remainingResourceStartX, resourceBarY, (int)(resourcePercentage* baseWidth * Game.UniformScale), 10, Color.Blue);
            Raylib.DrawRectangleLines(resourceBarLinesStart, resourceBarY, (int)(baseWidth * Game.UniformScale), 10, Color.White);

            // Draw ShootMeter bar below resource bar
            var shootMeterPercentage = resourceManager.ShootMeter; // 0.0 to 1.0
            var shootMeterStartX = GetUiResourceStartX(shootMeterPercentage, baseWidth, reverse);

            var shootMeterBarY = resourceBarY + 10 + 5; // 10 for shield bar height, 5 for spacing
            Raylib.DrawRectangle(shootMeterStartX, shootMeterBarY, (int)(shootMeterPercentage*baseWidth *Game.UniformScale), 10, Color.Lime);
            Raylib.DrawRectangleLines(resourceBarLinesStart, shootMeterBarY, (int)(baseWidth * Game.UniformScale), 10, Color.White);
        }

        public static void DrawWinner(Player player1, Player player2)
        {
            int winnerTextSize = (int)(50 * Game.UniformScale);

            // Winner display
            if (player1.Health <= 0 || player2.Health <= 0)
            {
                string winner = player1.Health > 0 ? "Player 1 Wins!" : "Player 2 Wins!";
                Vector2 textSize = Raylib.MeasureTextEx(Raylib.GetFontDefault(), winner, winnerTextSize, 1);
                Raylib.DrawText(winner, (int)(Game.Width / 2 - textSize.X / 2), (int)(Game.Width / 2 - textSize.Y / 2), winnerTextSize, Color.Gold);
            }
        }

        private static int GetUiResourceStartX(float percentage, int baseSize, bool reverse)
        {
            return (int)((reverse 
                ? Game.Width - ((_margin + ((percentage) * baseSize)) * Game.UniformScale)
                : _margin * Game.UniformScale));
        }
    }
}
