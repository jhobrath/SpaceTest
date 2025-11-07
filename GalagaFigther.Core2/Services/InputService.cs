using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Players;
using Raylib_cs;
using System;
using System.Collections.Generic;

namespace GalagaFighter.Core2.Services
{
    public interface IInputService : IClearable
    {
        void AddPlayer(Guid owner, IInputMappings mappings);
        void Update(float frameTime);
    }

    public class InputService : IInputService
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;

        public InputService(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        private readonly Dictionary<Guid, IInputMappings> _players = new();
        private float _gameTime = 0f;

        public void AddPlayer(Guid owner, IInputMappings mappings)
        {
            _players[owner] = mappings;
        }

        public void Clear()
        {
            _players.Clear();
        }

        public void Update(float frameTime)
        {
            _gameTime += frameTime;

            foreach (var playerMapping in _players)
            {
                var player = _objectService.Get<Player>(playerMapping.Key);
                var inputData = _gameDataRegistry.Get<PlayerInputData>(player);
                var mappings = playerMapping.Value;
                
                inputData.Up.Update(mappings.IsUpDown(), frameTime, _gameTime);
                inputData.Down.Update(mappings.IsDownDown(), frameTime, _gameTime);
                inputData.Left.Update(mappings.IsLeftDown(), frameTime, _gameTime);
                inputData.Right.Update(mappings.IsRightDown(), frameTime, _gameTime);
                inputData.Shoot.Update(mappings.IsShootDown(), frameTime, _gameTime);
                inputData.Defend.Update(mappings.IsDefendDown(), frameTime, _gameTime);
                inputData.DeployTurret.Update(mappings.IsDeployTurretDown(), frameTime, _gameTime);
            }
        }
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
    }

    public interface IInputMappings
    {
        bool IsUpDown();
        bool IsDownDown();
        bool IsLeftDown();
        bool IsRightDown();
        bool IsShootDown();
        bool IsDefendDown();
        bool IsDeployTurretDown();
    }

    public class KeyMappings : IInputMappings
    {
        public KeyboardKey Up { get; set; } = KeyboardKey.W;
        public KeyboardKey Down { get; set; } = KeyboardKey.S;
        public KeyboardKey Left { get; set; } = KeyboardKey.A;
        public KeyboardKey Right { get; set; } = KeyboardKey.D;
        public KeyboardKey Shoot { get; set; } = KeyboardKey.K;
        public KeyboardKey Defend { get; set; } = KeyboardKey.J;
        public KeyboardKey DeployTurret { get; set; } = KeyboardKey.U;

        public KeyMappings(KeyboardKey up, KeyboardKey down, KeyboardKey left, KeyboardKey right, 
            KeyboardKey shoot, KeyboardKey defend, KeyboardKey deployTurret)
        {
            Up = up;
            Down = down;
            Left = left;
            Right = right;
            Shoot = shoot;
            Defend = defend;
            DeployTurret = deployTurret;
        }
            
        public bool IsUpDown() => Raylib.IsKeyDown(Up);
        public bool IsDownDown() => Raylib.IsKeyDown(Down);
        public bool IsLeftDown() => Raylib.IsKeyDown(Left);
        public bool IsRightDown() => Raylib.IsKeyDown(Right);
        public bool IsShootDown() => Raylib.IsKeyDown(Shoot);
        public bool IsDefendDown() => Raylib.IsKeyDown(Defend);
        public bool IsDeployTurretDown() => Raylib.IsKeyDown(DeployTurret);
    }

    public class GamepadMappings : IInputMappings
    {
        public GamepadButton Up { get; set; } = GamepadButton.LeftFaceUp;
        public GamepadButton Down { get; set; } = GamepadButton.LeftFaceDown;
        public GamepadButton Left { get; set; } = GamepadButton.LeftFaceLeft;
        public GamepadButton Right { get; set; } = GamepadButton.LeftFaceRight;
        public GamepadButton Shoot { get; set; } = GamepadButton.RightTrigger1;
        public GamepadButton Defend { get; set; } = GamepadButton.LeftTrigger1;
        public GamepadButton DeployTurret { get; set; } = GamepadButton.RightFaceUp;

        public GamepadMappings(GamepadButton up, GamepadButton down, GamepadButton left, GamepadButton right,
            GamepadButton shoot, GamepadButton defend, GamepadButton deployTurret)
        {
            Up = up;
            Down = down;
            Left = left;
            Right = right;
            Shoot = shoot;
            Defend = defend;
            DeployTurret = deployTurret;
        }

        public bool IsUpDown() => Raylib.IsGamepadButtonDown(0, Up);
        public bool IsDownDown() => Raylib.IsGamepadButtonDown(0, Down);
        public bool IsLeftDown() => Raylib.IsGamepadButtonDown(0, Left);
        public bool IsRightDown() => Raylib.IsGamepadButtonDown(0, Right);
        public bool IsShootDown() => Raylib.IsGamepadButtonDown(0, Shoot);
        public bool IsDefendDown() => Raylib.IsGamepadButtonDown(0, Defend);
        public bool IsDeployTurretDown() => Raylib.IsGamepadButtonDown(0, DeployTurret);
    }

    public class ButtonData
    {
        public static implicit operator bool(ButtonData state)
        {
            return state.IsDown;
        }

        public float HeldDuration { get; set; } = 0f;
        public bool WasReleased { get; set; } = false;
        public bool IsPressed { get; set; } = false;
        public bool IsDown { get; set; } = false;

        public void Update(bool isDown, float frameTime, float currentTime)
        {
            bool wasDown = HeldDuration > 0f;

            if (isDown)
            {
                if (!wasDown) // Just pressed
                {
                    IsPressed = true;
                }
                else
                {
                    IsPressed = false;
                }

                HeldDuration += frameTime;
                IsDown = true;
                WasReleased = false;
            }
            else
            {
                if (wasDown) // Just released
                {
                    WasReleased = true;
                }
                else
                {
                    WasReleased = false;
                }

                IsPressed = false;
                IsDown = false;
                HeldDuration = 0f;
            }
        }
    }
}
