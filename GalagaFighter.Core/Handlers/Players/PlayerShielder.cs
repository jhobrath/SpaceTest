using GalagaFighter.Core.Models.Players;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerShielder
    {
        void Shield(Player player);
    }

    public class PlayerShielder : IPlayerShielder
    {
        public void Shield(Player player)
        {
            player.Shield += 1 * Raylib.GetFrameTime();
        }
    }
}
