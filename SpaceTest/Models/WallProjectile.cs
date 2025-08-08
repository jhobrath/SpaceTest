using Raylib_cs;
using GalagaFighter.Models.Players;
using System.Numerics;

namespace GalagaFighter.Models
{
    public class WallProjectile : Projectile
    {
        public bool IsStuck = false;

        public WallProjectile(Rectangle rect, Vector2 speed, Player owner)
            : base(rect, speed, owner)
        {
            sprite = SpriteGenerator.CreateProjectileSprite(ProjectileType.Wall, (int)rect.Width, (int)rect.Height);
            DestroyOnHit = false;
            BlocksMovement = true;
        }

        private float alpha = 1.0f;
        private float lifeTime = 10.0f;
        private const float fadeStartTime = 3.0f;

        public override int Damage => 0;

        public override void Update(Game game)
        {
            // Don't move, just count down lifetime
            lifeTime -= Raylib.GetFrameTime();

            // Start fading after 3 seconds
            if (lifeTime <= fadeStartTime)
            {
                alpha = lifeTime / fadeStartTime;
            }

            // Destroy when lifetime is up
            if (lifeTime <= 0)
            {
                IsActive = false;
            }

            if(!IsStuck)
                Rect.X += Speed.X;
        }

        public override void Draw()
        {
            // Use the generated sprite (which works!)
            if (sprite.Id > 0)
            {
                Raylib.DrawTexture(sprite, (int)Rect.X, (int)Rect.Y, new Color(255, 255, 255, Math.Max(0, (int)(255 * alpha))));
            }
            else
            {
                // Fallback to simple rectangle if sprite fails
                Color wallColor = GetColor();
                wallColor = new Color(Color.Gray.R, Color.Gray.G, Color.Gray.B, (byte)(255 * alpha));
                Raylib.DrawRectangleRec(Rect, wallColor);
            }
        }

        public override void OnHit(Player target, Game game)
        {
            if(!IsStuck)
                game.PlayWallStickSound();

            IsStuck = true;
        }

        public override Color GetColor()
        {
            return Color.Gray;
        }
    }
}