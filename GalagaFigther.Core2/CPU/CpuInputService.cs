using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.CPU
{
    public interface ICpuInputService
    {
        void SetPlayer(Player player);
        void Update(float frameTime);
    }

    public class CpuInputService : IInputMappings, ICpuInputService
    {
        private bool _up = false;
        private bool _shoot = true;
        private bool _shield = false;
        private bool _right = false;
        private bool _left = false;
        private bool _down = false;
        private bool _deploy = false;
        private bool _defend = false;
        private Player _player;
        private Player _opponent;


        public bool IsDefendDown() => _defend;
        public bool IsDeployTurretDown() => _deploy;
        public bool IsDownDown() => _down;
        public bool IsLeftDown() => _left;
        public bool IsRightDown() => _right;
        public bool IsShieldDown() => _shield;
        public bool IsShootDown() => _shoot;
        public bool IsUpDown() => _up;

        private readonly IObjectService _objectService;
        private readonly IGameDataRegistry _gameDataRegistry;

        public CpuInputService(IObjectService objectService, IGameDataRegistry gameDataRegistry)
        {
            _objectService = objectService;
            _gameDataRegistry = gameDataRegistry;
        }

        
        public void SetPlayer(Player player)
        {
            _player = player;
            _opponent = _objectService.GetOpponent(_player);
        }

        public void Update(float frameTime)
        {

        }
    }
}
