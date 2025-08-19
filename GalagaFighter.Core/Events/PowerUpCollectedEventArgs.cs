using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.PowerUps;
using System;
using System.Runtime.CompilerServices;

namespace GalagaFighter.Core.Events
{
    internal class PowerUpCollectedEventArgs : EventArgs
    {
        public Player Player { get; set; }
        public PowerUp PowerUp { get; set; }

        public PowerUpCollectedEventArgs(Player player, PowerUp powerUp)
        {
            Player = player;
            PowerUp = powerUp;
        }
    }
}