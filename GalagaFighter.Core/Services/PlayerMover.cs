using GalagaFighter.Core.Behaviors.Players.Updates;
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

            SetPosition(player, modifiers, left, right);
            SetSize(player, modifiers);
            SetRotation(player, modifiers, left, right);
        }

        protected void SetPosition(Player player, EffectModifiers modifiers, ButtonState left, ButtonState right)
        {
            var newPosition = player.Rect.Position;
            var speed = player.Speed.Y * modifiers.Stats.SpeedMultiplier;

            if (left)
            { 
                newPosition.Y = (newPosition.Y - speed / (1 + left.HeldDuration) * (player.IsPlayer1 ? 1 : -1));
            }

            if (right)
            {
                newPosition.Y = (newPosition.Y + speed / (1 + right.HeldDuration) * (player.IsPlayer1 ? 1 : -1));
            }

            if (newPosition.Y < Game.Margin)
                newPosition.Y = Game.Margin;

            var maxY = Game.Height - Game.Margin - player.Rect.Height;
            if (newPosition.Y > maxY)
                newPosition.Y = maxY;

            var frameTime = Raylib.GetFrameTime();
            player.CurrentFrameSpeed = new Vector2(
                (newPosition.X - player.Rect.X)/frameTime, 
                (newPosition.Y - player.Rect.Y)/frameTime
            );
            player.MoveTo(x: newPosition.X, y: newPosition.Y);
        }

        protected void SetRotation(Player player, EffectModifiers modifiers, ButtonState left, ButtonState right)
        {
            var movingRotation = 0f;
            if (left)
                movingRotation = left.HeldDuration * 5f * (player.IsPlayer1 ? -1 : 1);
            else if (right)
                movingRotation = right.HeldDuration * 5f * (player.IsPlayer1 ? 1 : -1);
            else
                movingRotation = 0f;

            movingRotation = Math.Clamp(movingRotation,-10f, 10f);

            player.CurrentFrameRotation = (player.Rotation * modifiers.Display.RotationMultiplier) + movingRotation + modifiers.Display.RotationOffset;
        }

        private void SetSize(Player player, EffectModifiers modifiers)
        {
            var xSize = player.Rect.Size.X * modifiers.Display.SizeMultiplier.X;
            var ySize = player.Rect.Size.Y * modifiers.Display.SizeMultiplier.Y;

            var currentSize = new Vector2(xSize, ySize);
            var currentPositionX = player.Rect.Position.X - xSize / 2;
            var currentPositionY = player.Rect.Position.Y - ySize / 2;

            player.CurrentFrameRect = new Rectangle(
                currentPositionX,
                currentPositionY,
                xSize,
                ySize
            );
        }
    }
}
