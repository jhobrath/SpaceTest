using GalagaFighter.Core.Handlers.Collisions;
using GalagaFighter.Core.Models;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Debris;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IProjectileTetherCollisionService
    {
        void HandleCollisions();
    }
    public class ProjectileTetherCollisionService : IProjectileTetherCollisionService
    {
        private readonly IObjectService _objectService;

        public ProjectileTetherCollisionService(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void HandleCollisions()
        {
            var tethers = _objectService.GetGameObjects<Tether>();
            var projectiles = _objectService.GetGameObjects<Projectile>();

            foreach(var projectile in projectiles)
            {
                foreach(var tether in tethers)
                {
                    if (projectile.Owner == tether.Owner)
                        continue;

                    if(ContactCollisionDetector.HasCollision(tether, projectile))
                    {
                        tether.Health -= projectile.BaseDamage * projectile.Modifiers.DamageMultiplier;
                        projectile.IsActive = false;
                        AddCollision(projectile);
                    }
                }
            }
        }

        private void AddCollision(Projectile projectile)
        {
            var positionX = projectile.Speed.X < 0
                ? projectile.Rect.X
                : projectile.Rect.X + projectile.Rect.Width;
            var positionY = projectile.Center.Y;

            var position = new Vector2(positionX, positionY);

            var collision = new IceShotCollision(Game.Id, position, Vector2.One * 40, projectile.Speed / 5f);
            _objectService.AddGameObject(collision);
        }
    }
}
