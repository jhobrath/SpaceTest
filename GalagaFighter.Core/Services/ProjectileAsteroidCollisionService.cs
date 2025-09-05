using GalagaFighter.Core.Handlers.Collisions;
using GalagaFighter.Core.Models.Debris;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IProjectileAsteroidCollisionService
    {
        void HandleCollisions();
    }
    public class ProjectileAsteroidCollisionService : IProjectileAsteroidCollisionService
    {
        private readonly IObjectService _objectService;
        private readonly ICollisionCreationService _collisionCreationService;

        public ProjectileAsteroidCollisionService(IObjectService objectService, ICollisionCreationService collisionCreationService)
        {
            _objectService = objectService;
            _collisionCreationService = collisionCreationService;
        }

        public void HandleCollisions()
        {
            var projectiles = _objectService.GetGameObjects<Projectile>();
            var asteroids = _objectService.GetGameObjects<Asteroid>();

            foreach (var projectile in projectiles)
                foreach (var asteroid in asteroids)
                    if (CollisionOccured(projectile, asteroid))
                        HandleCollision(projectile, asteroid);
        }

        private bool CollisionOccured(Projectile projectile, Asteroid asteroid)
        {
            return ContactCollisionDetector.HasCollision(projectile, asteroid);
        }

        private void HandleCollision(Projectile projectile, Asteroid asteroid)
        {
            var rotationToAdd = projectile.Center.Y - asteroid.Center.Y;
            var speedToAdd = (asteroid.Speed + projectile.Speed/10 ) / (asteroid.Speed);

            asteroid.Hurry(speedToAdd.X, speedToAdd.Y);
            asteroid.AddRotation(rotationToAdd);

            var impactPointX = projectile.Speed.X < 0 ? projectile.Rect.X : projectile.Rect.X + projectile.Rect.Width;
            var collisionSize = (float)Game.Random.NextDouble() * 50f + 50f;
            var collision = _collisionCreationService.Create(new Vector2(impactPointX, projectile.Center.Y), Vector2.One * collisionSize, projectile.Speed / 4);
            collision.SetDrawPriority(10);
            projectile.IsActive = false;
        }
    }
}
