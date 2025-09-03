using GalagaFighter.Core.Models.Players;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerWarpService
    {
        void Warp(Player player, EffectModifiers modifiers);

    }

    public class PlayerWarpService : IPlayerWarpService
    {
        public void Warp(Player player, EffectModifiers modifiers)
        {
            var gameHeight = Game.Height;
            var reflectPoint = gameHeight / 2;
            var distance = Math.Abs(player.Center.Y - reflectPoint);

            if (player.Center.Y > reflectPoint)
                player.Move(y:-distance * 2);
            else
                player.Move(y:distance * 2);

            modifiers.Warp = false;
        }
    }
}
