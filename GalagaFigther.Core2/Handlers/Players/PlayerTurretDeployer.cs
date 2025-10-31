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

        private readonly float _defaultDeployRate = 20f;

        public PlayerTurretDeployer(IGameDataRegistry gameDataRegistry, IObjectService objectService, IGameObjectPositionService positionService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
            _positionService = positionService;
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
            foreach (var createTurret in modifiers.Turrets.Create.Values)
                DeployTurret(player, modifiers, createTurret);

            turretData.DeployCountdown = _defaultDeployRate * modifiers.TurretDeployRate;
        }

        private void DeployTurret(Player player, PlayerModifiers modifiers, Func<GameObject, PlayerModifiers, List<Turret>> onDeploy)
        {
            var turrets = onDeploy.Invoke(player, modifiers);

            foreach (var turret in turrets)
            {
                PositionTurret(player, turret);
                _objectService.Add(turret);
                
                foreach (var gun in turret.Guns)
                {
                    _positionService.RegisterParent(turret, gun);
                    _objectService.Add(gun);
                }
            }
        }

        private void PositionTurret(Player player, GameObject turretObject)
        {
            var finalPosition = player.Center - turretObject.Rect.Size / 2;
            turretObject.MoveTo(finalPosition.X, finalPosition.Y);
        }
    }
}
