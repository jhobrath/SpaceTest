using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Events
{
    public class EffectDeactivatedEventArgs<T> : EventArgs
        where T : PlayerEffect
    {
        public T Effect { get; set; }
        public Player Player { get; set; }

        public EffectDeactivatedEventArgs(T effect, Player player)
        {
            Effect = effect;
            Player = player;
        }
    }
}
