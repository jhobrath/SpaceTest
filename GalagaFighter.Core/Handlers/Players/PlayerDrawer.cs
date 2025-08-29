using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace GalagaFighter.Core.Handlers.Players
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

            foreach (var decoration in modifiers.Decorations ?? [])
                decoration.Value?.Sprite.Update(frameTime);

            DrawShoot(player, modifiers, playerShootState);
            DrawMove(modifiers, player);
            DrawPlayer(player, modifiers);
            DrawGuns(player, modifiers);

            foreach(var decoration in modifiers.Decorations?.Other ?? [])
            {
                // Apply rotation transformation to the offset
                Vector2 originalOffset = decoration.Offset;
                float rotationRadians = player.Rotation * (float)Math.PI / 180f;
                float cos = (float)Math.Cos(rotationRadians);
                float sin = (float)Math.Sin(rotationRadians);
                
                Vector2 rotatedOffset = new(
                    originalOffset.X * cos - originalOffset.Y * sin,
                    originalOffset.X * sin + originalOffset.Y * cos
                );
                
                decoration.Sprite.Draw(player.Center + rotatedOffset, player.Rotation, decoration.Size!.Value.X, decoration.Size!.Value.Y, Color.White);
            }
        }

        private void DrawPlayer(Player player, EffectModifiers modifiers)
        {
            DrawWithPhantoms(player, modifiers, p =>
            {
                var color = UpdateColors(Color.White, modifiers);
                player.Sprite?.Draw(p.Center, p.Rotation, player.Rect.Width, player.Rect.Height, color);
            });
        }

        private void DrawShoot(Player player, EffectModifiers modifiers, PlayerShootState playerShootState)
        {
            if (modifiers.Decorations?.WindUpLeft != null && playerShootState == PlayerShootState.WindUpLeft)
                DrawWindUp(modifiers.Decorations.WindUpLeft, player, modifiers);
            else if (modifiers.Decorations?.ShootLeft != null && _lastShotLeft[player] < .25f)
                DrawShoot(modifiers.Decorations.ShootLeft, _lastShotLeft, player, modifiers);

            if (modifiers.Decorations?.WindUpRight != null && playerShootState == PlayerShootState.WindUpRight)
                DrawWindUp(modifiers.Decorations.WindUpRight, player, modifiers);
            else if (modifiers.Decorations?.ShootRight != null && _lastShotRight[player] < .25f)
                DrawShoot(modifiers.Decorations.ShootRight, _lastShotRight, player, modifiers);

            else if (modifiers.Decorations?.WindUpBoth != null && playerShootState == PlayerShootState.WindUpBoth)
                DrawWindUp(modifiers.Decorations.WindUpBoth, player, modifiers);
            else if (modifiers.Decorations?.ShootBoth != null && _lastShotBoth[player] < .25f)
                DrawShoot(modifiers.Decorations.ShootBoth, _lastShotBoth, player, modifiers);
        }

        private void DrawMove(EffectModifiers modifiers, Player player)
        {
            DrawWithPhantoms(player, modifiers, p =>
            {
                var left = _inputService.GetMoveLeft(player.Id);
                var right = _inputService.GetMoveRight(player.Id);
                if (modifiers.Decorations?.Move != null && (right || left))
                    modifiers.Decorations.Move.Draw(
                        p.Center,
                        new Vector2(player.Rect.Width, player.Rect.Height),
                        p.Rotation, Color.White);
            });
        }

        private void DrawShoot(SpriteDecoration sprite, Dictionary<Player, float> lastShot, Player player, EffectModifiers modifiers)
        {
            DrawWithPhantoms(player, modifiers, p =>
            {
                sprite.Draw(p.Center, new Vector2(player.Rect.Width, player.Rect.Height), p.Rotation, GetShootAlpha(lastShot[player]));
            });
        }

        private void DrawWindUp(SpriteDecoration sprite, Player player, EffectModifiers modifiers)
        {
            DrawWithPhantoms(player, modifiers, p =>
            {
                sprite.Draw(player.Center, new Vector2(player.Rect.Width, player.Rect.Height), player.Rotation, Color.White);
            });
        }


        private void DrawGuns(Player player, EffectModifiers modifiers)
        {
            DrawWithPhantoms(player, modifiers, p =>
            {
                modifiers.Decorations?.Guns?.Draw(p.Center, new Vector2(player.Rect.Width, player.Rect.Height), p.Rotation, Color.White);
            });
        }

        private void DrawWithPhantoms(Player player, EffectModifiers modifiers, Action<IDrawnPlayer> drawAction)
        {
            var playersToDraw = modifiers.Phantoms.Cast<IDrawnPlayer>().Concat([player]);
            foreach (var playerToDraw in playersToDraw)
                drawAction(playerToDraw);
        }

        private Color GetShootAlpha(float duration)
        {
            return new Color(1, 1, 1, ((.25f - duration) / .25f));
        }

        private Color UpdateColors(Color color, EffectModifiers effects)
        {
            var color1= color.ApplyRed(1 - effects.Display.RedAlpha)
                        .ApplyGreen(1 - effects.Display.GreenAlpha)
                        .ApplyBlue(1 - effects.Display.BlueAlpha);

            var color2 = color1.ApplyAlpha(effects.Display.Opacity);

            return color2;
        }
    }
}
