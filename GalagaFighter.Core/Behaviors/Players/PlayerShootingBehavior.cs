using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Behaviors.Players.Updates;
using GalagaFighter.Core.Behaviors.Projectiles;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Players
{
    public class PlayerShootingBehavior : IPlayerShootingBehavior
    {
        private float _fireRateTimer = 0f;
        protected bool _lastGunLeft = false;

        protected readonly IObjectService _objectService;

        public PlayerShootingBehavior(IObjectService objectService)
        {
            _objectService = objectService;
        }

        protected virtual Vector2 SpawnOffset => new Vector2(-10, 30);
        protected virtual float EffectiveFireRate => 1.2f*(float)Math.Pow(0.8f, 5);

        public void Apply(Player player, PlayerInputUpdate inputUpdate, PlayerMovementUpdate movementUpdate)
        {
            var canShoot = GetCanShoot(player, inputUpdate);

            if (!canShoot)
                return;

            var spawnSize = GetBaseSize(player);
            var spawnSpeed = GetSpawnSpeed(player, movementUpdate);
            var spawnPosition = GetSpawnPosition(player, spawnSize);
            var projectile = Create(player.Id, spawnPosition, spawnSize, spawnSpeed);

            SetRotation(projectile);

            _objectService.AddGameObject(projectile);
            _lastGunLeft = !_lastGunLeft;
        }

        protected virtual bool GetCanShoot(Player player, PlayerInputUpdate inputUpdate)
        {
            _fireRateTimer += Raylib.GetFrameTime();

            var fireRate = player.Stats.FireRate * EffectiveFireRate;
            if (!inputUpdate.Shoot || _fireRateTimer < fireRate)
                return false;

            _fireRateTimer = 0;
            return true;
        }

        protected virtual void SetRotation(Projectile projectile)
        {
            if (projectile.Speed.X < 0)
                projectile.Rotation = -180f;
        }

        protected virtual Vector2 GetSpawnPosition(Player player, Vector2 spawnSize)
        {
            var playerWidth = player.Rect.Width * player.Display.Size;
            var playerHeight = player.Rect.Height * player.Display.Size;
            var spawnX = player.IsPlayer1 ? player.Rect.X + playerWidth : player.Rect.X;
            var spawnY = player.Rect.Y + playerHeight / 2;
            spawnX += SpawnOffset.X * (player.IsPlayer1 ? 1 : -1);
            spawnY += SpawnOffset.Y * (_lastGunLeft ? 1 : -1);
            
            //Correct for initial size
            spawnX += player.IsPlayer1 ? 0 : -spawnSize.X;
            spawnY += -(spawnSize.Y / 2);

            var position = new Vector2(spawnX, spawnY);
            return position;
        }

        protected virtual Vector2 GetBaseSpeed()
        {
            return DefaultProjectile.BaseSpeed;
        }

        protected virtual Vector2 GetSpawnSpeed(Player player, PlayerMovementUpdate movementUpdate)
        {
            var baseSpeed = GetBaseSpeed();
            
            var playerVerticalSpeed = ((movementUpdate.To.Y - movementUpdate.From.Y)/ Raylib.GetFrameTime()) / 3;
            var playerHorizontalSpeed = 0f;

            return new Vector2(baseSpeed.X*(player.IsPlayer1 ? 1 : -1) + playerHorizontalSpeed, baseSpeed.Y + playerVerticalSpeed);
        }

        protected virtual Vector2 GetBaseSize(Player player)
        {
            return DefaultProjectile.BaseSize;
        }

        protected virtual Projectile Create(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            var projectile = Spawn(owner, initialPosition, initialSize, initialSpeed);
            projectile.SetMovementBehavior(new ProjectileMovementBehavior());
            projectile.SetDestroyBehavior(new ProjectileDestroyBehavior());
            projectile.SetCollisionBehavior(new ProjectileCollisionBehavior(_objectService));
            return projectile;
        }

        protected virtual Projectile Spawn(Guid owner, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            return new DefaultProjectile(owner, initialPosition, initialSize, initialSpeed);
        }
    }
}
