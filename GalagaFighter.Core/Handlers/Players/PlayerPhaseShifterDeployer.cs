using GalagaFighter.Core.Handlers.Collisions;
using GalagaFighter.Core.Models;
using GalagaFighter.Core.Models.Debris;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerPhaseShifterDeployer
    {
        void Deploy(Player player);
    }
    public class PlayerPhaseShifterDeployer : IPlayerPhaseShifterDeployer
    {
        private readonly IInputService _inputService;
        private readonly IObjectService _objectService;

        private const float _minimumDeployChangeTime = 1f;
        private float _deployTime = _minimumDeployChangeTime;

        private const float _minimumDeadShifterTimeout = 10f;
        private float _deadShifterTime = _minimumDeadShifterTimeout;


        private List<PhaseShifter> _shifters = [];

        public PlayerPhaseShifterDeployer(IInputService inputService, IObjectService objectService)
        {
            _inputService = inputService;
            _objectService = objectService;

        }

        public void Deploy(Player player)
        {
            _deployTime += Raylib.GetFrameTime();
            _deadShifterTime += Raylib.GetFrameTime();

            DeployShifter(player);

            for (var i = _shifters.Count - 1;i>=0;i--)
            {
                var shifter = _shifters[i];
                UpdateShifter(player, shifter);
                DestroyShifter(shifter);
            }
        }

        private void UpdateShifter(Player player, PhaseShifter shifter)
        {
            shifter.Sprite.Update(Raylib.GetFrameTime());
        }

        private void DeployShifter(Player player)
        {
            if (_deadShifterTime < _minimumDeadShifterTimeout)
                return;

            var deploy = _inputService.GetDeploy(player.Id);
            if (deploy.IsPressed && _deployTime > _minimumDeployChangeTime)
            {
                if (_shifters.Count == 0)
                {
                    var shifter = new PhaseShifter(player);
                    _shifters.Add(shifter);
                    _objectService.AddGameObject(shifter);
                }
                else
                {
                    _shifters[0].IsActive = false;
                    _shifters.Remove(_shifters[0]);
                    _deadShifterTime = 0f;
                }

                _deployTime = 0f;
            }
        }

        private void DestroyShifter(PhaseShifter shifter)
        {
            if (shifter.Health > 0f)
                return;

            shifter.IsActive = false;
            _shifters.Remove(shifter);
            _deadShifterTime = 0f;
        }
    }
}