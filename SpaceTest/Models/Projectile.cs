using Raylib_cs;
using GalagaFighter.Models.Players;
using System.Numerics;

namespace GalagaFighter.Models
{
    public abstract class Projectile : GameObject
    {
        public Vector2 Speed { get; }
        public Player Owner { get; }
        public bool DestroyOnHit { get; protected set; } = true;
        public bool BlocksMovement { get; protected set; } = false;
        public abstract int Damage { get; }

        protected Texture2D sprite;

        protected Projectile(Rectangle rect, Vector2 speed, Player owner) : base(rect)
        {
            Speed = speed;
            Owner = owner;
            IsActive = true;
        }

        public override void Update(Game game)
        {
            Rect.X += Speed.X;
            Rect.Y += Speed.Y;
            if (Rect.X < -Rect.Width || Rect.X > Raylib.GetScreenWidth())
            {
                IsActive = false;
            }
        }

        public abstract void OnHit(Player target, Game game);

        public abstract Color GetColor();

        public override void Draw()
        {
            if (sprite.Id > 0)
            {
                Raylib.DrawTexture(sprite, (int)Rect.X, (int)Rect.Y, Color.White);
            }
            else
            {
                Raylib.DrawRectangleRec(Rect, GetColor());
            }
        }
    }
}
