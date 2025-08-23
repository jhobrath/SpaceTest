using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Numerics;
using System.Collections.Generic;
using GalagaFighter.Core.Controllers;
using System.Net.Http.Headers;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerDrawer
    {
        void Draw(Player player, EffectModifiers modifiers, PlayerShootState playerShootState);
    }
    public class PlayerDrawer : IPlayerDrawer
    {
        private readonly IInputService _inputService;

        private readonly Dictionary<Player, float> _lastShotBoth = [];
        private readonly Dictionary<Player, float> _lastShotLeft = [];
        private readonly Dictionary<Player, float> _lastShotRight = [];
        private readonly Dictionary<Player, bool> _lastFrameWindingLeft = [];
        private readonly Dictionary<Player, bool> _lastFrameWindingRight = [];
        private readonly Dictionary<Player, bool> _lastFrameWindingBoth = [];

        public PlayerDrawer(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void Draw(Player player, EffectModifiers modifiers, PlayerShootState playerShootState)
        {
            var frameTime = Raylib.GetFrameTime();

            _lastShotBoth[player] = playerShootState == PlayerShootState.ShootBoth ? 0f : _lastShotBoth.GetValueOrDefault(player, 100f) + frameTime;
            _lastShotLeft[player] = playerShootState == PlayerShootState.ShootLeft ? 0f : _lastShotLeft.GetValueOrDefault(player, 100f) + frameTime;
            _lastShotRight[player] = playerShootState == PlayerShootState.ShootRight ? 0f : _lastShotRight.GetValueOrDefault(player, 100f) + frameTime;

            var lastShotTime = Math.Min(_lastShotBoth[player], Math.Min(_lastShotLeft[player], _lastShotRight[player]));
            var shootHeldTime = _inputService.GetShoot(player.Id).HeldDuration;

            foreach (var decoration in modifiers.Decorations ?? new SpriteDecorations())
                decoration.Value?.Sprite.Update(frameTime);

            if (modifiers.Decorations?.WindUpLeft != null && playerShootState == PlayerShootState.WindUpLeft)
                modifiers.Decorations.WindUpLeft.Draw(player.Center, new Vector2(player.CurrentFrameRect.Width, player.CurrentFrameRect.Height), player.CurrentFrameRotation, Color.White);
            else if (modifiers.Decorations?.ShootLeft != null && _lastShotLeft[player] < .25f)
                DrawShoot(modifiers.Decorations.ShootLeft, _lastShotLeft, player);

            if(modifiers.Decorations?.WindUpRight != null && playerShootState == PlayerShootState.WindUpRight)
                modifiers.Decorations.WindUpRight.Draw(player.Center, new Vector2(player.CurrentFrameRect.Width, player.CurrentFrameRect.Height), player.CurrentFrameRotation, Color.White);
            else if (modifiers.Decorations?.ShootRight != null && _lastShotRight[player] < .25f)
                DrawShoot(modifiers.Decorations.ShootRight, _lastShotRight, player);

            else if (modifiers.Decorations?.WindUpBoth != null && playerShootState == PlayerShootState.WindUpBoth)
                modifiers.Decorations.WindUpBoth.Draw(player.Center, new Vector2(player.CurrentFrameRect.Width, player.CurrentFrameRect.Height), player.CurrentFrameRotation, Color.White);
            else if (modifiers.Decorations?.ShootBoth != null && _lastShotBoth[player] < .25f)
                DrawShoot(modifiers.Decorations.ShootBoth, _lastShotBoth, player);

            DrawMove(modifiers, player);

            player.CurrentFrameSprite.Draw(player.Center, player.CurrentFrameRotation, player.CurrentFrameRect.Width, player.CurrentFrameRect.Height, player.CurrentFrameColor);
        }

        private void DrawMove(EffectModifiers modifiers, Player player)
        {
            var left = _inputService.GetMoveLeft(player.Id);
            var right = _inputService.GetMoveRight(player.Id);
            if (modifiers.Decorations?.Move != null && (right || left))
                modifiers.Decorations.Move.Draw(player.Center, new Vector2(player.CurrentFrameRect.Width, player.CurrentFrameRect.Height), player.CurrentFrameRotation, Color.White);

        }

        private void DrawShoot(EffectModifiers modifiers, Player player)
        {
            var decorations = modifiers.Decorations;
            if (decorations?.ShootBoth != null && _lastShotBoth[player] < .25f)
                DrawShoot(decorations.ShootBoth, _lastShotBoth, player);
            else if (decorations?.ShootLeft != null && _lastShotLeft[player] < .25f)
                DrawShoot(decorations.ShootLeft, _lastShotLeft, player);
            else if (decorations?.ShootRight != null && _lastShotRight[player] < .25f)
                DrawShoot(decorations.ShootRight, _lastShotRight, player);
        }

        private void DrawShoot(SpriteDecoration sprite, Dictionary<Player, float> lastShot, Player player)
        {
            sprite.Draw(player.Center, new Vector2(player.CurrentFrameRect.Width, player.CurrentFrameRect.Height), player.CurrentFrameRotation, GetShootAlpha(lastShot[player]));
        }

        private Color GetShootAlpha(float duration)
        {
            return new Color(1, 1, 1, ((.25f - duration) / .25f));
        }
    }
}
