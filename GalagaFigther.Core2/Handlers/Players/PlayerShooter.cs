using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Handlers.Projectiles;
using GalagaFighter.Core2.Models;
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
        private readonly IProjectileShooter _projectileShooter;

        private const float _defaultFireRate = .15f;

        public PlayerShooter(IGameDataRegistry gameDataRegistry,IProjectileShooter projectileShooter)
        {
            _gameDataRegistry = gameDataRegistry;
            _projectileShooter = projectileShooter;
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

            var inputData = _gameDataRegistry.Get<PlayerInputData>(player);
            if (!inputData.Shoot.IsDown)
                return;

            SpawnProjectiles(player, shootData, modifiers);
        }

        private void SpawnProjectiles(Player player, PlayerShootData shootData, PlayerModifiers modifiers)
        {
            var rotationData = _gameDataRegistry.Get<PlayerRotationData>(player);

            foreach (var gun in modifiers.Guns)
                foreach (var barrel in gun.Shoot(player))
                    _projectileShooter.Shoot(player, barrel.Value, barrel.Key);

            shootData.LastShotLeft = !shootData.LastShotLeft;
            shootData.ShotCountdown = _defaultFireRate * modifiers.Stats.FireRate;
        }
    }
}
