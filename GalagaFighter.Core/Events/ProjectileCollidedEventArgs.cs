using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System;

namespace GalagaFighter.Core.Events
{
    public class ProjectileCollidedEventArgs : EventArgs
    {
        public Projectile Projectile { get; set; }
        public Player Player { get; set; }

        public ProjectileCollidedEventArgs(Projectile projectile, Player player)
        {
            Projectile = projectile;
            Player = player;
        }
    }
}
