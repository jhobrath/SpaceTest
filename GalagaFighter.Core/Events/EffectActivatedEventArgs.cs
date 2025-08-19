using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using System;

namespace GalagaFighter.Core.Events
{
    public class EffectActivatedEventArgs<T> : EventArgs
        where T : PlayerEffect
    {
        public T Effect { get; set; }
        public Player Player { get; set; }

        public EffectActivatedEventArgs(PlayerEffect effect, Player player)
        {
            Effect = effect;
            Player = player;
        }
    }
}
