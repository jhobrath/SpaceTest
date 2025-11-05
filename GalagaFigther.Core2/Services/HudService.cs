using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core2.Services
{
    public interface IHudService
    {
        void Update();
    }

    public class HudService : IHudService
    {
        private readonly IObjectService _objectService;
        private readonly IGameDataRegistry _gameDataRegistry;

        public HudService(IObjectService objectService, IGameDataRegistry gameDataRegistry)
        {
            _objectService = objectService;
            _gameDataRegistry = gameDataRegistry;
        }

        public void Update()
        {
            var player1 = _objectService.Get<Player>(Game.Player1Id);
            var player2 = _objectService.Get<Player>(Game.Player2Id);

            RenderHealthBars(player1, player2); // Put this in the top-left corner and top right corner respectively
            RenderBaseStats(player1, player2); //Put Shield, Damage, Speed, FireRate (show firerate as 1/Firerate)
            RenderTurretIcons(player1, false);
            RenderTurretIcons(player2, true);
        }



        private void RenderBaseStats(Player player1, Player player2)
        {
            RenderPlayerBaseStats(player1, 50, false);
            RenderPlayerBaseStats(player2, 50, true);
        }

        private void RenderPlayerBaseStats(Player player, int yStart, bool isRightJustified)
        {
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);
            var baseStats = _gameDataRegistry.Get<PlayerBaseStats>(player);

            var speedValue = (baseStats.Speed * modifiers.Stats.SpeedMultiplier) ;
            var shield = baseStats.Shield * modifiers.Stats.ShieldMultiplier;
            var damage = baseStats.Damage * modifiers.Stats.DamageMultiplier;
            var fireRate = .15f/(baseStats.FireRate * modifiers.Stats.FireRateMultiplier) * 100;

            // Render Shield and Damage as a percentage (e.g. 120%)
            string shieldPct = ((shield / baseStats.Shield) * 100).ToString("0") + "%";
            string speedPct = ((speedValue / baseStats.Speed) * 100).ToString("0") + "%";
            string damagePct = ((damage / baseStats.Damage) * 100).ToString("0") + "%";

            // Prepare stat icons and values
            string[] statIconNames = { "Shield", "Damage", "Speed", "FireRate" };
            string[] statValues = {
                shieldPct,
                damagePct,
                speedPct,
                fireRate.ToString("0") + "%"
            };

            int statCount = statIconNames.Length;
            var gameState = _gameDataRegistry.Get<GalagaFighter.Core2.Models.Game.GameState>();
            var scale = gameState.UniformScale.X; // Assume X and Y are the same
            int barWidth = 500;
            int margin = (int)(30 * scale);
            int fontSize = (int)(20 * scale);
            int iconSize = (int)(fontSize * 1.5f); // Boost icon size to 1.5x the line height
            int spacing = (int)(barWidth * scale / statCount);
            int y = yStart + (int)(22 * scale); // Move icons down a bit to avoid overlap

            for (int i = 0; i < statCount; i++)
            {
                int x;
                if (!isRightJustified)
                    x = margin + i * spacing;
                else
                    x = (int)(gameState.ScreenSize.X - margin - barWidth * scale + i * spacing);

                // Center icon vertically with text
                int iconYOffset = (iconSize > fontSize) ? -(iconSize - fontSize) / 2 : 0;

                // Draw icon scaled to boosted size
                string iconPath = $"Sprites/Icons/{statIconNames[i]}.png";
                Texture2D icon = TextureCache.Get(iconPath);
                Rectangle src = new Rectangle(0, 0, icon.Width, icon.Height);
                Rectangle dest = new Rectangle(x, y + iconYOffset, iconSize, iconSize);
                Vector2 origin = Vector2.Zero;
                Raylib.DrawTexturePro(icon, src, dest, origin, 0f, Color.White);

                // Draw value next to icon, vertically centered
                Raylib.DrawText(statValues[i], x + iconSize + 4, y, fontSize, Color.White);
            }
        }

        private void RenderHealthBars(Player player1, Player player2)
        {
            RenderHealthBar(player1, 0, false);
            RenderHealthBar(player2, 0, true);
        }

        private void RenderHealthBar(Player player, int yStart, bool isRightJustified)
        {
            var gameState = _gameDataRegistry.Get<GalagaFighter.Core2.Models.Game.GameState>();
            var scale = gameState.UniformScale.X;
            int barWidth = 500;
            int barHeight = 30;
            int margin = (int)(30 * scale);
            int x = isRightJustified
                ? (int)(gameState.ScreenSize.X - margin - barWidth * scale)
                : margin;
            int y = yStart + margin;

            // Player health is not a property on Player, so we must get it from PlayerBaseStats and PlayerModifiers
            var baseStats = _gameDataRegistry.Get<PlayerBaseStats>(player);
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);
            float maxHealth = baseStats.Health * modifiers.Stats.HealthMultiplier;
            float currentHealth = player.Health;
            float overHealth = MathF.Max(0, currentHealth - maxHealth);
            float baseHealth = MathF.Min(currentHealth, maxHealth);

            // Draw background bar (gray)
            Raylib.DrawRectangle(x, y, (int)(barWidth * scale), barHeight, new Color(80, 80, 80, 255));

            // Draw overhealth (darker red, translucent) - not possible, always 0
            // Draw base health (red)
            if (baseHealth > 0)
            {
                int healthWidth = (int)((baseHealth / maxHealth) * barWidth * scale);
                Raylib.DrawRectangle(x, y, healthWidth, barHeight, new Color(200, 40, 40, 255));
            }

            if(overHealth > 0)
            {
                int healthWidth = (int)((overHealth / maxHealth) * barWidth * scale);
                Raylib.DrawRectangle(x, y, healthWidth, barHeight, new Color(100, 20, 20, 200));
            }

            // Draw border
            Raylib.DrawRectangleLines(x, y, (int)(barWidth * scale), barHeight, new Color(255,255,255,255));

            // Draw health text
            int fontSize = (int)(20 * scale);
            string healthText = $"HP: {currentHealth:0.0}";
            Raylib.DrawText(healthText, x + 8, y + 4, fontSize, new Color(255,255,255,255));
        }

        private void RenderTurretIcons(Player player, bool isRightJustified)
        {
            if (player?.Turrets == null || player.Turrets.Count == 0)
                return;

            var gameState = _gameDataRegistry.Get<GalagaFighter.Core2.Models.Game.GameState>();
            var scale = gameState.UniformScale.X;
            int iconSize = (int)(48 * scale);
            int margin = (int)(30 * scale);
            int spacing = (int)(iconSize * 1.1f);
            int count = player.Turrets.Count;
            int y = (int)(gameState.ScreenSize.Y - margin - iconSize); // Place at bottom of screen

            for (int i = 0; i < count; i++)
            {
                int turretIdx = isRightJustified ? (count - 1 - i) : i;
                int x = isRightJustified
                    ? (int)(gameState.ScreenSize.X - margin - (i + 1) * spacing)
                    : margin + i * spacing;

                // Draw gray background
                Raylib.DrawRectangle(x, y, iconSize, iconSize, new Color(80, 80, 80, 200));

                // Draw turret sprite to fill the icon rectangle
                var turret = player.Turrets[turretIdx];
                if (turret.Sprite is not null)
                {
                    Rectangle dest = new Rectangle(x + (int)iconSize/2, y + (int)iconSize/2, iconSize, iconSize);
                    turret.Sprite.Draw(dest);
                }

                foreach(var gun in turret.Guns)
                {
                    Rectangle dest = new Rectangle(x + (int)iconSize / 2, y + (int)iconSize / 2, iconSize, iconSize);
                    gun.Sprite.Draw(dest);
                }

                // Draw blue rounded border if selected
                if (turretIdx == player.TurretIndex)
                {
                    int borderThickness = (int)(4 * scale);
                    Raylib.DrawRectangleRoundedLines(
                        new Rectangle(x, y, iconSize, iconSize),
                        0.3f, 8,  Color.Blue
                    );
                }
            }
        }
    }
}
