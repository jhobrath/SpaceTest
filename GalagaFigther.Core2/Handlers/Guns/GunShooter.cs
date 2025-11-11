using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.GameObjects.Turrets;
using GalagaFighter.Core2.Handlers.Projectiles;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Guns;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Static;
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
            if (!gun.ShotRequested)
                return;

            var player = _objectService.GetPlayer(gun);
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);

            if (gun.CountDown < gun.FireRate * modifiers.Stats.FireRateMultiplier)
                return;

            gun.CountDown = 0f;
            gun.ShotRequested = false;


            var shootData = _gameDataRegistry.Get<PlayerShootData>(player);
            foreach (var barrel in gun.Shoot(player))
            {
                var proj = _projectileShooter.Shoot(gun, barrel.Value, barrel.Key);

                if (gun.IsPlayerGun && gun.RecoveryTime > 0f && shootData.RecoveryTime < .02f)
                {
                    shootData.RecoveryTime += gun.RecoveryTime;
                    var projVector = Vector2.Normalize(proj.Speed) * -600f;
                    player.Hurry(projVector.X, projVector.Y);
                }
            }

            if (gun.Barrels.Count == 1)
                SetRecoil(gun);
        }

        private void SetRecoil(Gun gun)
        {
            var recoilData = _gameDataRegistry.Get<GunRecoilData>(gun);
            recoilData.RecoilLifetime = 0f;
            recoilData.RecoilPeriod = .5f;
            recoilData.RecoilDistance = gun.Barrels[0].Recoil;
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
