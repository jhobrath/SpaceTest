using GalagaFighter.Core.Behaviors.Players;
using GalagaFighter.Core.Behaviors.Players.Updates;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IInputService
    {
        void Update();

        void AddPlayer(Guid owner, KeyMappings mappings);

        ButtonState GetShoot(Guid owner);
        ButtonState GetMoveLeft(Guid owner);
        ButtonState GetMoveRight(Guid owner);
        ButtonState GetSwitch(Guid owner);
    }

    public class InputService : IInputService
    {
        private Dictionary<Guid, KeyMappings> _mappings = [];
        private Dictionary<Guid, float> _movingLeftDuration = [];
        private Dictionary<Guid, float> _movingRightDuration = [];
        private Dictionary<Guid, float> _shootingDuration = [];
        private Dictionary<Guid, float> _switchingDuration = [];

        public InputService()
        {
        }

        public void AddPlayer(Guid owner, KeyMappings mappings)
        {
            _mappings.Add(owner, mappings);
            _movingLeftDuration.Add(owner, 0f);
            _movingRightDuration.Add(owner, 0f);
            _shootingDuration.Add(owner, 0f);
            _switchingDuration.Add(owner, 0f);
        }

        public ButtonState GetMoveRight(Guid owner) => GetButton(_mappings[owner].MoveRight, _movingRightDuration[owner]);
        public ButtonState GetMoveLeft(Guid owner) => GetButton(_mappings[owner].MoveLeft, _movingLeftDuration[owner]);
        public ButtonState GetShoot(Guid owner) => GetButton(_mappings[owner].Shoot, _shootingDuration[owner]);
        public ButtonState GetSwitch(Guid owner) => GetButton(_mappings[owner].Switch, _switchingDuration[owner]);


        public void Update()
        {
            foreach(var mapping in _mappings)
            {
                UpdateMovingLeft(mapping.Key);
                UpdateMovingRight(mapping.Key);
                UpdateIsShooting(mapping.Key);
                UpdateIsSwitching(mapping.Key);
            }
        }

        private ButtonState GetButton(KeyboardKey key, float duration)
        {
            var frameTime = Raylib.GetFrameTime();
            var state = new ButtonState
            {
                HeldDuration = duration,
                IsDown = Raylib.IsKeyDown(key),
                IsPressed = Math.Abs(duration - frameTime) < .001
            };

            if(state.IsDown && state.IsPressed)
            {
                var s = "";
            }

            return state;
        }

        public void UpdateMovingLeft(Guid owner)
        {
            if (!_mappings.TryGetValue(owner, out KeyMappings? mappings))
                return;

            var isMovingLeft = Raylib.IsKeyDown(mappings.MoveLeft);

            if (!isMovingLeft)
                _movingLeftDuration[owner] = 0f;
            else
                _movingLeftDuration[owner] += Raylib.GetFrameTime();
        }

        public void UpdateMovingRight(Guid owner)
        {
            if (!_mappings.TryGetValue(owner, out KeyMappings? mappings))
                return;

            var _isMovingRight = Raylib.IsKeyDown(mappings.MoveRight);

            if (!_isMovingRight)
                _movingRightDuration[owner] = 0f;
            else
                _movingRightDuration[owner] += Raylib.GetFrameTime();
        }

        public void UpdateIsShooting(Guid owner)
        {
            if (!_mappings.TryGetValue(owner, out KeyMappings? mappings))
                return;

            var isShooting = Raylib.IsKeyDown(mappings.Shoot);

            if (!isShooting)
                _shootingDuration[owner] = 0f;
            else
                _shootingDuration[owner] += Raylib.GetFrameTime();
        }

        public void UpdateIsSwitching(Guid owner)
        {
            if (!_mappings.TryGetValue(owner, out KeyMappings? mappings))
                return;

            var isSwitching = Raylib.IsKeyDown(mappings.Switch);

            if (!isSwitching)
                _switchingDuration[owner] = 0f;
            else
                _switchingDuration[owner] += Raylib.GetFrameTime();
        }
    }
}
