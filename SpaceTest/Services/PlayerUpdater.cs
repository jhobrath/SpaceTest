using GalagaFighter.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Services
{
    public interface IPlayerUpdater
    {
        void Update(Player player, float frameTime);
    }
    public class PlayerUpdater
    {
    }
}
