using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using Raylib_cs;
using System;
using System.Numerics;
using System.Collections.Generic;
using GalagaFighter.Core.Controllers;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerShooter
    {
        void Shoot(Player player, EffectModifiers modifiers);
    }
    public class PlayerShooter : IPlayerShooter
    {
        private readonly Dictionary<Guid, float> _fireRateTimer = new();
        private readonly Dictionary<Guid, bool> _lastGunLeft = new();
        private readonly Dictionary<Guid, Projectile?> _lastProjectile = new();

        protected readonly IObjectService _objectService;
        private readonly IInputService _inputService;
        private readonly IProjectileController _projectileController;

        protected virtual float EffectiveFireRate => 1.2f * (float)Math.Pow(0.8f, 5);

        public PlayerShooter(IInputService inputService, IObjectService objectService, IProjectileController projectileUpdater)
        {
            _objectService = objectService;
            _inputService = inputService;
            _projectileController = projectileUpdater;
        }

        public void Shoot(Player player, EffectModifiers modifiers)
        {
            var canShoot = GetCanShoot(player, modifiers);
            if (!canShoot)
                return;

            var spawnPosition = GetSpawnPosition(player, modifiers);
            SpawnProjectile(player, modifiers, spawnPosition);

            if(modifiers.Stats.DoubleShot)
            {
                var spawnPosition2 = GetSpawnPosition(player, modifiers);
                SpawnProjectile(player, modifiers, spawnPosition2);
            }
        }

        protected virtual bool GetCanShoot(Player player, EffectModifiers modifiers)
        {
            if (!_fireRateTimer.ContainsKey(player.Id))
                _fireRateTimer[player.Id] = 0f;

            _fireRateTimer[player.Id] += Raylib.GetFrameTime();

            var shoot = _inputService.GetShoot(player.Id);
            if (!shoot)
                return false;

            var fireRate = modifiers.Stats.FireRateMultiplier * EffectiveFireRate;
            if (_fireRateTimer[player.Id] < fireRate)
                return false;

            _fireRateTimer[player.Id] = 0;
            return true;
        }

        private Vector2 GetSpawnPosition(Player player, EffectModifiers modifiers)
        {
            var playerWidth = player.CurrentFrameRect.Width;
            var playerHeight = player.CurrentFrameRect.Height;

            var spawnX = player.IsPlayer1 ? player.Rect.X + playerWidth : player.Rect.X;
            var spawnY = player.Rect.Y + playerHeight / 2;

            var position = new Vector2(spawnX, spawnY);
            return position;
        }

        private void SpawnProjectile(Player player, EffectModifiers modifiers, Vector2 spawnPosition)
        { 
            if (!_lastGunLeft.ContainsKey(player.Id))
                _lastGunLeft[player.Id] = false;

            if (!_lastProjectile.ContainsKey(player.Id))
                _lastProjectile[player.Id] = null;

            if (_lastProjectile[player.Id] != null && _lastProjectile[player.Id].Modifiers.WindUpDuration > 0f)
                return;

            foreach(var projectileFunc in modifiers.Projectile.Projectiles)
            {
                var projectileModifiers = modifiers.Projectile.Clone();

                var projectile = projectileFunc(_projectileController.Create(), player, spawnPosition, projectileModifiers);
                SetRotation(projectile);
                
                projectile.Move(x: projectile.SpawnOffset.X * (player.IsPlayer1 ? 1 : -1) * modifiers.Display.SizeMultiplier.X);
                projectile.Move(y: projectile.SpawnOffset.Y * (_lastGunLeft[player.Id] ? 1 : -1) * modifiers.Display.SizeMultiplier.Y);

                //Undo offset from spawn position
                projectile.Move(x: player.IsPlayer1 ? 0 : -projectile.Rect.Width);
                projectile.Move(y: -(projectile.Rect.Height / 2));

                projectile.Modifiers.OnShoot?.Invoke(projectile);

                _objectService.AddGameObject(projectile);
                _lastProjectile[player.Id] = projectile;
            }

            _lastGunLeft[player.Id] = !_lastGunLeft[player.Id];
        }

        protected virtual void SetRotation(Projectile projectile)
        {
            if (projectile.Speed.X < 0)
                projectile.Rotation += -180f;
        }
    }
}
