using GalagaFigther.Core2.Controllers;
using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.GameObjects.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Services
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

        public GameObjectUpdateService(IObjectService objectService, IPlayerController playerController, 
            IProjectileController projectileController, IPowerUpController powerUpController)
        {
            _objectService = objectService;
            _playerController = playerController;
            _projectileController = projectileController;
            _powerUpController = powerUpController;
        }

        public void Update(float frameTime)
        {
            UpdateType(_playerController, frameTime);
            UpdateType(_projectileController, frameTime);
            UpdateType(_powerUpController, frameTime);
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
        }

        private void DrawType<T>(IController<T> controller, float frameTime)
            where T : GameObject
        {
            var gameObjects = _objectService.GetAll<T>();
            foreach (var gameObject in gameObjects)
                controller.Draw(gameObject, frameTime);
        }
    }
}
