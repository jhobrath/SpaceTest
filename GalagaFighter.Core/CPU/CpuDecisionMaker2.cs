using GalagaFighter.Core.CPU.Gambits;
using GalagaFighter.Core.Handlers.Collisions;
using GalagaFighter.Core.Models.Debris;
using GalagaFighter.Core.Models.Effects.Statuses;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.CPU
{
    public interface ICpuDecisionMaker2 : ICpuDecisionMaker
    {
        void InitializePlayer(Player player);
    }

    public class CpuDecisionMaker2 : ICpuDecisionMaker
    {
        private bool _moveDown = false;
        private bool _moveUp = false;
        private bool _switch = false;
        private bool _deploy = false;
        private bool _shoot = false;

        private readonly IOpponentBulletWatcher _opponentBulletWatcher;
        private readonly IObjectService _objectService;
        private readonly IPlayerManagerFactory _playerManagerFactory;
        private Player? _player;

        private bool _deployWasReleased = true;
        private float _shootRefillWait = 2f;

        public CpuDecisionMaker2(IOpponentBulletWatcher opponentBulletWatcher,
            IObjectService objectService, IPlayerManagerFactory playerManagerFactory)
        {
            _opponentBulletWatcher = opponentBulletWatcher;
            _objectService = objectService;
            _playerManagerFactory = playerManagerFactory;
        }

        public void InitializePlayer(Player player)
        {
            _player = player;
        }

        public bool IsDeployDown()
        {
            return _deploy;
        }

        public bool IsMoveLeftDown()
        {
            return _moveDown;
        }

        public bool IsMoveRightDown()
        {
            return _moveUp;
        }

        public bool IsShootDown()
        {
            return _shoot;
        }

        public bool IsSwitchDown()
        {
            return _switch;
        }

        public void Update()
        {
            if (_player == null)
                return;
            
            _moveDown = false;
            _moveUp = false;
            _switch = false;
            _deploy = false;
            _shoot = false;

            var modifiers = GetModifiers();
            if(!modifiers.Untouchable)
                HandleProjectileThreats();

            HandlePhaseShiftDeploy(modifiers);
            HandlePlayerChasing();
            HandleShooting();
            
            _deployWasReleased = !_deploy;
        }

        private void HandleShooting()
        {
            _shootRefillWait += Raylib.GetFrameTime();

            var resourceManager = _playerManagerFactory.GetResourceManager(_player!);
            if (resourceManager.ShootMeter < .5f)
            {
                _shootRefillWait = 0f;
                return;
            }

            if (_shootRefillWait > 2f)
            {
                _shoot = true;
            }
        }

        private void HandleProjectileThreats()
        {
            var bulletThreat = _opponentBulletWatcher.GetThreat(_player);
            if (bulletThreat.Above > .5 && bulletThreat.Below > .5)
            {
                var resourceManager = _playerManagerFactory.GetResourceManager(_player.Id);
                if (resourceManager.ShieldMeter > 10)
                    _switch = true;
            }
            else if (!_switch && bulletThreat.Above > .5)
            {
                _moveDown = true;
            }
            else if (!_switch && bulletThreat.Below > .5)
            {
                _moveUp = true;
            }
        }

        private void HandlePlayerChasing()
        {
            if (_moveUp || _moveDown)
                return;

            var opponent = GetOpponent();
            if (_player.Center.Y < opponent.Center.Y - 100)
                _moveDown = true;
            else if (_player.Center.Y > opponent.Center.Y + 100)
                _moveUp = true;
        }

        private Player GetOpponent()
        {
            return _objectService.GetGameObjects<Player>().Single(x => x.Id != _player.Id);
        }

        private EffectModifiers GetModifiers()
        {
            var effectManager = _playerManagerFactory.GetEffectManager(_player.Id);
            return effectManager.GetModifiers();
        }

        private void HandlePhaseShiftDeploy(EffectModifiers modifiers)
        {
            var effectManager = _playerManagerFactory.GetEffectManager(_player);
            var phaseShifter = _objectService.GetGameObjects<PhaseShifter>()
                .SingleOrDefault(x => x.Owner == _player.Id);

            bool hasEffect = effectManager.HasEffect<PhaseShiftedEffect>();
            if (phaseShifter != null)
            {
                if(hasEffect)
                {
                    //
                }
                else 
                { 
                    if (_player.Center.Y > phaseShifter.Center.Y)
                        _moveUp = true;
                    else
                        _moveDown = true;
                }
                return;
            }

            if (phaseShifter == null && _deployWasReleased)
            {
                _deploy = true;
                _deployWasReleased = false;
                return;
            }
        }
    }
}
