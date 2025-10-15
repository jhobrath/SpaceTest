using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.GameObjects.Projectiles;
using GalagaFigther.Core2.Models.Players;
using GalagaFigther.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Handlers.Players
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

        public PlayerShooter(IInputService inputService, IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _inputService = inputService;
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        public void Shoot(Player player, float frameTime)
        {
            var shootData = _gameDataRegistry.Get<PlayerShootData>();
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
            var posX = player.X + player.Width + modifiers.GunOffset.X;
            var posY = player.Center.Y + modifiers.GunOffset.Y * (shootData.LastShotLeft ? -1 : 1);
            var position = new Vector2(posX, posY);

            foreach (var onShoot in modifiers.Projectile.OnShoot)
                ShootProjectile(player, position, onShoot);

            shootData.LastShotLeft = !shootData.LastShotLeft;
            shootData.ShotCountdown = .15f;
        }

        private void ShootProjectile(Player player, Vector2 position, KeyValuePair<string, Func<Guid, Vector2, List<GameObject>>> onShoot)
        {
            var projectiles = onShoot.Value(player.Id, position);
            foreach (var projectile in projectiles)
            {
                projectile.Move(y: -projectile.Height / 2);
                _objectService.Add(projectile);
            }
        }
    }
}
