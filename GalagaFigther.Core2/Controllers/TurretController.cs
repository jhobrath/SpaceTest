using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Turrets;
using GalagaFighter.Core2.Handlers.Projectiles;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Models.Turrets;
using GalagaFighter.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Controllers
{
    public interface ITurretController : IController<Turret>
    {

    }
    public class TurretController : ITurretController
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;
        private readonly IProjectileShooter _projectileShooter;

        private const float _defaultCountdown = .41f;

        public TurretController(IGameDataRegistry gameDataRegistry, IObjectService objectService, 
            IProjectileShooter projectileInitialPositionCalculator)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
            _projectileShooter = projectileInitialPositionCalculator;
        }

        public void Update(Turret turret, float frameTime)
        {
            var turretData = _gameDataRegistry.Get<TurretDeployData>(turret);

            turretData.Lifetime += frameTime;
            if(turretData.Lifetime > 20f)
                turret.IsActive = false;

            turretData.ShotCountdown -= frameTime;
            if (turretData.ShotCountdown > 0)
                return;

            turretData.ShotCountdown = _defaultCountdown;

            var guns = _objectService.GetChildren<TurretGun>(turret);
            
            foreach(var gun in guns)
                foreach (var offset in gun.GunOffsets)
                    foreach(var projectile in gun.OnShoot(turret.Owner))
                        _projectileShooter.Shoot(gun, projectile, offset.Position, offset.SpeedMultiplier);
        }

        public void Draw(Turret turret, float frameTime)
        {
            turret.Sprite.Draw(turret.Rect, turret.Rotation, turret.Color);

            var children = _objectService.GetChildren(turret);
            foreach(var child in children)
            {
                child.Sprite.Draw(child.Rect, child.Rotation, child.Color);
            }
        }
    }
}
