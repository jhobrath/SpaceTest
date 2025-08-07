using Raylib_cs;

namespace GalagaFighter.Models
{
    public class WallProjectile : Projectile
    {
        private bool isStuck = false;
        private float stuckTimer = 10.0f;

        public WallProjectile(Rectangle rect, float speed, Player owner) 
            : base(rect, speed, owner) 
        {
            sprite = SpriteGenerator.CreateProjectileSprite(ProjectileType.Wall, (int)rect.Width, (int)rect.Height);
        }

        public override bool BlocksMovement => isStuck;
        public override bool DestroyOnHit => false; // Walls don't get destroyed when touched
        public override int Damage => 0; // Walls don't do damage

        protected override void UpdateMovement(Game game)
        {
            if (!isStuck)
            {
                Rect.X += speed;
                
                int screenWidth = Raylib.GetScreenWidth();
                
                // Check if wall has reached the edge
                if (Rect.X <= 0 || Rect.X >= screenWidth - Rect.Width)
                {
                    isStuck = true;
                    // Snap to edge
                    Rect.X = (Rect.X <= 0) ? 0 : screenWidth - Rect.Width;
                    game.PlayWallStickSound();
                }
            }
            else
            {
                stuckTimer -= Raylib.GetFrameTime();
                if (stuckTimer <= 0)
                {
                    IsActive = false;
                }
            }
        }

        protected override void CheckBounds(Game game)
        {
            // Wall projectiles don't get destroyed by normal bounds checking
            // They handle their own lifecycle in UpdateMovement
        }

        public override void OnHit(Player target, Game game)
        {
            // Walls don't have hit effects, they just block movement
        }

        public override Color GetColor()
        {
            return Color.Brown;
        }
    }
}