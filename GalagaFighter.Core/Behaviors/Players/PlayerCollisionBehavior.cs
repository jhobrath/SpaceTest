using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Events;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;

namespace GalagaFighter.Core.Behaviors.Players
{
    public class PlayerCollisionBehavior : IPlayerCollisionBehavior
    {
        private readonly IObjectService _objectService;
        private readonly IEventService _eventService;
        private readonly Action<Player, int>? _damageHandler;

        public PlayerCollisionBehavior(IEventService eventService, IObjectService objectService, Action<Player, int>? damageHandler = null)
        {
            _eventService = eventService;
            _objectService = objectService;
            _damageHandler = damageHandler;
        }

        public void Apply(Player player, Projectile projectile)
        {
            UpdatePlayer(player, projectile);
            _eventService.Publish(new ProjectileCollidedEventArgs(projectile, player));
        }

        protected virtual void UpdatePlayer(Player player, Projectile projectile)
        {
            // Centralized damage calculation using projectile and player stats
            float baseDamage = projectile.BaseDamage;
            float shield = player.Stats.Shield;
            float healthMultiplier = player.Stats.Health; // e.g., 2.0 for double health effect

            float finalDamage = CalculateDamage(baseDamage, shield, healthMultiplier, projectile, player);

            if (_damageHandler != null)
                _damageHandler(player, (int)finalDamage);
            else
                player.Health -= (int)finalDamage; // fallback
        }

        private float CalculateDamage(float baseDamage, float shield, float healthMultiplier, Projectile projectile, Player player)
        {
            // Example calculation: base damage * shield, divided by health multiplier
            float damage = baseDamage * shield;
            damage /= healthMultiplier > 0 ? healthMultiplier : 1f;
            // Extend here for more modifiers (status effects, projectile effects, etc.)
            return damage;
        }
    }
}
