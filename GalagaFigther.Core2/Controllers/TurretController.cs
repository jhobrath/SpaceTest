using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.GameObjects.Turrets;
using GalagaFighter.Core2.Handlers.Projectiles;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Models.Turrets;
using GalagaFighter.Core2.Services;
using Raylib_cs;
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

        private const float _defaultCountdown = .05f;

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
            var player = _objectService.Get(turret.Owner);

            turretData.Lifetime += frameTime;
            if(turretData.Lifetime > 20f)
            { 
                turret.IsActive = false;

                var children = _objectService.GetChildren(turret);
                foreach (var child in children)
                    child.IsActive = false;
            }

            turretData.ShotCountdown -= frameTime;
            if (turretData.ShotCountdown > 0)
                return;
            
            turretData.ShotCountdown = _defaultCountdown;

            foreach (var turretGun in turret.Guns)
                turretGun.ShotRequested = true;
        }

        public void Draw(Turret turret, float frameTime)
        {
            var hasDrawnGun = false;

            foreach (var decoration in turret.Decorations.OrderBy(x => x.Depth))
            {
                if (decoration.Depth > 0 && !hasDrawnGun)
                {
                    DrawTurret(turret);
                    hasDrawnGun = true;
                }

                decoration.Update(turret, frameTime);
                decoration.Sprite.Update(frameTime);
                decoration.Sprite.Draw(new(turret.WorldPosition, turret.Rect.Size), turret.WorldRotation + decoration.Rotation, Color.White);
            }

            if (!hasDrawnGun)
                DrawTurret(turret);
        }

        private void DrawTurret(Turret turret)
        {
            turret.Sprite.Draw(turret);
        }
    }
}
