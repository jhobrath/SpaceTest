using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services
{
    public interface IGameObjectUpdateService
    {
        public void Update(float frameTime);
        public void Draw(float frameTime);
    }

    public class GameObjectUpdateService : IGameObjectUpdateService
    {
        private readonly IObjectService _objectService;
        private readonly IPlayerController _playerController;
        private readonly IProjectileController _projectileController;
        private readonly IPowerUpController _powerUpController;
        private readonly ITurretController _turretController;
        private readonly IGunController _gunController;
        private readonly IParticleEmitterController _particleEmitterController;
        private readonly IParticleController _particleController;
        private readonly ICollisionController _collisionController;

        public GameObjectUpdateService(IObjectService objectService, IPlayerController playerController,
            IProjectileController projectileController, IPowerUpController powerUpController,
            ITurretController turretController, IGunController gunController,
            IParticleEmitterController particleEmitterController, IParticleController particleController,
            ICollisionController collisionController)
        {
            _objectService = objectService;
            _playerController = playerController;
            _projectileController = projectileController;
            _powerUpController = powerUpController;
            _turretController = turretController;
            _gunController = gunController;
            _particleEmitterController = particleEmitterController;
            _particleController = particleController;
            _collisionController = collisionController;
        }

        public void Update(float frameTime)
        {
            UpdateType(_playerController, frameTime);
            UpdateType(_projectileController, frameTime);
            UpdateType(_powerUpController, frameTime);
            UpdateType(_turretController, frameTime);
            UpdateType(_gunController, frameTime);
            UpdateType(_particleEmitterController, frameTime);
            UpdateType(_particleController, frameTime);
            UpdateType(_collisionController, frameTime);
        }

        private void UpdateType<T>(IController<T> controller, float frameTime)
            where T : GameObject
        {
            var gameObjects = _objectService.GetAll<T>();
            foreach (var gameObject in gameObjects)
                controller.Update(gameObject, frameTime);
        }

        public void Draw(float frameTime)
        {
            DrawType(_playerController, frameTime);
            DrawType(_projectileController, frameTime);
            DrawType(_powerUpController, frameTime);
            DrawType(_turretController, frameTime);
            DrawType(_gunController, frameTime);
            DrawType(_particleEmitterController, frameTime);
            DrawType(_particleController, frameTime);
            DrawType(_collisionController, frameTime);
        }

        private void DrawType<T>(IController<T> controller, float frameTime)
            where T : GameObject
        {
            var gameObjects = _objectService.GetAll<T>();
            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.IsActive)
                    continue;

                gameObject.Sprite.Update(frameTime);

                controller.Draw(gameObject, frameTime);

                //DrawHitboxes(gameObject);
            }
        }

        private static void DrawHitboxes<T>(T gameObject) where T : GameObject
        {
            //if(!(gameObject is ShotGunShellProjectile shell))
            //    return;

            var vertices = PolygonVerticesCompiler.GetVertices(gameObject);

            for (var i = 0; i < vertices.Length; i++)
            {
                var endIndex = i == vertices.Length - 1 ? 0 : i + 1;
                Raylib.DrawLine((int)vertices[i].X, (int)vertices[i].Y, (int)vertices[endIndex].X, (int)vertices[endIndex].Y, Color.Red);
            }
        }
    }
}
