using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Handlers.Players
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

            SetPhantomSpeed(player, modifiers, left, right);
        }

        private void SetPhantomSpeed(Player player, EffectModifiers modifiers, ButtonState left, ButtonState right)
        {
            foreach(var phantom in modifiers.Phantoms)
            {
                if (left.IsPressed || right.IsPressed)
                {
                    var opposeDirection = Game.Random.Next(0, 2) == 1;
                    phantom.HurryTo(y: player.Speed.Y * (opposeDirection ? -1f : 1f));
                }
                else if (left.IsDown || right.IsDown)
                {
                    var directionOpposed = (player.Speed.Y > 0 && phantom.Speed.Y < 0) || (player.Speed.Y < 0 && phantom.Speed.Y > 0);
                    phantom.HurryTo(y: player.Speed.Y * (directionOpposed ? -1 : 1));
                }
                else
                    phantom.HurryTo(y: player.Speed.Y);

                phantom.Update();
            }
        }

        protected void SetSpeed(Player player, EffectModifiers modifiers, ButtonState left, ButtonState right)
        {
            var durationFactor = left ? left.HeldDuration : (right ? right.HeldDuration : 0f);
            if(durationFactor == 0f)
            {
                var newYFactor = player.Speed.Y - (player.Speed.Y * 10f * Raylib.GetFrameTime());
                player.HurryTo(player.Speed.X, newYFactor);
                return;
            }

            var speedVariability = 600f;
            var baseSpeed = 1200f*player.BaseStats.SpeedMultiplier * modifiers.Stats.SpeedMultiplier;
            var speedFactor = baseSpeed - Math.Min(speedVariability, speedVariability/durationFactor);

            if(player.IsPlayer1)
            { 
                DebugWriter.Write(speedFactor.ToString());
            }
            var signFactor = left ^ !player.IsPlayer1 ? -1 : 1;
            var speedY = speedFactor * signFactor;

            player.HurryTo(x: player.Speed.X, y: speedY);
        }

        private void SetPosition(Player player, EffectModifiers modifiers)
        {
            var frameTime = Raylib.GetFrameTime();
            
            var deltaX = player.Speed.X * frameTime;
            var deltaY = player.Speed.Y * frameTime;
            
            var newY = player.Rect.Y + deltaY;
            if (newY < Game.Margin)
            {
                player.MoveTo(y: Game.Margin);
                player.HurryTo(y: 0f);
            }
            else if (newY + player.Rect.Height > Game.Height - Game.Margin)
            {
                player.MoveTo(y: Game.Height - Game.Margin - player.Rect.Height);
                player.HurryTo(y: 0f); // Stop Y movement when hitting boundary
            }
            else
            {
                player.Move(deltaX, deltaY);
            }
        }

        protected void SetRotation(Player player, EffectModifiers modifiers, ButtonState left, ButtonState right)
        {
            //TODO: Find a way to only use original Rotation here
            var originalRotation = player.IsPlayer1 ? 90f : -90f;

            var frameTime = Raylib.GetFrameTime();

            var movingRotation = 0f;
            if (left)
                movingRotation = Math.Min(left.HeldDuration * -10f, -10f);
            else if (right)
                movingRotation = Math.Max(10f, right.HeldDuration * 10f);
            else
                movingRotation = 0f;

            movingRotation = Math.Clamp(movingRotation, -10f, 10f);

            player.Rotation = originalRotation + movingRotation + modifiers.Display.RotationOffset;
        }

        private void SetSize(Player player, EffectModifiers modifiers)
        {
            //TODO: Find a way to only use original Size here
            var originalSize = new Vector2(160f, 160f);

            var xSize = originalSize.X * modifiers.Display.SizeMultiplier.X;
            var ySize = originalSize.Y * modifiers.Display.SizeMultiplier.Y;

            var currentSize = new Vector2(xSize, ySize);
            var currentPositionX = player.Rect.Position.X - (xSize - player.Rect.Size.X) / 2;
            var currentPositionY = player.Rect.Position.Y - (ySize - player.Rect.Size.Y) / 2;

            player.MoveTo(currentPositionX, currentPositionY);
            player.ScaleTo(xSize, ySize);
        }
    }
}
