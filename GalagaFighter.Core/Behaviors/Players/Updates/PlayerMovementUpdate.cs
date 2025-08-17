using GalagaFighter.Core.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Players.Updates
{
    public class PlayerMovementUpdate
    {
        public Vector2 From { get; set; } = Vector2.Zero;
        public Vector2 To { get; set; } = Vector2.Zero;

        public PlayerMovementUpdate(Player player)
        {
            From = player.Rect.Position;
            To = player.Rect.Position;
        }
    }
}
