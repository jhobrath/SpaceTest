using GalagaFighter.Core.Models.Players;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerMover
    {
        void Move(Player player, EffectModifiers modifiers);
    }
    public class PlayerMover : IPlayerMover
    {
        private readonly IInputService _inputService;

        public PlayerMover(IInputService inputService)
        {
            _inputService = inputService;
        }

        public void Move(Player player, EffectModifiers modifiers)
        {
            var left = _inputService.GetMoveLeft(player.Id);
            var right = _inputService.GetMoveRight(player.Id);

            SetSpeed(player, modifiers, left, right);
            SetPosition(player, modifiers);
            SetSize(player, modifiers);
            SetRotation(player, modifiers, left, right);
        }

        protected void SetSpeed(Player player, EffectModifiers modifiers, ButtonState left, ButtonState right)
        {
            var baseSpeed = 20f * modifiers.Stats.SpeedMultiplier;

            var speedY = 0f;
            if (left)
                speedY = -baseSpeed / (1 + left.HeldDuration) * (player.IsPlayer1 ? 1 : -1);
            else if (right)
                speedY = baseSpeed / (1 + right.HeldDuration) * (player.IsPlayer1 ? 1 : -1);

            player.HurryTo(x: player.Speed.X, y: speedY);

        }

        private void SetPosition(Player player, EffectModifiers modifiers)
        {
            var newY = player.Rect.Y + player.Speed.Y;
            if (newY < Game.Margin)
            {
                player.MoveTo(y: Game.Margin);
                player.HurryTo(y: 0f); // Stop Y movement when hitting boundary
            }
            else if (newY + player.Rect.Height > Game.Height - Game.Margin)
            {
                player.MoveTo(y: Game.Height - Game.Margin - player.Rect.Height);
                player.HurryTo(y: 0f); // Stop Y movement when hitting boundary
            }
            else
            {
                // Move the player using the speed-based approach
                player.MoveTo(x: player.Rect.X + player.Speed.X, y: player.Rect.Y + player.Speed.Y);
            }
        }

        protected void SetRotation(Player player, EffectModifiers modifiers, ButtonState left, ButtonState right)
        {
            //TODO: Find a way to only use original Rotation here
            var originalRotation = player.IsPlayer1 ? 90f : -90f;

            var frameTime = Raylib.GetFrameTime();


            var movingRotation = 0f;
            if (left)
                movingRotation = Math.Min(left.HeldDuration * -10f,-10f);
            else if (right)
                movingRotation = Math.Max(10f, right.HeldDuration * 10f);
            else
                movingRotation = 0f;

            movingRotation = Math.Clamp(movingRotation,-10f, 10f);

            player.Rotation = originalRotation +  movingRotation + modifiers.Display.RotationOffset;

            if(!player.IsPlayer1)
            DebugWriter.Write(player.Rotation.ToString());
        }

        private void SetSize(Player player, EffectModifiers modifiers)
        {
            //TODO: Find a way to only use original Size here
            var originalSize = new Vector2(160f, 160f);

            var xSize = originalSize.X * modifiers.Display.SizeMultiplier.X;
            var ySize = originalSize.Y * modifiers.Display.SizeMultiplier.Y;

            var currentSize = new Vector2(xSize, ySize);
            var currentPositionX = player.Rect.Position.X - (xSize - player.Rect.Size.X)/ 2;
            var currentPositionY = player.Rect.Position.Y - (ySize - player.Rect.Size.Y)/ 2;

            player.MoveTo(currentPositionX, currentPositionY);
            player.ScaleTo(xSize, ySize);
        }
    }
}
