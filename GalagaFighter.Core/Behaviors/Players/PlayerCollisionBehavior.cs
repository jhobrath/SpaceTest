using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Events;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;

namespace GalagaFighter.Core.Behaviors.Players
{
    public class PlayerCollisionBehavior : IPlayerCollisionBehavior
    {
        private readonly IObjectService _objectService;
        private readonly IEventService _eventService;

        public PlayerCollisionBehavior(IEventService eventService, IObjectService objectService)
        {
            _eventService = eventService;
            _objectService = objectService;
        }

        public void Apply(Player player, Projectile projectile)
        {
            UpdatePlayer(player, projectile);

            _eventService.Publish(new ProjectileCollidedEventArgs(projectile, player));
        }

        protected virtual void UpdatePlayer(Player player, Projectile projectile)
        {
            player.Health -= projectile.Damage*player.Stats.Shield;
        }
    }
}
