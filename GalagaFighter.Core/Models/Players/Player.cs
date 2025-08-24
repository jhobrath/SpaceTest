using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Players
{
    public class Player : GameObject
    {
        public float Health { get; set; } = 100f;
        public bool IsPlayer1 { get; private set; }

        private readonly IPlayerController _playerController;

        public Player(IPlayerController playerUpdater, Rectangle initialPosition, bool isPlayer1)
            : base(Game.Id, new SpriteWrapper("Sprites/Ships/MainShip.png"), initialPosition.Position, initialPosition.Size, new Vector2(0,20f))
        {
            _playerController = playerUpdater;
            IsPlayer1 = isPlayer1;
        }

        public override void Update(Game game)
        {
            _playerController.Update(game, this);
        }

        public override void Draw()
        {
            _playerController.Draw(this);
        }
    }
}
