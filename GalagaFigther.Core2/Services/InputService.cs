using Raylib_cs;
using System;

namespace GalagaFighter.Core2.Services
{
    public interface IInputService
    {
        void Update(float frameTime);
        
        // Player 1 Movement
        ButtonState Forward { get; }
        ButtonState Back { get; }
        ButtonState Left { get; }
        ButtonState Right { get; }
        
        // Player 1 Actions
        ButtonState Shoot { get; }
        ButtonState Defend { get; }
    }

    public class InputService : IInputService
    {
        // Player 1 key mappings (WASD + K/J)
        private const KeyboardKey ForwardKey = KeyboardKey.W;
        private const KeyboardKey BackKey = KeyboardKey.S;
        private const KeyboardKey LeftKey = KeyboardKey.A;
        private const KeyboardKey RightKey = KeyboardKey.D;
        private const KeyboardKey ShootKey = KeyboardKey.K;
        private const KeyboardKey DefendKey = KeyboardKey.J;

        // Button state tracking
        private readonly ButtonData _forward = new();
        private readonly ButtonData _back = new();
        private readonly ButtonData _left = new();
        private readonly ButtonData _right = new();
        private readonly ButtonData _shoot = new();
        private readonly ButtonData _defend = new();

        private float _gameTime = 0f;

        // Public properties for accessing button states
        public ButtonState Forward => _forward.ToButtonState();
        public ButtonState Back => _back.ToButtonState();
        public ButtonState Left => _left.ToButtonState();
        public ButtonState Right => _right.ToButtonState();
        public ButtonState Shoot => _shoot.ToButtonState();
        public ButtonState Defend => _defend.ToButtonState();

        public void Update(float frameTime)
        {
            _gameTime += frameTime;

            // Update all button states
            _forward.Update(Raylib.IsKeyDown(ForwardKey), frameTime, _gameTime);
            _back.Update(Raylib.IsKeyDown(BackKey), frameTime, _gameTime);
            _left.Update(Raylib.IsKeyDown(LeftKey), frameTime, _gameTime);
            _right.Update(Raylib.IsKeyDown(RightKey), frameTime, _gameTime);
            _shoot.Update(Raylib.IsKeyDown(ShootKey), frameTime, _gameTime);
            _defend.Update(Raylib.IsKeyDown(DefendKey), frameTime, _gameTime);
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

    internal class ButtonData
    {
        public float HeldDuration { get; set; } = 0f;
        public bool WasReleased { get; set; } = false;
        public bool IsPressed { get; set; } = false;

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
                HeldDuration = 0f;
            }
        }

        public ButtonState ToButtonState()
        {
            return new ButtonState
            {
                IsPressed = IsPressed,
                IsDown = HeldDuration > 0f,
                HeldDuration = HeldDuration,
                WasReleased = WasReleased
            };
        }
    }
}
