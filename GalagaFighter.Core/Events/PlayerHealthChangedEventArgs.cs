using GalagaFighter.Core.Models.Players;
using System;

namespace GalagaFighter.Core.Events
{
    public class PlayerHealthChangedEventArgs : EventArgs
    {
        public Player Player { get; }
        public float OldHealth { get; }
        public float NewHealth { get; }

        public PlayerHealthChangedEventArgs(Player player, float oldHealth, float newHealth)
        {
            Player = player;
            OldHealth = oldHealth;
            NewHealth = newHealth;
        }
    }
}
