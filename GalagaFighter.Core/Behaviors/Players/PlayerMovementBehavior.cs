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
    public class PlayerMovementBehavior : IPlayerMovementBehavior
    {
        protected virtual float BaseSpeed => 20f;

        public PlayerMovementBehavior()
        {

        }

        public PlayerMovementUpdate Apply(Player player, PlayerInputUpdate inputUpdate)
        {
            var update = new PlayerMovementUpdate(player);

            var position = GetPosition(player, inputUpdate);
            var rotation = GetRotation(player, position);

            player.Display.Rotation = rotation;
            player.MoveTo(position.X, position.Y);

            update.To = position;
            return update;
        }

        protected virtual Vector2 GetPosition(Player player, PlayerInputUpdate inputUpdate)
        {
            var newPosition = player.Rect.Position;
            var speed = BaseSpeed * player.Stats.MovementSpeed * Game.UniformScale;

            if (inputUpdate.Left)
            {
                player.HurryTo(y: speed);
                newPosition.Y = newPosition.Y - speed / (1 + inputUpdate.Left.HeldDuration) * (player.IsPlayer1 ? 1 : -1);
            }
            else if (inputUpdate.Right)
            {
                newPosition.Y = newPosition.Y + speed / (1 + inputUpdate.Right.HeldDuration) * (player.IsPlayer1 ? 1 : -1);
            }

            if (newPosition.Y < Game.Margin)
                newPosition.Y = Game.Margin;

            var maxY = Game.Height - Game.Margin - player.Rect.Height;
            if (newPosition.Y > maxY)
                newPosition.Y = maxY;

            player.HurryTo(y: newPosition.Y - player.Rect.Y);

            return newPosition;
        }

        protected virtual float GetRotation(Player player, Vector2 newPosition)
        {
            if (newPosition.Y < player.Rect.Y)
                return player.IsPlayer1 ? 85f : 95f;// player.Display.Rotation - 5f * (player.IsPlayer1 ? 1 : -1);

            if (newPosition.Y > player.Rect.Y)
                return player.IsPlayer1 ? 95f : 85f;// + 5f * (player.IsPlayer1 ? 1 : -1);

            return player.IsPlayer1 ? 90f : -90f;
        }
    }
}
