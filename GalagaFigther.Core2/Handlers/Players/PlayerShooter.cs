using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Players
{
    public interface IPlayerShooter
    {
        void Shoot(Player player, float frameTime);
    }
    public class PlayerShooter : IPlayerShooter
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IInputService _inputService;
        private readonly IObjectService _objectService;

        private const float _defaultFireRate = .15f;

        public PlayerShooter(IInputService inputService, IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _inputService = inputService;
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        public void Shoot(Player player, float frameTime)
        {
            var shootData = _gameDataRegistry.Get<PlayerShootData>(player);
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);

            if (shootData.ShotCountdown > 0)
            {
                shootData.ShotCountdown -= frameTime;
                return;
            }

            if (!_inputService.Shoot.IsDown)
                return;

            Shoot(player, shootData, modifiers);
        }

        private void Shoot(Player player, PlayerShootData shootData, PlayerModifiers modifiers)
        {
            var rotationData = _gameDataRegistry.Get<PlayerRotationData>(player);
            
            // Use the correct gun position based on which shot (left/right)
            var gunOffset = shootData.LastShotLeft ? 
                new Vector2(modifiers.GunOffset.X, -modifiers.GunOffset.Y) : // Right gun
                modifiers.GunOffset; // Left gun

            foreach (var onShoot in modifiers.Projectile.OnShoot)
                ShootProjectile(player, gunOffset, onShoot, rotationData);

            shootData.LastShotLeft = !shootData.LastShotLeft;
            shootData.ShotCountdown = _defaultFireRate * modifiers.FireRate;
        }

        private void ShootProjectile(Player player, Vector2 gunOffset, KeyValuePair<string, Func<Guid, Vector2, List<GameObject>>> onShoot, PlayerRotationData rotationData)
        {
            var offsetRotationRadians = (player.Rotation - 90) * MathF.PI / 180f;
            var gunTip = new Vector2(
                player.Center.X + (gunOffset.X * MathF.Cos(offsetRotationRadians) - gunOffset.Y * MathF.Sin(offsetRotationRadians)),
                player.Center.Y + (gunOffset.X * MathF.Sin(offsetRotationRadians) + gunOffset.Y * MathF.Cos(offsetRotationRadians))
            );

            var projectiles = onShoot.Value(player.Id, gunTip);
            
            foreach (var projectile in projectiles)
            {
                PositionProjectile(player, gunTip, projectile);
                _objectService.Add(projectile);
            }
        }

        private void PositionProjectile(Player player, Vector2 gunTip, GameObject projectile)
        {
            var speedLength = projectile.Speed.Length();

            var gunRotationRadians = (90 - player.Rotation) * MathF.PI / 180f;
            var speedXPct = MathF.Cos(gunRotationRadians);
            var speedYPct = -MathF.Sin(gunRotationRadians);

            projectile.HurryTo(speedLength * speedXPct, speedLength * speedYPct);

            projectile.Rotation = player.Rotation;

            var halfWidth = projectile.Width / 2f;
            var halfHeight = projectile.Height / 2f;

            projectile.MoveTo(gunTip.X - halfWidth, gunTip.Y - halfHeight);
        }
    }
}
