using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Turrets;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
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
        private readonly IObjectService _objectService;

        private readonly float _defaultDeployRate = 20f;

        public PlayerTurretDeployer(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        public void Deploy(Player player, float frameTime)
        {
            var turretData = _gameDataRegistry.Get<PlayerTurretData>(player);
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);

            if (turretData.DeployCountdown > 0)
            {
                turretData.DeployCountdown -= frameTime;
                return;
            }

            var inputData = _gameDataRegistry.Get<PlayerInputData>(player);
            if (!inputData.DeployTurret.IsDown)
                return;

            Deploy(player, turretData, modifiers);
        }

        private void Deploy(Player player, PlayerTurretData turretData, PlayerModifiers modifiers)
        {
            foreach (var onDeploy in modifiers.Turret.OnDeploy)
                DeployTurret(player, onDeploy);

            turretData.DeployCountdown = _defaultDeployRate * modifiers.TurretDeployRate;
        }

        private void DeployTurret(Player player, KeyValuePair<string, Func<Guid, Vector2, List<GameObject>>> onDeploy)
        {
            var turretObjects = onDeploy.Value(player.Id, player.Center);

            foreach (var turretObject in turretObjects)
            {
                PositionTurret(player, turretObject);
                _objectService.Add(turretObject);
            }
        }

        private void PositionTurret(Player player, GameObject turretObject)
        {
            turretObject.Move(-turretObject.Width / 2, -turretObject.Height / 2);
        }
    }
}
