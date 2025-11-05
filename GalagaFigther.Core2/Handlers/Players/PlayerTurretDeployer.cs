using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Turrets;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Players
{
    public interface IPlayerTurretDeployer
    {
        void Deploy(Player player, float frameTime);
    }

    public class PlayerTurretDeployer : IPlayerTurretDeployer
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IGameObjectPositionService _positionService;
        private readonly IObjectService _objectService;

        private readonly float _defaultDeployRate = 1f;

        public PlayerTurretDeployer(IGameDataRegistry gameDataRegistry, IObjectService objectService, IGameObjectPositionService positionService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
            _positionService = positionService;
        }

        public void Deploy(Player player, float frameTime)
        {
            var turretData = _gameDataRegistry.Get<PlayerTurretData>(player);

            if (turretData.DeployCountdown > 0)
            {
                turretData.DeployCountdown -= frameTime;
                return;
            }

            var inputData = _gameDataRegistry.Get<PlayerInputData>(player);
            if (!inputData.DeployTurret.IsDown)
                return;

            if (!player.Turrets.Any())
                return;

            Deploy(player, turretData);
        }

        private void Deploy(Player player, PlayerTurretData turretData)
        {
            var turret = player.Turrets[player.TurretIndex];
            player.Turrets.RemoveAt(player.TurretIndex);

            if (player.TurretIndex > player.Turrets.Count)
                player.TurretIndex = Math.Max(0, player.Turrets.Count - 1);
            
            DeployTurret(player, turret);
            turretData.DeployCountdown = _defaultDeployRate;
        }

        private void DeployTurret(Player player, Turret turret)
        {
            PositionTurret(player, turret);
            _objectService.Add(turret);

            foreach (var gun in turret.Guns)
            {
                _positionService.RegisterParent(turret, gun);
                _objectService.Add(gun);
            }
        }

        private void PositionTurret(Player player, GameObject turretObject)
        {
            var finalPosition = player.Center - turretObject.Rect.Size / 2;
            turretObject.MoveTo(finalPosition.X, finalPosition.Y);
        }
    }
}
