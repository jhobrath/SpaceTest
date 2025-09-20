using GalagaFighter.Core.Handlers.Players;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Effects.Projectiles;
using GalagaFighter.Core.Models.Effects.Statuses;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GalagaFighter.Core.Static
{
    public static class UiService
    {
        private static int _healthTextSize = (int)(24 * Game.UniformScale);
        private static int _controlTextSize = (int)(20 * Game.UniformScale);
        private static int _statusTextSize = (int)(16 * Game.UniformScale);
        private static int _margin = (int)(30 * Game.UniformScale);
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
            DrawPlayerAugments(player1, false);
            DrawPlayerAugments(player2, true);
            DrawWinner(player1, player2);
            
            DrawProjectileEffects(player1, false, rowNumber: 0);
            DrawProjectileEffects(player2, true, rowNumber: 0);

            DrawStatusEffects(player1, false, rowNumber: 1);
            DrawStatusEffects(player2, true, rowNumber: 1);
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

        private class HudIcon { public string IconPath { get; set; } public int Count { get; set; } }

        private static void DrawProjectileEffects(Player player, bool reverse, float rowNumber)
        {
            var effectManager = (IExposedPlayerEffectManager)_playerManagerFactory.GetEffectManager(player);
            var projectileEffects = effectManager.Effects
                .Where(ef => ef.IsProjectile || ef is FireRateEffect)
                .GroupBy(x => x.GetType())
                .ToDictionary(x => x.Key, x => new HudIcon { Count = x.Count(), IconPath = x.First().IconPath });

            // Special handling for DefaultShoot + FireRate integration
            var defaultShoot = projectileEffects.Where(x => x.Key == typeof(DefaultShootEffect)).ToList();
            if (defaultShoot.Any())
            {
                var fireRateCount = projectileEffects.ContainsKey(typeof(FireRateEffect)) ? projectileEffects[typeof(FireRateEffect)].Count : 0;
                projectileEffects[typeof(DefaultShootEffect)].IconPath = "Sprites/Effects/Projectiles/DefaultShoot" + (fireRateCount + 1) + ".png";
                projectileEffects[typeof(DefaultShootEffect)].Count = 1;
                projectileEffects.Remove(typeof(FireRateEffect));
            }

            var selected = effectManager.SelectedProjectile;
            var isDefaultEffect = selected != null && selected.GetType() == typeof(DefaultShootEffect);
            
            // Determine which icons should be highlighted
            Func<string, int, bool> shouldHighlight = (iconPath, index) => 
                (isDefaultEffect && index == 0) || (selected != null && iconPath == selected.IconPath);

            DrawEffectIcons(projectileEffects, reverse, rowNumber, shouldHighlight);
        }

        private static void DrawStatusEffects(Player player, bool reverse, float rowNumber)
        {
            var effectManager = (IExposedPlayerEffectManager)_playerManagerFactory.GetEffectManager(player);
            var statusEffects = effectManager.Effects
                .Where(ef => ef is StatusEffect && !(ef is FireRateEffect))
                .GroupBy(x => x.GetType())
                .ToDictionary(x => x.Key, x => new HudIcon { Count = x.Count(), IconPath = x.First().IconPath });

            // No highlighting for status effects
            Func<string, int, bool> shouldHighlight = (iconPath, index) => false;

            DrawEffectIcons(statusEffects, reverse, rowNumber, shouldHighlight);
        }

        private static void DrawEffectIcons(
            Dictionary<Type, HudIcon> effects, 
            bool reverse, 
            float rowNumber, 
            Func<string, int, bool> shouldHighlight)
        {
            var slotSize = 50f * Game.UniformScale;
            var iconSize = slotSize - 6f * Game.UniformScale;
            var startX = reverse
                ? Game.Width - (_margin + slotSize * 12)
                : _margin;

            var slot = new Vector2(slotSize, slotSize);

            int i = 0;
            foreach (var key in effects.Keys)
            {
                var col = (reverse ? ((12 - (i % 12)) - 1) : i % 12);
                var row = (int)Math.Floor(i / 12f);

                var texture = new SpriteWrapper(TextureService.Get(effects[key].IconPath));
                var position = new Vector2(startX + col * slotSize, _margin + (rowNumber * slotSize) + row * slotSize + iconSize / 2 + slotSize - 10    );
                var center = new Vector2(position.X + slotSize / 2, position.Y + slotSize / 2);

                // Highlight if specified
                if (shouldHighlight(effects[key].IconPath, i))
                    Raylib.DrawRectangle((int)position.X + 1, (int)position.Y + 1, (int)slot.X - 2, (int)slot.Y - 2, Color.LightGray);

                texture.Draw(position, 0f, iconSize, iconSize, Color.White);

                // Show count for status effects (which can have multiple instances)
                var isStatusEffect = typeof(StatusEffect).IsAssignableFrom(key);
                if (isStatusEffect && effects[key].Count > 1)
                    Raylib.DrawText(effects[key].Count.ToString(), (int)position.X + 25, (int)position.Y + 25, 20, Color.White);

                i++;
            }
        }

        public static void DrawPlayerHealth(Player player, bool reverse)
        {
            var baseWidth = 500;
            var baseHeight = 30;

            var remainingHealthPercentage = player.Health / 100f;
            var remainingShieldPercentage = player.Shield / 100f;

            var remainingHealthStartX = GetUiResourceStartX(remainingHealthPercentage, baseWidth, reverse);
            var healthBarLinesStart = GetUiResourceStartX(1f, baseWidth, reverse);

            // Draw health bar (red)
            Raylib.DrawRectangle(remainingHealthStartX, _margin, (int)(remainingHealthPercentage * baseWidth * Game.UniformScale), baseHeight, Color.Red);

            // Draw shield bar (translucent silvery) over health
            var shieldColor = new Color(200, 200, 220, 120); // Silvery with alpha
            var remainingShieldStartX = GetUiResourceStartX(remainingShieldPercentage, baseWidth, reverse);
            Raylib.DrawRectangle(remainingShieldStartX, _margin, (int)(remainingShieldPercentage * baseWidth * Game.UniformScale), baseHeight, shieldColor);

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

        private static void DrawPlayerAugments(Player player, bool reverse)
        {
            var baseWidth = 500;
            var augmentHeight = 30 + 10 + 10; // Health bar (30) + shield bar (10) + shoot bar (10)
            var augmentSize = (int)(augmentHeight * Game.UniformScale);
            
            var effectManager = (IExposedPlayerEffectManager)_playerManagerFactory.GetEffectManager(player);
            var activeEffects = effectManager.Effects.ToList();
            
            // Calculate base position (end of the bars)
            var barEndX = reverse 
                ? (int)(Game.Width - ((_margin + baseWidth) * Game.UniformScale))
                : (int)((_margin + baseWidth) * Game.UniformScale);
            
            var augmentY = _margin;
            
            // Position augments next to bars, towards center
            var augmentStartX = reverse 
                ? barEndX - augmentSize // On left side (towards center)
                : barEndX; // On right side (towards center)
            
            int augmentIndex = 0;
            
            // Draw Offensive Augment (closest to bars)
            if (player.OffensiveAugment != null)
            {
                var sampleEffect = player.OffensiveAugment();
                var isActive = activeEffects.Any(e => e.GetType() == sampleEffect.GetType());
                var opacity = isActive ? 1.0f : 0.5f;
                
                var augmentX = reverse 
                    ? augmentStartX - (augmentIndex * augmentSize)
                    : augmentStartX + (augmentIndex * augmentSize);
                    
                DrawAugmentIcon(sampleEffect.IconPath, augmentX, augmentY, augmentSize, opacity);
                augmentIndex++;
            }
            
            // Draw Defensive Augment (next to offensive)
            if (player.DefensiveAugment != null)
            {
                var sampleEffect = player.DefensiveAugment();
                var isActive = activeEffects.Any(e => e.GetType() == sampleEffect.GetType());
                var opacity = isActive ? 1.0f : 0.5f;
                
                var augmentX = reverse 
                    ? augmentStartX - (augmentIndex * augmentSize)
                    : augmentStartX + (augmentIndex * augmentSize);
                    
                DrawAugmentIcon(sampleEffect.IconPath, augmentX, augmentY, augmentSize, opacity);
            }
        }

        private static void DrawAugmentIcon(string iconPath, int x, int y, int size, float opacity)
        {
            var texture = new SpriteWrapper(TextureService.Get(iconPath));
            var center = new Vector2(x + size / 2f, y + size / 2f);
            var color = Color.White;
            color.A = (byte)(255 * opacity);
            
            texture.Draw(new Vector2(x,y), 0f, size, size, color);
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
