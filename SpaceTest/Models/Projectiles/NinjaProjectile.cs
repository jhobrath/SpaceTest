using GalagaFighter;
using GalagaFighter.Models;
using GalagaFighter.Models.Players;
using Raylib_cs;
using System.Numerics;
using GalagaFighter.Models.PowerUps;
using GalagaFighter.Models.Effects;

namespace SpaceTest.Models.Projectiles
{
    public class NinjaProjectile : Projectile
    {
        private readonly SpriteWrapper spriteWrapper;

        public NinjaProjectile(Rectangle rect, Vector2 speed, Player owner) 
            : base(rect, speed, owner) 
        {
            // Use ninja.png as an animated sprite (3 frames, 0.12s per frame)
            var texture = Raylib.LoadTexture("Sprites/Projectiles/ninja.png");
            spriteWrapper = new SpriteWrapper(texture, 3, 0.12f);
        }

        public override int Damage => 20; // Ninja do double damage

        public override void OnHit(Player target, Game game)
        {
            game.PlayHitSound();
        }

        public override Color GetColor()
        {
            return Color.White;
        }

        public override void Update(Game game)
        {
            base.Update(game);
            spriteWrapper.Update(Raylib.GetFrameTime());
        }

        public override void Draw()
        {
            // Draw animated ninja sprite
            spriteWrapper.Draw(
                new Vector2(Rect.X + Rect.Width / 2f, Rect.Y + Rect.Height / 2f),
                0f,
                Rect.Width,
                Rect.Height
            );
        }
    }
}