using GalagaFighter;
using GalagaFighter.Models;
using GalagaFighter.Models.Players;
using Raylib_cs;
using System.Numerics;
using GalagaFighter.Models.PowerUps;
using GalagaFighter.Models.Effects;

namespace SpaceTest.Models.Projectiles
{
    public class IceProjectile : Projectile
    {
        public IceProjectile(Rectangle rect, Vector2 speed, Player owner) 
            : base(rect, speed, owner) 
        {
            sprite = SpriteGenerator.CreateProjectileSprite(ProjectileType.Ice, (int)rect.Width, (int)rect.Height);
        }

        public override int Damage => 0; // Ice projectiles don't do damage

        public override void OnHit(Player target, Game game)
        {
            // Add a FrozenEffect to the target
            target.Stats.AddEffect(target, new FrozenEffect(target));

            game.PlayIceHitSound();
        }

        public override Color GetColor()
        {
            return Color.SkyBlue;
        }

        public override void Draw()
        {
            // Use the generated sprite (which works!)
            if (sprite.Id > 0)
            {
                Raylib.DrawTexture(sprite, (int)Rect.X, (int)Rect.Y, Color.White);
            }
            else
            {
                // Fallback to simple rectangle if sprite fails
                Raylib.DrawRectangleRec(Rect, Color.SkyBlue);
            }
        }
    }
}