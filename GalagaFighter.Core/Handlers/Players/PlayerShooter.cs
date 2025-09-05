using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Handlers.Projectiles;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Linq;
using System.Numerics;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerShooter
    {
        PlayerShootState Shoot(Player player, EffectModifiers modifiers);
        void ShootOneTime(Player player, EffectModifiers modifiers);
    }

    public enum PlayerShootState
    {
        WindUpLeft,
        WindUpRight,
        WindUpBoth,
        ShootLeft,
        ShootRight,
        ShootBoth,
        Magnet,
        Idle
    }

    public class PlayerShooter : IPlayerShooter
    {
        private float _fireRateTimer = 0f;
        private bool _lastGunLeft = false;
        private Projectile? _lastProjectile = null;

        protected readonly IObjectService _objectService;
        private readonly IInputService _inputService;
        private readonly IProjectileController _projectileController;
        private readonly IMagnetProjectileService _magnetProjectileService;
        private readonly IPlayerManagerFactory _playerManagerFactory;
        private readonly IPlayerProjectileSpawner _projectileSpawner;

        protected virtual float EffectiveFireRate => 1.2f * (float)Math.Pow(0.8f, 5);


        private float _lastFireRateFactor = 1.0f;


        public PlayerShooter(IInputService inputService, IObjectService objectService, 
            IProjectileController projectileUpdater, IMagnetProjectileService magnetProjectileService,
            IPlayerManagerFactory playerManagerFactory, IPlayerProjectileSpawner projectileSpawner)
        {
            _objectService = objectService;
            _inputService = inputService;
            _projectileController = projectileUpdater;
            _magnetProjectileService = magnetProjectileService;
            _playerManagerFactory = playerManagerFactory;
            _projectileSpawner = projectileSpawner;
        }

        public PlayerShootState Shoot(Player player, EffectModifiers modifiers)
        {
            var shootButton = _inputService.GetShoot(player.Id);
            UpdateShootMeter(player, modifiers, shootButton);

            var canShoot = GetCanShoot(player, modifiers, shootButton);
            if (!canShoot)
                return PlayerShootState.Idle;

            if (modifiers.Magnetic)
            {
                var shoot = _inputService.GetShoot(player.Id);
                if(shoot.HeldDuration == Raylib.GetFrameTime())
                    AudioService.PlayMagnetSound();

                _magnetProjectileService.Magnetize(player);
                return PlayerShootState.ShootBoth; 
            }

            return _projectileSpawner.SpawnProjectiles(player, modifiers, shootButton);
        }

        private void UpdateShootMeter(Player player, EffectModifiers modifiers, ButtonState shootButton)
        {
            if (!modifiers.AffectedByShootMeter)
                return;

            var resourceManager = _playerManagerFactory.GetResourceManager(player.Id);
            resourceManager.HandleShootMeter(shootButton);

            if (modifiers.WereReset)
                _lastFireRateFactor = 1f;

            var shootMeter = .5f + .5f*resourceManager.ShootMeter;
            
            modifiers.Stats.FireRateMultiplier *= _lastFireRateFactor;
            modifiers.Stats.FireRateMultiplier /= shootMeter;

            _lastFireRateFactor = shootMeter;
        }

        protected virtual bool GetCanShoot(Player player, EffectModifiers modifiers, ButtonState shootButton)
        {
            _fireRateTimer += Raylib.GetFrameTime();

            if (modifiers.Projectile.WindUpDuration > 0f)
                return true;

            if (modifiers.Magnetic)
            {
                var canMagnet = GetCanShootMagnet(player, shootButton);
                if (canMagnet.HasValue)
                    return canMagnet.Value;
            }

            if (!shootButton)
                return false;

            var fireRate = modifiers.Stats.FireRateMultiplier * EffectiveFireRate * player.BaseStats.FireRateMultiplier;
            if (_fireRateTimer < fireRate)
                return false;

            _fireRateTimer = 0;
            return true;
        }

        private bool? GetCanShootMagnet(Player player, ButtonState shoot)
        {
            if (shoot)
            {
                if (shoot.HeldDuration == Raylib.GetFrameTime())
                    AudioService.PlayMagnetSound();

                return true;
            }
            else if (shoot.WasReleased)
            {
                _magnetProjectileService.DeMagnetize(player);
                return false;
            }

            return null;
        }

        public void ShootOneTime(Player player, EffectModifiers modifiers)
        {
            foreach (var projectileFunc in modifiers.Projectile.OneTimeProjectiles)
            {
                var projectile = projectileFunc(_projectileController, player, new Vector2(player.Center.X, player.Center.Y), modifiers.Projectile.Clone());
                _objectService.AddGameObject(projectile);
            }

            modifiers.Projectile.OneTimeProjectiles.Clear();
        }
    }
}
