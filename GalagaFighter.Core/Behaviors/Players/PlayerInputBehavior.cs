using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Behaviors.Players.Updates;
using GalagaFighter.Core.Models;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Behaviors.Players
{
    public class KeyMappings
    {
        public KeyboardKey MoveLeft { get; set; } = KeyboardKey.W;
        public KeyboardKey MoveRight { get; set; } = KeyboardKey.S;
        public KeyboardKey Shoot { get; set; } = KeyboardKey.D;

        public KeyMappings(KeyboardKey left, KeyboardKey right, KeyboardKey shoot)
        {
            MoveLeft = left;
            MoveRight = right;
            Shoot = shoot;
        }
    }

    public class PlayerInputBehavior : IPlayerInputBehavior
    {
        private KeyMappings _mappings;

        private float _movingLeftDuration = 0f;
        private float _movingRightDuration = 0f;
        private float _shootingDuration = 0f;

        public PlayerInputBehavior(KeyMappings mappings)
        {
            _mappings = mappings;   
        }

        public PlayerInputUpdate Apply(PlayerInputUpdate update)
        {
            update.Left = IsMovingLeft();
            update.Right = IsMovingRight();
            update.Shoot = IsShooting();

            return update;
        }

        protected virtual ButtonState IsMovingLeft()
        {
            var isMovingLeft = Raylib.IsKeyDown(_mappings.MoveLeft);

            if (!isMovingLeft)
                _movingLeftDuration = 0f;
            else
                _movingLeftDuration += Raylib.GetFrameTime();

            return new ButtonState
            {
                IsPressed = isMovingLeft,
                HeldDuration = _movingLeftDuration
            };
        }

        protected virtual ButtonState IsMovingRight()
        {
            var isMovingRight = Raylib.IsKeyDown(_mappings.MoveRight);

            if (!isMovingRight)
                _movingRightDuration = 0f;
            else
                _movingRightDuration += Raylib.GetFrameTime();

            return new ButtonState
            {
                IsPressed = isMovingRight,
                HeldDuration = _movingRightDuration
            };
        }

        protected virtual bool IsShooting()
        {
            var isShooting = Raylib.IsKeyDown(_mappings.Shoot);

            if (!isShooting)
                _shootingDuration = 0f;
            else
                _shootingDuration += Raylib.GetFrameTime(); 

            return new ButtonState
                {
                    IsPressed = isShooting,
                    HeldDuration = _shootingDuration
                };
        }
    }
}
