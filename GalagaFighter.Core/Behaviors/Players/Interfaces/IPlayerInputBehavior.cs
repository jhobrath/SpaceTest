using GalagaFighter.Core.Behaviors.Players.Updates;
using GalagaFighter.Core.Models;
using GalagaFighter.Core.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Players.Interfaces
{
    public interface IPlayerInputBehavior
    {
        PlayerInputUpdate Apply(Player player);
    }
}
