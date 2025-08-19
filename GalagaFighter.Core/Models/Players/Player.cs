using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Players
{
    public class Player : GameObject
    {
        public float Health { get; set; } = 100f;
        public PlayerStats Stats { get; set; } = new();
        public PlayerDisplay Display { get; set; }
        public bool IsPlayer1 { get; private set; }

        public Player(Guid owner, PlayerDisplay display, bool isPlayer1)
            : base(owner, display.Sprite, display.Rect.Position, display.Rect.Size, new Vector2(0,0))
        {
            Display = display;
            IsPlayer1 = isPlayer1;
        }

        public void Collide(Projectile projectile)
        {
            // Collision logic will be handled by controller/system
        }

        public override void Update(Game game)
        {
            // Update logic will be handled by controller/system
        }

        public override void Draw()
        {
            Display.Sprite.Draw(Center, Display.Rotation, Rect.Width * Display.Size, Rect.Height * Display.Size, Display.Color);
        }
    }
}
