using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects.Offensives
{
    public abstract class OffensiveEffect : PlayerEffect
    {
        public virtual int Cost => 10;
    }
}
