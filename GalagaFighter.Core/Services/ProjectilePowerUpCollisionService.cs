using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.PowerUps;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IProjectilePowerUpCollisionService
    {
        void HandleCollisions();
    }

    public class ProjectilePowerUpCollisionService : IProjectilePowerUpCollisionService
    {
        private readonly IObjectService _objectService;

        public ProjectilePowerUpCollisionService(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void HandleCollisions()
        {
            var projectiles = _objectService.GetGameObjects<Projectile>();
            var powerUps = _objectService.GetGameObjects<PowerUp>();

            foreach(var projectile in projectiles)
            {
                foreach(var powerUp in powerUps)
                {
                    if (powerUp.Owner == projectile.Owner)
                        continue;

                    if (!Raylib.CheckCollisionRecs(projectile.Rect, powerUp.Rect))
                        continue;
                 
                    Collide(projectile, powerUp);
                }
            }
        }

        private void Collide(Projectile projectile, PowerUp powerUp)
        {
            powerUp.Owner = projectile.Owner;
            projectile.IsActive = false;
            projectile.Modifiers.OnProjectileDestroyed?.Invoke(projectile);
        }
    }
}
