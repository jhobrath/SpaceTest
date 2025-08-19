using GalagaFighter.Core.Behaviors.Players.Updates;
using GalagaFighter.Core.Behaviors.Projectiles;
using GalagaFighter.Core.Events;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Players
{
    public class WoodShotShootingBehavior : PlayerShootingBehavior
    {
        private readonly IInputService _inputService;
        private readonly IEventService _eventService;

        protected override Vector2 SpawnOffset => new Vector2(-40, 25);

        public WoodShotShootingBehavior(IEventService eventService, IObjectService objectService, IInputService inputService) : base(objectService)
        {
            _inputService = inputService;
            _eventService = eventService;
        }

        protected override bool GetCanShoot(Player player, PlayerInputUpdate inputUpdate)
        {
            if (!inputUpdate.Shoot)
                return false;

            var existingProjectiles = _objectService.GetChildren<WoodProjectile>(player.Id);
            var unreleased = existingProjectiles.Count(p => !p.Released && !p.Planked);
            return unreleased == 0;
        }

        protected override Vector2 GetBaseSize(Player player)
        {
            return WoodProjectile.BaseSize;
        }

        protected override Vector2 GetBaseSpeed()
        {
            return WoodProjectile.BaseSpeed;
        }

        protected override Projectile Create(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            var projectile = new WoodProjectile(owner, initialPosition, initialSize, initialSpeed);
            var player = (Player)_objectService.GetById(owner);
            projectile.SetMovementBehavior(new WoodUnreleasedMovementBehavior(_objectService, _inputService, spawnOffset: player.Rect.Y - projectile.Rect.Y));
            projectile.SetDestroyBehavior(new ProjectileDestroyBehavior());
            projectile.SetCollisionBehavior(new ProjectileCollisionBehavior(_objectService));
            projectile.SetDrawPriority(-1);

            _eventService.Publish(new ProjectileFiredEventArgs<WoodProjectile>(projectile, player));

            return projectile;
        }

        protected override void SetRotation(Projectile projectile)
        {
            if (projectile.Speed.X > 0f)
                projectile.Rotation = -180f;
        }
    }
}
