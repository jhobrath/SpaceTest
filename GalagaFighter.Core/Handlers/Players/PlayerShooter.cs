using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Handlers.Projectiles;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
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

        protected virtual float EffectiveFireRate => 1.2f * (float)Math.Pow(0.8f, 5);

        public PlayerShooter(IInputService inputService, IObjectService objectService, IProjectileController projectileUpdater,
            IMagnetProjectileService magnetProjectileService)
        {
            _objectService = objectService;
            _inputService = inputService;
            _projectileController = projectileUpdater;
            _magnetProjectileService = magnetProjectileService;
        }

        public PlayerShootState Shoot(Player player, EffectModifiers modifiers)
        {
            var canShoot = GetCanShoot(player, modifiers, out ButtonState shoot);
            if (!canShoot)
            {
                if (shoot.HeldDuration > 0f && modifiers.Projectile.WindUpDuration > 0f)
                    return _lastGunLeft
                        ? player.IsPlayer1 ? PlayerShootState.WindUpLeft : PlayerShootState.WindUpRight
                        : player.IsPlayer1 ? PlayerShootState.WindUpRight : PlayerShootState.WindUpLeft;

                return PlayerShootState.Idle;
            }

            if (modifiers.Magnetic)
            {
                if(shoot.HeldDuration == Raylib.GetFrameTime())
                    AudioService.PlayMagnetSound();

                _magnetProjectileService.Magnetize(player);
                return PlayerShootState.Magnet;
            }

            if (_lastProjectile != null && (_lastProjectile?.Modifiers.WindUpDuration ?? 0f) > 0f)
                return _lastGunLeft
                    ? player.IsPlayer1 ? PlayerShootState.WindUpLeft : PlayerShootState.WindUpRight
                    : player.IsPlayer1 ? PlayerShootState.WindUpRight : PlayerShootState.WindUpLeft;

            var spawnPosition = GetSpawnPosition(player, modifiers);
            SpawnProjectile(player, modifiers, spawnPosition);

            if (modifiers.Stats.DoubleShot)
            {
                var spawnPosition2 = GetSpawnPosition(player, modifiers);
                SpawnProjectile(player, modifiers, spawnPosition2);
                return PlayerShootState.ShootBoth;
            }

            if (_lastProjectile?.Modifiers.WindUpDuration > 0f)
                return _lastGunLeft
                ? player.IsPlayer1 ? PlayerShootState.ShootRight : PlayerShootState.ShootLeft
                : player.IsPlayer1 ? PlayerShootState.ShootLeft : PlayerShootState.ShootRight;

            return _lastGunLeft
                ? player.IsPlayer1 ? PlayerShootState.ShootLeft : PlayerShootState.ShootRight
                : player.IsPlayer1 ? PlayerShootState.ShootRight : PlayerShootState.ShootLeft;
        }

        protected virtual bool GetCanShoot(Player player, EffectModifiers modifiers, out ButtonState shoot)
        {
            _fireRateTimer += Raylib.GetFrameTime();

            shoot = _inputService.GetShoot(player.Id);
            if (modifiers.Magnetic)
            {
                var canMagnet = GetCanShootMagnet(player, shoot);
                if (canMagnet.HasValue)
                    return canMagnet.Value;
            }

            if (!shoot)
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

        private Vector2 GetSpawnPosition(Player player, EffectModifiers modifiers)
        {
            var playerWidth = player.Rect.Width;
            var playerHeight = player.Rect.Height;

            var spawnX = player.IsPlayer1 ? player.Rect.X + playerWidth : player.Rect.X;
            var spawnY = player.Rect.Y + playerHeight / 2;

            var position = new Vector2(spawnX, spawnY);
            return position;
        }

        private void SpawnProjectile(Player player, EffectModifiers modifiers, Vector2 spawnPosition)
        {
            foreach (var projectileFunc in modifiers.Projectile.OnShootProjectiles)
                ShootProjectile(player, modifiers, spawnPosition, projectileFunc);

            _lastGunLeft = !_lastGunLeft;
        }

        private void ShootProjectile(Player player, EffectModifiers modifiers, Vector2 spawnPosition, Func<IProjectileController, Player, Vector2, PlayerProjectile, Projectile> projectileFunc)
        {
            var projectileModifiers = modifiers.Projectile.Clone();

            projectileModifiers.DamageMultiplier *= player.BaseStats.Damage;

            var projectile = projectileFunc(_projectileController.Create(), player, spawnPosition, projectileModifiers);
            SetRotation(projectile);

            projectile.Move(x: projectile.SpawnOffset.X * (player.IsPlayer1 ? 1 : -1) * modifiers.Display.SizeMultiplier.X);
            projectile.Move(y: projectile.SpawnOffset.Y * (_lastGunLeft ? 1 : -1) * modifiers.Display.SizeMultiplier.Y);

            //Undo offset from spawn position
            projectile.Move(x: player.IsPlayer1 ? 0 : -projectile.Rect.Width);
            projectile.Move(y: -(projectile.Rect.Height / 2));

            projectile.Modifiers.OnShoot?.Invoke(projectile);

            _objectService.AddGameObject(projectile);
            _lastProjectile = projectile;
        }

        protected virtual void SetRotation(Projectile projectile)
        {
            if (projectile.Speed.X < 0)
                projectile.Rotation += -180f;
        }

        public void ShootOneTime(Player player, EffectModifiers modifiers)
        {
            Raylib.DrawRectangleLines((int)player.Rect.X, (int)player.Rect.Y, (int)player.Rect.Width, (int)player.Rect.Height, Color.Red);

            foreach (var projectileFunc in modifiers.Projectile.OneTimeProjectiles)
            {
                var projectile = projectileFunc(_projectileController, player, new Vector2(player.Center.X, player.Center.Y), modifiers.Projectile);
                _objectService.AddGameObject(projectile);
            }

            modifiers.Projectile.OneTimeProjectiles.Clear();
        }
    }
}
