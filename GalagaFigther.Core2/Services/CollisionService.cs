using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.PowerUps;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Handlers.Collisions;
using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services
{
    public interface ICollisionService
    {
        void Update();
    }
    public class CollisionService : ICollisionService
    {
        private readonly IObjectService _objectService;
        private readonly IProjectilePowerUpCollisionHandler _projectilePowerUpCollisionHandler;

        public CollisionService(IObjectService objectService, IProjectilePowerUpCollisionHandler projectilePowerUpCollisionHandler)
        {
            _objectService = objectService;
            _projectilePowerUpCollisionHandler = projectilePowerUpCollisionHandler;
        }

        public void Update()
        {
            CheckCollisions<Projectile, PowerUp>(_projectilePowerUpCollisionHandler.Handle);
        }

        private void CheckCollisions<Type1, Type2>(Action<Type1, Type2> handle)
            where Type1: GameObject
            where Type2: GameObject
        {
            var type1s = _objectService.GetAll<Type1>();
            var type2s = _objectService.GetAll<Type2>();

            foreach(var type1 in type1s)
                foreach(var type2 in type2s)
                    if(CheckCollision(type1, type2))
                        handle(type1, type2);
        }

        private bool CheckCollision(GameObject projectile, GameObject powerUp)
        {
            var projectileVertices = PolygonVerticesCompiler.GetVertices(projectile.Rect, projectile.Center, projectile.Rotation);
            var powerUpVertices = PolygonVerticesCompiler.GetVertices(powerUp.Rect, powerUp.Center, powerUp.Rotation);
            bool collides = PolygonCollisionDetector.Detect(projectileVertices, powerUpVertices);

            return collides;
        }
    }
}
