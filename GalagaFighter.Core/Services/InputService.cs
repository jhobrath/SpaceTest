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

    public class ButtonState
    {
        public bool IsPressed { get; set; } = false;
        public bool IsDown { get; set; } = false;
        public float HeldDuration { get; set; } = 0f;
        public bool WasReleased { get; set; } = false;

        public static implicit operator bool(ButtonState state)
        {
            return state.IsDown;
        }

        public static implicit operator ButtonState(bool val)
        {
            return new ButtonState
            {
                IsPressed = false,
                IsDown = val,
                HeldDuration = 0f,
                WasReleased = false
            };
        }
    }

    public class KeyMappings
    {
        public KeyboardKey MoveLeft { get; set; } = KeyboardKey.W;
        public KeyboardKey MoveRight { get; set; } = KeyboardKey.S;
        public KeyboardKey Shoot { get; set; } = KeyboardKey.D;
        public KeyboardKey Switch { get; set; } = KeyboardKey.A;

        public KeyMappings(KeyboardKey left, KeyboardKey right, KeyboardKey shoot, KeyboardKey switchButton)
        {
            MoveLeft = left;
            MoveRight = right;
            Shoot = shoot;
            Switch = switchButton;
        }
    }


    public class InputService : IInputService
    {
        private Dictionary<Guid, KeyMappings> _mappings = [];
        private Dictionary<Guid, float> _movingLeftDuration = [];
        private Dictionary<Guid, float> _movingRightDuration = [];
        private Dictionary<Guid, float> _shootingDuration = [];
        private Dictionary<Guid, float> _switchingDuration = [];

        private Dictionary<Guid, bool> _rightWasReleased = [];
        private Dictionary<Guid, bool> _leftWasReleased = [];
        private Dictionary<Guid, bool> _shootWasReleased = [];
        private Dictionary<Guid, bool> _switchWasReleased = [];
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
            _rightWasReleased.Add(owner, false);
            _leftWasReleased.Add(owner, false);
            _shootWasReleased.Add(owner, false);
            _switchWasReleased.Add(owner, false);
        }

        public ButtonState GetMoveRight(Guid owner) => GetButton(_mappings[owner].MoveRight, _movingRightDuration[owner], _rightWasReleased[owner]);
        public ButtonState GetMoveLeft(Guid owner) => GetButton(_mappings[owner].MoveLeft, _movingLeftDuration[owner], _leftWasReleased[owner]);
        public ButtonState GetShoot(Guid owner) => GetButton(_mappings[owner].Shoot, _shootingDuration[owner], _shootWasReleased[owner]);
        public ButtonState GetSwitch(Guid owner) => GetButton(_mappings[owner].Switch, _switchingDuration[owner], _switchWasReleased[owner]);

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

        private ButtonState GetButton(KeyboardKey key, float duration, bool wasReleased)
        {
            var frameTime = Raylib.GetFrameTime();
            var state = new ButtonState
            {
                HeldDuration = duration,
                IsDown = Raylib.IsKeyDown(key),
                IsPressed = Math.Abs(duration - frameTime) < .001,
                WasReleased = wasReleased
            };

            return state;
        }

        public void UpdateMovingLeft(Guid owner)
        {
            if (!_mappings.TryGetValue(owner, out KeyMappings? mappings))
                return;

            var isMovingLeft = Raylib.IsKeyDown(mappings.MoveLeft);


            if (!isMovingLeft)
            {
                _leftWasReleased[owner] = _movingLeftDuration[owner] != 0f;
                _movingLeftDuration[owner] = 0f;
            }
            else
                _movingLeftDuration[owner] += Raylib.GetFrameTime();
        }

        public void UpdateMovingRight(Guid owner)
        {
            if (!_mappings.TryGetValue(owner, out KeyMappings? mappings))
                return;

            var _isMovingRight = Raylib.IsKeyDown(mappings.MoveRight);

            if (!_isMovingRight)
            {
                _rightWasReleased[owner] = _movingRightDuration[owner] != 0f;
                _movingRightDuration[owner] = 0f;
            }
            else
                _movingRightDuration[owner] += Raylib.GetFrameTime();
        }

        public void UpdateIsShooting(Guid owner)
        {
            if (!_mappings.TryGetValue(owner, out KeyMappings? mappings))
                return;

            var isShooting = Raylib.IsKeyDown(mappings.Shoot);

            if (!isShooting)
            {
                _shootWasReleased[owner] = _shootingDuration[owner] != 0f;
                _shootingDuration[owner] = 0f;
            }
            else
                _shootingDuration[owner] += Raylib.GetFrameTime();
        }

        public void UpdateIsSwitching(Guid owner)
        {
            if (!_mappings.TryGetValue(owner, out KeyMappings? mappings))
                return;

            var isSwitching = Raylib.IsKeyDown(mappings.Switch);

            if (!isSwitching)
            {
                _switchWasReleased[owner] = _switchingDuration[owner] != 0f;
                _switchingDuration[owner] = 0f;
            }
            else
                _switchingDuration[owner] += Raylib.GetFrameTime();
        }
    }
}
