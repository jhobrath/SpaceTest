using Raylib_cs;
using System;
using System.Collections.Generic;

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
        public bool IsDoublePressed { get; set; } = false;

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
                WasReleased = false,
                IsDoublePressed = false
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

    // Consolidated button tracking data
    internal class ButtonData
    {
        public float HeldDuration { get; set; } = 0f;
        public bool WasReleased { get; set; } = false;
        public bool IsPressed { get; set; } = false;
        public bool IsDoublePressed { get; set; } = false;
        public float LastPressTime { get; set; } = -1f;
        public int PressCount { get; set; } = 0;
        
        private const float DOUBLE_PRESS_WINDOW = 0.3f; // 300ms window for double press
        
        public void Update(bool isDown, float frameTime, float currentTime)
        {
            bool wasDown = HeldDuration > 0f;
            
            if (isDown)
            {
                if (!wasDown) // Just pressed
                {
                    IsPressed = true;
                    
                    // Check for double press
                    if (currentTime - LastPressTime <= DOUBLE_PRESS_WINDOW)
                    {
                        PressCount++;
                        if (PressCount >= 2)
                        {
                            IsDoublePressed = true;
                            PressCount = 0; // Reset after registering double press
                        }
                    }
                    else
                    {
                        PressCount = 1; // Start new press sequence
                    }
                    
                    LastPressTime = currentTime;
                }
                else
                {
                    IsPressed = false;
                    IsDoublePressed = false;
                }
                
                HeldDuration += frameTime;
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
                IsDoublePressed = false;
                HeldDuration = 0f;
                
                // Reset double press detection after window expires
                if (currentTime - LastPressTime > DOUBLE_PRESS_WINDOW)
                {
                    PressCount = 0;
                }
            }
        }
        
        public ButtonState ToButtonState()
        {
            return new ButtonState
            {
                IsPressed = IsPressed,
                IsDown = HeldDuration > 0f,
                HeldDuration = HeldDuration,
                WasReleased = WasReleased,
                IsDoublePressed = IsDoublePressed
            };
        }
    }

    // Player input data consolidated into single structure
    internal class PlayerInputData
    {
        public KeyMappings Mappings { get; set; }
        public ButtonData MoveLeft { get; set; } = new();
        public ButtonData MoveRight { get; set; } = new();
        public ButtonData Shoot { get; set; } = new();
        public ButtonData Switch { get; set; } = new();
    }

    public class InputService : IInputService
    {
        private readonly Dictionary<Guid, PlayerInputData> _players = [];
        private float _gameTime = 0f;

        public void AddPlayer(Guid owner, KeyMappings mappings)
        {
            _players[owner] = new PlayerInputData { Mappings = mappings };
        }

        public void Update()
        {
            var frameTime = Raylib.GetFrameTime();
            _gameTime += frameTime;

            foreach (var player in _players.Values)
            {
                player.MoveLeft.Update(Raylib.IsKeyDown(player.Mappings.MoveLeft), frameTime, _gameTime);
                player.MoveRight.Update(Raylib.IsKeyDown(player.Mappings.MoveRight), frameTime, _gameTime);
                player.Shoot.Update(Raylib.IsKeyDown(player.Mappings.Shoot), frameTime, _gameTime);
                player.Switch.Update(Raylib.IsKeyDown(player.Mappings.Switch), frameTime, _gameTime);
            }
        }

        public ButtonState GetMoveLeft(Guid owner) => _players[owner].MoveLeft.ToButtonState();
        public ButtonState GetMoveRight(Guid owner) => _players[owner].MoveRight.ToButtonState();
        public ButtonState GetShoot(Guid owner) => _players[owner].Shoot.ToButtonState();
        public ButtonState GetSwitch(Guid owner) => _players[owner].Switch.ToButtonState();
    }
}
