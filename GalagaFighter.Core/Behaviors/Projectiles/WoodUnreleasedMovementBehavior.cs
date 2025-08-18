using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Projectiles
{
    public class WoodUnreleasedMovementBehavior : ProjectileMovementBehavior
    {
        private readonly IInputService _inputService;
        private readonly IObjectService _objectService;
        private readonly float _spawnOffset;
        private float _heldDurationOffset = -1f;
        private float _heldDuration = 0f;

        private float ActualHeldDuration => _heldDuration - _heldDurationOffset;
        private static bool _hasShot = false;

        public WoodUnreleasedMovementBehavior(IObjectService objectService, IInputService inputService, float spawnOffset)
        {
            _inputService = inputService;
            _objectService = objectService;
            _spawnOffset = spawnOffset;
        }

        public override void Apply(Projectile projectile)
        {
            CheckForRelease(projectile);

            base.Apply(projectile);
        }

        private void CheckForRelease(Projectile projectile)
        {
            var shootState = _inputService.GetShoot(projectile.Owner);

            if (_heldDurationOffset == -1f)
                _heldDurationOffset = shootState.HeldDuration;

            if (!shootState.IsDown)
            { 
                Release(projectile);
                _heldDurationOffset = 0f;
                return;
            }
            else 
            { 
                _heldDuration = shootState.HeldDuration;
                projectile.Hurry(x: .965f);
            }

            if (_heldDuration - _heldDurationOffset >= 1.5f)
            { 
                Release(projectile);
                _heldDurationOffset = _heldDuration;
            }

            var owner = _objectService.GetOwner(projectile);
            projectile.MoveTo(y: owner.Rect.Y - _spawnOffset + owner.Speed.Y);
        }

        private void Release(Projectile projectile)
        {
            ChangeSpeed(projectile);
            projectile.SetDestroyBehavior(new WoodReleasedDestroyBehavior());
            projectile.SetMovementBehavior(new WoodReleasedMovementBehavior());
            projectile.SetCollisionBehavior(new WoodCollisionBehavior(_objectService));
            ((WoodProjectile)projectile).Released = true;
        }

        private void ChangeSpeed(Projectile projectile)
        {
            var newSpeed = Math.Max(1050f, Math.Min(11000f, (ActualHeldDuration / 1.5f) * 11000f));
            if (projectile.Speed.X < 0)
                projectile.HurryTo(x: newSpeed);
            else
                projectile.HurryTo(x: -newSpeed);
        }
    }
}
