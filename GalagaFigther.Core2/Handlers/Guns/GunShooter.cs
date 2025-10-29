using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Handlers.Projectiles;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Guns;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Guns
{
    public interface IGunShooter
    {
        void Shoot(Gun gun);
    }
    public class GunShooter : IGunShooter
    {
        private readonly IObjectService _objectService;
        private readonly IProjectileShooter _projectileShooter;
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IGameObjectPositionService _gameObjectPositionService;

        public GunShooter(IObjectService objectService, IProjectileShooter projectileShooter,
            IGameDataRegistry gameDataRegistry, IGameObjectPositionService gameObjectPositionService)
        {
            _objectService = objectService;
            _projectileShooter = projectileShooter;
            _gameDataRegistry = gameDataRegistry;
            _gameObjectPositionService = gameObjectPositionService;
        }

        public void Shoot(Gun gun)
        {
            if (!gun.ShotDue)
                return;
            

            var player = _objectService.GetPlayer(gun);
            foreach (var barrel in gun.Shoot(player))
                _projectileShooter.Shoot(gun, barrel.Value, barrel.Key);

            gun.ShotDue = false;
            
            if (gun.Barrels.Count == 1)
                SetRecoil(gun);
        }

        private void SetRecoil(Gun gun)
        {
            var recoilData = _gameDataRegistry.Get<GunRecoilData>(gun);
            recoilData.RecoilLifetime = 0f;
            recoilData.RecoilPeriod = .5f;
            recoilData.RecoilDistance = 10f;
        }

        private Vector2 GetRotatedOffset(GameObject objectShooting, Vector2 offset)
        {
            var offsetRotationRadians = (objectShooting.WorldRotation - 90) * MathF.PI / 180f;
            return new Vector2(
                (offset.X * MathF.Cos(offsetRotationRadians) - offset.Y * MathF.Sin(offsetRotationRadians)),
                (offset.X * MathF.Sin(offsetRotationRadians) + offset.Y * MathF.Cos(offsetRotationRadians))
            );
        }
    }
}
