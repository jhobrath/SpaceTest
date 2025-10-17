using GalagaFighter.Core2.Controllers;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
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
            {
                controller.Draw(gameObject, frameTime);

                var vertices = PolygonVerticesCompiler.GetVertices(gameObject);

                for(var i = 0;i < vertices.Length;i++)
                {
                    var endIndex = i == vertices.Length - 1 ? 0 : i + 1;
                    Raylib.DrawLine((int)vertices[i].X, (int)vertices[i].Y, (int)vertices[endIndex].X, (int)vertices[endIndex].Y, Color.Red);
                }
            }
        }
    }
}
