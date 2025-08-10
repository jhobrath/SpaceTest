using GalagaFighter.Models.Players;
using Raylib_cs;

namespace GalagaFighter.Models.Effects
{
    public abstract class PlayerEffect
    {
        protected readonly Player Player;
        public bool IsActive { get; protected set; } = true;

        protected PlayerEffect(Player player)
        {
            Player = player;
        }

        public virtual void OnActivate() { }
        public virtual void OnUpdate(float frameTime) { }
        public virtual void OnDeactivate() { }
        public virtual void OnShoot(Game game) { }
        public bool ShouldDeactivate() => !IsActive;

        // Allow effects to modify ship properties (speed, color, etc.)
        public virtual void ModifyPlayer(Player player, ref float speed, ref Color color) { }

        // Speed multiplier for stacking movement effects
        public virtual float SpeedMultiplier => 1.0f;

        protected virtual float Duration => 5f;
    }
}