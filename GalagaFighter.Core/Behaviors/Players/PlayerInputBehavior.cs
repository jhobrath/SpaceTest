using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Behaviors.Players.Updates;
using GalagaFighter.Core.Models;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
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
        private readonly IInputService _inputService;

        public PlayerInputBehavior(IInputService inputService)
        {
            _inputService = inputService;
        }

        public PlayerInputUpdate Apply(Player player)
        {
            var update = new PlayerInputUpdate();

            update.Left = IsMovingLeft(player.Id);
            update.Right = IsMovingRight(player.Id);
            update.Shoot = IsShooting(player.Id);

            return update;
        }

        protected virtual ButtonState IsMovingLeft(Guid owner) => _inputService.GetMoveLeft(owner);
        protected virtual ButtonState IsMovingRight(Guid owner) => _inputService.GetMoveRight(owner);
        protected virtual bool IsShooting(Guid owner) => _inputService.GetShoot(owner);
    }
}
