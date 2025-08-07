using Raylib_cs;

namespace GalagaFighter.Models
{
    public abstract class Projectile : GameObject
    {
        public Player Owner { get; protected set; }
        protected readonly float speed;
        protected Texture2D sprite;

        protected Projectile(Rectangle rect, float speed, Player owner) : base(rect)
        {
            this.speed = speed;
            Owner = owner;
        }

        // Virtual methods that can be overridden by specific projectile types
        public virtual void OnHit(Player target, Game game) { }
        public virtual bool BlocksMovement => false;
        public virtual int Damage => 10;
        public virtual bool DestroyOnHit => true;
        
        public override void Update(Game game)
        {
            UpdateMovement(game);
            CheckBounds(game);
        }

        protected virtual void UpdateMovement(Game game)
        {
            Rect.X += speed;
        }

        protected virtual void CheckBounds(Game game)
        {
            int screenWidth = Raylib.GetScreenWidth();
            if (Rect.X > screenWidth || Rect.X < 0)
            {
                IsActive = false;
            }
        }

        public override void Draw()
        {
            if (sprite.Id > 0) // Check if sprite is valid
            {
                Raylib.DrawTexture(sprite, (int)Rect.X, (int)Rect.Y, Color.White);
            }
            else
            {
                // Fallback to color rectangle if sprite failed to load
                Raylib.DrawRectangleRec(Rect, GetColor());
            }
        }

        public abstract Color GetColor();
    }
}
