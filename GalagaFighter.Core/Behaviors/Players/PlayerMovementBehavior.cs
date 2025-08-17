using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Behaviors.Players.Updates;
using GalagaFighter.Core.Models.Players;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Players
{
    public class DefaultMovementBehavior : IPlayerMovementBehavior
    {
        private float _baseSpeed = 20f;

        public DefaultMovementBehavior()
        {

        }

        public PlayerMovementUpdate Apply(Player player, PlayerInputUpdate inputUpdate, PlayerMovementUpdate update)
        {
            var vec2 = update.To;
            var speed = _baseSpeed * player.Stats.MovementSpeed * Game.UniformScale;

            if (inputUpdate.Left)
            {
                vec2.Y = update.From.Y - speed / (1+inputUpdate.Left.HeldDuration) * (player.IsPlayer1 ? 1 : -1);
            }
            else if (inputUpdate.Right)
            {
                vec2.Y = update.From.Y + speed / (1 + inputUpdate.Right.HeldDuration) * (player.IsPlayer1 ? 1 : -1);
            }

            if (vec2.Y < Game.Margin)
                vec2.Y = Game.Margin;

            var maxY = Game.Height - Game.Margin - player.Rect.Height;
            if (vec2.Y > maxY)
                vec2.Y = maxY;

            update.To = vec2;

            ApplyMovementRotation(player, update);

            return update;
        }

        //Does this belong here?
        private void ApplyMovementRotation(Player player, PlayerMovementUpdate movement)
        {
            if (movement.To.Y < movement.From.Y)
                player.Display.Rotation -= 5f * (player.IsPlayer1 ? 1 : -1);
            else if (movement.To.Y > movement.From.Y)
                player.Display.Rotation += 5f * (player.IsPlayer1 ? 1 : -1);
        }
    }
}
