using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Effects;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Players
{
    public class Player : GameObject
    {
        public float Health { get; set; } = 100f;

        public PlayerStats BaseStats { get; private set; } = new PlayerStats();
        public Color? PalleteSwap { get; set; }
        public bool IsPlayer1 { get; private set; }

        public Func<PlayerEffect>? OffensiveAugment { get; set; }
        public Func<PlayerEffect>? DefensiveAugment { get; set; }

        private readonly IPlayerController _playerController;

        public Player(IPlayerController playerUpdater, Rectangle initialPosition, bool isPlayer1)
            : base(Game.Id, 
                  new SpriteWrapper("Sprites/Ships/MainShip.png"), 
                  initialPosition.Position, 
                  initialPosition.Size, 
                  new Vector2(0,20f))
        {
            _playerController = playerUpdater;
            IsPlayer1 = isPlayer1;
            
            // Set the hitbox for this player to use triangle collision with percentage coordinates
            Hitbox = new HitboxTriangle([
                new Vector2(0.045f, 0.685f),  // Left wing: 4.5% in, 68.5% down
                new Vector2(0.955f, 0.685f),  // Right wing: 95.5% in, 68.5% down
                new Vector2(0.5f, 0.08f)      // Ship tip: 50% in, 8% down
            ]);
        }

        public void Initialize(float health, PlayerStats stats, Color palleteSwap)
        {
            Health = health;
            BaseStats = stats;
            PalleteSwap = palleteSwap;
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
