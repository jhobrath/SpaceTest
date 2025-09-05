using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Linq;
using System.Numerics;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerProjectileSpawner
    {
        PlayerShootState SpawnProjectiles(Player player, EffectModifiers modifiers, ButtonState shootButton);
    }

    public class PlayerProjectileSpawner : IPlayerProjectileSpawner
    {
        private readonly IObjectService _objectService;
        private readonly IProjectileController _projectileController;
        private bool _lastGunLeft = false;
        private Projectile? _lastProjectile = null;

        public PlayerProjectileSpawner(IObjectService objectService, IProjectileController projectileController)
        {
            _objectService = objectService;
            _projectileController = projectileController;
        }

        public PlayerShootState SpawnProjectiles(Player player, EffectModifiers modifiers, ButtonState shootButton)
        {
            var windUpState = GetWindupState(player, modifiers, shootButton);
            if (windUpState != null)
                return windUpState.Value;

            var shooters = modifiers.Phantoms.Select(x => new { IsPhantom = true, x.Center }).ToList();
            shooters.Add(new { IsPhantom = false, player.Center });

            foreach(var shooter in shooters)
            { 
                var spawnPosition = GetSpawnPosition(player, modifiers, shooter.Center);
                SpawnProjectile(player, modifiers, spawnPosition, shooter.IsPhantom);

                if (modifiers.Stats.DoubleShot)
                {
                    var spawnPosition2 = GetSpawnPosition(player, modifiers, shooter.Center);
                    SpawnProjectile(player, modifiers, spawnPosition2, shooter.IsPhantom);
                    return PlayerShootState.ShootBoth;
                }
            }

            if (_lastProjectile?.Modifiers.WindUpDuration > 0f)
                return _lastGunLeft ? PlayerShootState.ShootRight : PlayerShootState.ShootLeft;

            return _lastGunLeft ? PlayerShootState.ShootLeft : PlayerShootState.ShootRight;
        }

        private PlayerShootState? GetWindupState(Player player, EffectModifiers modifiers, ButtonState shootButton)
        {
            if (modifiers.Projectile.WindUpDuration == 0f)
                return null;

            if (!shootButton.IsDown)
            {
                if (_lastProjectile != null && _lastProjectile.Modifiers.WindUpDuration > 0f)
                    return _lastGunLeft ? PlayerShootState.WindUpLeft : PlayerShootState.WindUpRight;
                else
                    return PlayerShootState.Idle;
            }

            if (_lastProjectile != null && (_lastProjectile?.Modifiers.WindUpDuration ?? 0f) > 0f)
                return _lastGunLeft ? PlayerShootState.WindUpLeft : PlayerShootState.WindUpRight;

            return null;
        }

        private Vector2 GetSpawnPosition(Player player, EffectModifiers modifiers, Vector2? center = null)
        {
            center ??= player.Center;
            var playerWidth = player.Rect.Width;
            var playerHeight = player.Rect.Height;

            var spawnX = player.IsPlayer1 ? player.Rect.X + playerWidth : player.Rect.X;
            var spawnY = center.Value.Y;

            var position = new Vector2(spawnX, spawnY);
            return position;
        }

        private void SpawnProjectile(Player player, EffectModifiers modifiers, Vector2 spawnPosition, bool isPhantom)
        {
            foreach (var projectileFunc in modifiers.Projectile.OnShootProjectiles)
            {
                var projectile = ShootProjectile(player, modifiers, spawnPosition, projectileFunc, isPhantom, _lastGunLeft);
                _lastProjectile = projectile;
            }

            if(!isPhantom)
                _lastGunLeft = !_lastGunLeft;
        }

        private Projectile ShootProjectile(Player player, EffectModifiers modifiers, Vector2 spawnPosition, Func<IProjectileController, Player, Vector2, PlayerProjectile, Projectile> projectileFunc, bool isPhantom, bool lastGunLeft)
        {
            var projectileModifiers = modifiers.Projectile.Clone();

            projectileModifiers.DamageMultiplier *= player.BaseStats.Damage;
            projectileModifiers.OnClone?.Invoke(projectileModifiers);

            var projectile = projectileFunc(_projectileController.Create(), player, spawnPosition, projectileModifiers);
            SetRotation(projectile);
            SetProjectilePlacement(player, modifiers, lastGunLeft, projectile);

            projectile.Modifiers.OnShoot?.Invoke(projectile);
            projectile.Modifiers.Untouchable |= isPhantom;

            _objectService.AddGameObject(projectile);
            return projectile;
        }

        private static void SetProjectilePlacement(Player player, EffectModifiers modifiers, bool lastGunLeft, Projectile projectile)
        {
            if (projectile.SpawnOffset == Vector2.Zero)
                return;

            projectile.Move(x: projectile.SpawnOffset.X * (player.IsPlayer1 ? 1 : -1) * modifiers.Display.SizeMultiplier.X);
            projectile.Move(y: projectile.SpawnOffset.Y * (player.IsPlayer1 ? 1 : -1) * (lastGunLeft ? 1 : -1) * modifiers.Display.SizeMultiplier.Y);
            projectile.Move(x: (player.IsPlayer1 ? 0 : -1)* projectile.Rect.Width);
            projectile.Move(y: (player.IsPlayer1 ? -1 : -1) * (projectile.Rect.Height / 2));
        }

        private void SetRotation(Projectile projectile)
        {
            if (projectile.Speed.X < 0)
                projectile.Rotation += -180f;
        }
    }
}