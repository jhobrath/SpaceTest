using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IProjectileMoverWindUpper
    {
        IProjectileMoverWindUpper Create();
        void WindUp(Projectile projectile);
    }
    public class ProjectileMoverWindUpper : IProjectileMoverWindUpper
    {
        private readonly IInputService _inputService;
        private readonly IObjectService _objectService;
        private float _spawnOffset = 0f;
        private float _heldDurationOffset = -1f;
        private float _heldDuration = 0f;

        private float ActualHeldDuration => _heldDuration - _heldDurationOffset;
        private static bool _hasShot = false;

        public ProjectileMoverWindUpper(IObjectService objectService, IInputService inputService)
        {
            _inputService = inputService;
            _objectService = objectService;
            _spawnOffset = 0f;///Calculate on create
        }

        public IProjectileMoverWindUpper Create()
        {
            return new ProjectileMoverWindUpper(_objectService, _inputService);
        }

        public void WindUp(Projectile projectile)
        {
            projectile.SetDrawPriority(-1f);

            if (_spawnOffset == 0f)
            {
                var player = _objectService.GetOwner(projectile);
                _spawnOffset = player.Rect.Y - projectile.Rect.Y;
                projectile.HurryTo(x: projectile.Modifiers.WindUpSpeed * (projectile.Speed.X < 0 ? 1 : -1));
            }

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

            if (_heldDuration - _heldDurationOffset >= projectile.Modifiers.WindUpDuration)
            {
                Release(projectile);
                _heldDurationOffset = _heldDuration;
            }

            var owner = _objectService.GetOwner(projectile);
            projectile.MoveTo(y: owner.Rect.Y - _spawnOffset);
        }

        private void Release(Projectile projectile)
        {
            ChangeSpeed(projectile);
            projectile.Modifiers.OnWindUpReleased?.Invoke(projectile);
            projectile.Modifiers.WindUpDuration = 0f;
        }

        private void ChangeSpeed(Projectile projectile)
        {
            var newSpeed = Math.Max(1050f, Math.Min(projectile.BaseSpeed.X, (ActualHeldDuration / projectile.Modifiers.WindUpDuration) * projectile.BaseSpeed.X));
            if (projectile.Speed.X < 0)
                projectile.HurryTo(x: newSpeed);
            else
                projectile.HurryTo(x: -newSpeed);
        }
    }
}
