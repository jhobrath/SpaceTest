using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System;

namespace GalagaFighter.Core.Events
{
    public class ProjectileFiredEventArgs<T> : EventArgs
        where T : Projectile
    {
        public T Projectile { get; set; }
        public Player Player { get; set; }

        public ProjectileFiredEventArgs(T projectile, Player player)
        {
            Projectile = projectile;
            Player = player;
        }
    }
}
