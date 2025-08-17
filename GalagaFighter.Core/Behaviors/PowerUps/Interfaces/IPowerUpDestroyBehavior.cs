using GalagaFighter.Core.Models.PowerUps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.PowerUps.Interfaces
{
    public interface IPowerUpDestroyBehavior
    {
        void Apply(PowerUp powerUp);
    }
}
