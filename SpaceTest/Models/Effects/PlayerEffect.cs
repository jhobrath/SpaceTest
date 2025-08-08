using GalagaFighter.Models.Players;
using GalagaFighter;
using GalagaFighter.Models;

namespace GalagaFighter.Models.Effects
{
    public abstract class PlayerEffect
    {
        protected readonly Player Player;
        protected bool IsActive;

        // Projectile effect configuration
        protected virtual ProjectileType ProjectileType { get; } = ProjectileType.Normal;

        protected PlayerEffect(Player player)
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