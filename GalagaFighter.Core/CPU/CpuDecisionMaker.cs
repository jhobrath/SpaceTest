using GalagaFighter.Core.Handlers.Players;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.CPU
{
    public interface ICpuDecisionMaker
    {
        bool IsMoveLeftDown();
        bool IsMoveRightDown();
        bool IsShootDown();
        bool IsSwitchDown();

        void Update();
    }
    public class CpuDecisionMaker : ICpuDecisionMaker
    {
        private readonly IObjectService _objectService;
        private readonly IPlayerManagerFactory _managerFactory;
        private IPlayerEffectManager _effectManager;
        private IPlayerResourceManager _resourceManager;

        private IPlayerEffectManager _opponentEffectManager;
        private IPlayerResourceManager _opponentResourceManager;

        private readonly Guid _playerId;

        public bool _moveLeft;
        public bool _moveRight;
        public bool _shoot;
        public bool _switch;

        public bool IsMoveLeftDown() => _moveLeft;
        public bool IsMoveRightDown() => _moveRight;
        public bool IsShootDown() => _shoot;
        public bool IsSwitchDown() => _switch;

        private readonly int _maxFrameCount = 10;
        private readonly int _difficulty = 1;
        private int _frameCount = 0;
        private Player _opponent;
        private Player _player;

        public CpuDecisionMaker(IObjectService objectService, IPlayerManagerFactory managerFactory, Guid id)
        {
            _objectService = objectService;
            _managerFactory = managerFactory;
            _playerId = id;

            InitializeManagers();
        }

        private void InitializeManagers()
        {
            var players = _objectService.GetGameObjects<Player>();
            _opponent = players.Last(x => x.Id != _playerId);
            _player = players.Single(x => x.Id == _playerId);

            _effectManager = _managerFactory.GetEffectManager(_playerId);
            _resourceManager = _managerFactory.GetResourceManager(_playerId);

            _opponentResourceManager = _managerFactory.GetResourceManager(_opponent.Id);
            _opponentEffectManager = _managerFactory.GetEffectManager(_opponent.Id);
        }


        public void Update()
        {
            _frameCount++;
            if ((_maxFrameCount - _difficulty) > _frameCount)
                return;

            _frameCount = 0;
            MakeDecision();
        }

        private void MakeDecision()
        {
            var modifiers = _effectManager.GetModifiers();
            var opponentProjectiles = _objectService.GetChildren<Projectile>(_opponent.Id)
                .Select(x => new {
                    Damage = x.BaseDamage * x.Modifiers.DamageMultiplier,
                    Position = x.Center,
                    Size = x.Rect.Size,
                    Speed = x.Speed
                }).ToList();

            var opponentPosition = _opponent.Center;
            var playerPosition = _player.Center;

            // Always shoot for now
            _shoot = true;

            // Default: don't switch
            _switch = false;

            // Default movement
            _moveLeft = false;
            _moveRight = false;

            // 1. Check for dangerous projectiles using Vector2.Distance
            bool dangerDetected = false;
            float dangerThreshold = 200f;
            foreach (var proj in opponentProjectiles)
            {
                float distance = Vector2.Distance(proj.Position, playerPosition);
                if (distance < dangerThreshold)
                {
                    dangerDetected = true;
                    // Move up if projectile is below, down if above
                    if (proj.Position.Y > playerPosition.Y)
                    {
                        // Move up (from ship's perspective, MoveRight)
                        _moveLeft = false;
                        _moveRight = true;
                    }
                    else
                    {
                        // Move down (from ship's perspective, MoveLeft)
                        _moveLeft = true;
                        _moveRight = false;
                    }
                    break;
                }
            }

            // 2. If danger cannot be avoided, use switch (defend) if resources are sufficient
            if (dangerDetected && _resourceManager.ShieldMeter > 40)
            {
                _switch = true;
            }

            // 3. PowerUp targeting logic with difficulty check
            var powerUps = _objectService.GetGameObjects<GalagaFighter.Core.Models.PowerUps.PowerUp>()
                .Where(p => p.IsActive).ToList();
            bool prioritizePowerUp = powerUps.Count > 0;
            // Only go for power-up if random chance meets difficulty
            if (prioritizePowerUp && Game.Random.Next(_maxFrameCount) < _difficulty)
            {
                // Find closest power-up
                var targetPowerUp = powerUps.OrderBy(p => Math.Abs(p.Center.Y - playerPosition.Y)).First();
                float alignmentThreshold = 150f;
                if (playerPosition.Y < targetPowerUp.Center.Y - alignmentThreshold)
                {
                    _moveLeft = true;
                    _moveRight = false;
                }
                else if (playerPosition.Y > targetPowerUp.Center.Y + alignmentThreshold)
                {
                    _moveLeft = false;
                    _moveRight = true;
                }
                else
                {
                    _moveLeft = false;
                    _moveRight = false;
                }
                // Always shoot at power-up
                _shoot = true;
                return;
            }

            // 4. If no power-up or not targeting power-up, try to align with opponent to attack
            if (!dangerDetected)
            {
                float alignmentThreshold = 150f;
                if (playerPosition.Y < opponentPosition.Y - alignmentThreshold)
                {
                    // CPU is above opponent, move down
                    _moveLeft = true;
                    _moveRight = false;
                }
                else if (playerPosition.Y > opponentPosition.Y + alignmentThreshold)
                {
                    // CPU is below opponent, move up
                    _moveLeft = false;
                    _moveRight = true;
                }
                else
                {
                    // Aligned vertically, stop moving
                    _moveLeft = false;
                    _moveRight = false;
                }
            }
        }
    }
}
