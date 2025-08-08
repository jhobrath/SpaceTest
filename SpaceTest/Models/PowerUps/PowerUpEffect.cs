using GalagaFighter.Models.Players;
using SpaceTest.Models.Projectiles;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Models.PowerUps
{
    public abstract class PowerUpEffect
    {
        protected readonly Player Player;
        protected bool IsActive;

        // Projectile effect configuration
        protected virtual ProjectileType ProjectileType { get; } = ProjectileType.Normal;

        protected PowerUpEffect(Player player)
        {
            Player = player;
            IsActive = true;
        }

        public virtual void OnActivate() { }
        public virtual void OnUpdate(float frameTime) {  }
        public virtual void OnDeactivate() { }
        public virtual void OnShoot(Game game) { }

        public bool ShouldDeactivate() => !IsActive;
    }
}