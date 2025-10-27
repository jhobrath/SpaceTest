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
        private readonly IPlayerPowerUpCollisionHandler _playerPowerUpCollisionHandler;
        private readonly IPlayerProjectileCollisionHandler _playerProjectileCollisionHandler;

        public CollisionService(IObjectService objectService, IProjectilePowerUpCollisionHandler projectilePowerUpCollisionHandler, 
            IPlayerPowerUpCollisionHandler playerPowerUpCollisionHandler, IPlayerProjectileCollisionHandler playerProjectileCollisionHandler)
        {
            _objectService = objectService;
            _projectilePowerUpCollisionHandler = projectilePowerUpCollisionHandler;
            _playerPowerUpCollisionHandler = playerPowerUpCollisionHandler;
            _playerProjectileCollisionHandler = playerProjectileCollisionHandler;
        }

        public void Update()
        {
            CheckCollisions<Projectile, PowerUp>(_projectilePowerUpCollisionHandler.Handle);
            CheckCollisions<Player, PowerUp>(_playerPowerUpCollisionHandler.Handle);
            CheckCollisions<Player, Projectile>(_playerProjectileCollisionHandler.Handle);
        }

        private void CheckCollisions<Type1, Type2>(Action<Type1, Type2> handle)
            where Type1: GameObject
            where Type2: GameObject
        {
            var type1s = _objectService.GetAll<Type1>();
            var type2s = _objectService.GetAll<Type2>();

            foreach (var type1 in type1s)
                foreach (var type2 in type2s)
                {
                    var outcome = CheckCollision(type1, type2);
                    if (outcome)
                        handle(type1, type2);
                }
        }

        private bool CheckCollision(GameObject type1, GameObject type2)
        {
            var projectileVertices = PolygonVerticesCompiler.GetVertices(type1);
            var powerUpVertices = PolygonVerticesCompiler.GetVertices(type2);
            var collides = PolygonCollisionDetector.Detect(projectileVertices, powerUpVertices);

            return collides;
        }
    }
}
