using GalagaFighter.Core.Handlers.Collisions;
using GalagaFighter.Core.Models.Debris;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerDrawer
    {
        void Draw(Player player, EffectModifiers modifiers, PlayerShootState playerShootState, float shootMeter);
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

        public void Draw(Player player, EffectModifiers modifiers, PlayerShootState playerShootState, float shootMeter)
        {
            var frameTime = Raylib.GetFrameTime();

            if (modifiers.WereReset)
            {
                _lastShotBoth[player] =  100f;
                _lastShotLeft[player] =  100f;
                _lastShotRight[player] = 100f;
            }

            _lastShotBoth[player] = playerShootState == PlayerShootState.ShootBoth ? 0f : _lastShotBoth.GetValueOrDefault(player, 100f) + frameTime;
            _lastShotLeft[player] = playerShootState == PlayerShootState.ShootLeft ? 0f : _lastShotLeft.GetValueOrDefault(player, 100f) + frameTime;
            _lastShotRight[player] = playerShootState == PlayerShootState.ShootRight ? 0f : _lastShotRight.GetValueOrDefault(player, 100f) + frameTime;

            foreach (var decoration in modifiers.Decorations ?? [])
                decoration.Value?.Sprite.Update(frameTime);

            var lastShot = new[] { _lastShotBoth[player], _lastShotLeft[player], _lastShotRight[player] }.Where(x => x > 0).Min();
            var lastShotKickback = lastShot < .3f ? 5 : (lastShot < .4 ? 4 : (lastShot < .5 ? 3 : (lastShot < .6 ? 2 : (lastShot < .7 ? 1 : 0))));
            lastShotKickback *= player.IsPlayer1 ? -1 : 1;

            //player.Move(x: lastShotKickback);

            var jiggleFactorX = modifiers.Jiggle > 0 ? (float)Game.Random.NextDouble() * modifiers.Jiggle - modifiers.Jiggle / 2f : 0;
            var jiggleFactorY = modifiers.Jiggle > 0 ? (float)Game.Random.NextDouble() * modifiers.Jiggle - modifiers.Jiggle / 2f : 0;
            var jiggle = new Vector2(jiggleFactorX, jiggleFactorY);
            player.Move(jiggle.X, jiggle.Y);

            DrawShoot(player, modifiers, playerShootState);
            DrawMove(modifiers, player);
            DrawGlow(player, modifiers);
            DrawPlayer(player, modifiers, playerShootState);
            DrawShield(player, modifiers, shootMeter);
            DrawGuns(player, modifiers, shootMeter);



            foreach (var decoration in modifiers.Decorations?.Other ?? [])
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
                
                if(decoration.Offset == Vector2.Zero)
                    decoration.Sprite.Draw(player.Position + rotatedOffset, decoration.FollowRotation ? player.Rotation : (player.IsPlayer1 ? 90 : -90), decoration.Size!.Value.X, decoration.Size!.Value.Y, decoration.Sprite.Color);
                else
                    decoration.Sprite.Draw(player.Center + rotatedOffset, decoration.FollowRotation ? player.Rotation : (player.IsPlayer1 ? 90 : -90), decoration.Size!.Value.X, decoration.Size!.Value.Y, decoration.Sprite.Color);
            }

            player.Move(-jiggle.X, -jiggle.Y);
            //player.Move(x: -lastShotKickback);
            //DrawHitbox(player);
        }

        private void DrawHitbox(Player player)
        {
            if (player.Hitbox == null)
                return;

            var verts = player.Hitbox.Vertices;
            verts = ContactCollisionDetector.GetActualBounds(player);
            Raylib.DrawTriangleLines(verts[2], verts[1], verts[0], Color.Red);
        }

        private void DrawGlow(Player player, EffectModifiers modifiers)
        {
            if (modifiers.Decorations?.Glow == null)
                return;

            DrawWithPhantoms(player, modifiers, p =>
            {
                modifiers.Decorations?.Glow.Sprite?.Draw(p.Rect.Position, p.Rotation, player.Rect.Width, player.Rect.Height, modifiers.Decorations.Glow.Sprite.Color);
            });
        }

        private void DrawPlayer(Player player, EffectModifiers modifiers, PlayerShootState playerShootState)
        {
            var color = UpdateColors(Color.White, modifiers);
            
            DrawWithPhantoms(player, modifiers, p =>
            {
                player.Sprite?.Draw(p.Rect.Position, p.Rotation, player.Rect.Width, player.Rect.Height, color);
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
                        p.Rect.Position,
                        new Vector2(player.Rect.Width, player.Rect.Height),
                        p.Rotation, Color.White);
            });
        }

        private void DrawShoot(SpriteDecoration sprite, Dictionary<Player, float> lastShot, Player player, EffectModifiers modifiers)
        {
            DrawWithPhantoms(player, modifiers, p =>
            {
                sprite.Draw(p.Rect.Position, new Vector2(player.Rect.Width, player.Rect.Height), p.Rotation, GetShootAlpha(lastShot[player]));
            });
        }

        private void DrawWindUp(SpriteDecoration sprite, Player player, EffectModifiers modifiers)
        {
            DrawWithPhantoms(player, modifiers, p =>
            {
                sprite.Draw(player.Rect.Position, new Vector2(player.Rect.Width, player.Rect.Height), player.Rotation, Color.White);
            });
        }


        private void DrawGuns(Player player, EffectModifiers modifiers, float shootMeter)
        {
            var redAlpha = ColorExtensions.ApplyRed(Color.White, 1 - shootMeter);

            var jiggleFactorX = modifiers.Jiggle > 0 ? (float)Game.Random.NextDouble() * modifiers.Jiggle - modifiers.Jiggle / 2f : 0;
            var jiggleFactorY = modifiers.Jiggle > 0 ? (float)Game.Random.NextDouble() * modifiers.Jiggle - modifiers.Jiggle / 2f : 0;

            DrawWithPhantoms(player, modifiers, p =>
            {
                modifiers.Decorations?.Guns?.Draw(p.Rect.Position + new Vector2(jiggleFactorX, jiggleFactorY), new Vector2(player.Rect.Width, player.Rect.Height), modifiers.Decorations.Guns.FollowRotation ? p.Rotation : (player.IsPlayer1 ? 90 : -90), redAlpha);
            });
        }

        private void DrawShield(Player player, EffectModifiers modifiers, float shootMeter)
        {
            var jiggleFactorX = modifiers.Jiggle > 0 ? (float)Game.Random.NextDouble() * modifiers.Jiggle - modifiers.Jiggle / 2f : 0;
            var jiggleFactorY = modifiers.Jiggle > 0 ? (float)Game.Random.NextDouble() * modifiers.Jiggle - modifiers.Jiggle / 2f : 0;

            if (modifiers.Decorations?.Shield == null)
                return;

            var color = modifiers.Decorations.Shield.Sprite.Color.ApplyAlpha(modifiers.Display.Opacity * Math.Clamp(player.Shield/100f, 0, 1));

            DrawWithPhantoms(player, modifiers, p =>
            {
                modifiers.Decorations?.Shield?.Draw(p.Rect.Position + new Vector2(jiggleFactorX, jiggleFactorY), new Vector2(player.Rect.Width, player.Rect.Height), modifiers.Decorations.Shield.FollowRotation ? p.Rotation : (player.IsPlayer1 ? 90 : -90), color);
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
            return new Color(1, 1, 1, ((.5f - duration) / .5f));
        }

        private Color UpdateColors(Color color, EffectModifiers effects)
        {
            color = color.ApplyRed(1 - effects.Display.RedAlpha)
                        .ApplyGreen(1 - effects.Display.GreenAlpha)
                        .ApplyBlue(1 - effects.Display.BlueAlpha);

            color = color.ApplyAlpha(effects.Display.Opacity);

            return color;
        }
    }
}
