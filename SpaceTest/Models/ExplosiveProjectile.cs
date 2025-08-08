using Raylib_cs;
using GalagaFighter.Models.Players;
using System.Numerics;
using System;

namespace GalagaFighter.Models
{
    public class ExplosiveProjectile : Projectile
    {
        private const int ExplosionRadius = 50;

        public ExplosiveProjectile(Rectangle rect, Vector2 speed, Player owner)
            : base(rect, speed, owner)
        {
            sprite = SpriteGenerator.CreateProjectileSprite(ProjectileType.Explosive, (int)rect.Width, (int)rect.Height);
        }

        public override int Damage => 50; // Increased damage for explosive projectile

        public override void OnHit(Player target, Game game)
        {
            // Deal damage to the target
            target.TakeDamage(Damage);
            
            // Create area of effect damage to all nearby objects
            foreach (var obj in game.GetGameObjects())
            {
                if (obj is Player otherPlayer && otherPlayer != target && otherPlayer != Owner)
                {
                    float distance = Vector2.Distance(
                        new Vector2(Rect.X + Rect.Width / 2, Rect.Y + Rect.Height / 2),
                        new Vector2(otherPlayer.Rect.X + otherPlayer.Rect.Width / 2, otherPlayer.Rect.Y + otherPlayer.Rect.Height / 2)
                    );
                    
                    if (distance <= ExplosionRadius)
                    {
                        otherPlayer.TakeDamage(Damage / 2); // Half damage to nearby players
                    }
                }
            }
            
            game.PlayHitSound(); // Could add a special explosion sound here
        }

        public override Color GetColor()
        {
            return Color.Orange; // Distinctive explosive color
        }

        public override void Draw()
        {
            // Draw the projectile sprite
            if (sprite.Id > 0)
            {
                Raylib.DrawTexture(sprite, (int)Rect.X, (int)Rect.Y, Color.White);
                
                // Add a pulsing effect to show it's explosive
                float pulseIntensity = MathF.Sin((float)Raylib.GetTime() * 10) * 0.3f + 0.7f;
                Color pulseColor = new Color((int)(255 * pulseIntensity), (int)(165 * pulseIntensity), 0, 100);
                
                Rectangle pulseRect = new Rectangle(
                    Rect.X - 2, Rect.Y - 2, 
                    Rect.Width + 4, Rect.Height + 4
                );
                
                Raylib.DrawRectangleRec(pulseRect, pulseColor);
            }
            else
            {
                // Fallback to original drawing code
                base.Draw();
            }
        }
    }
}