using GalagaFighter;
using GalagaFighter.Models;
using GalagaFighter.Models.Effects;
using GalagaFighter.Models.Players;
using Raylib_cs;
using System.Numerics;

namespace GalagaFigther.Models.Projectiles
{
    public abstract class Projectile : GameObject
    {
        public Vector2 Speed { get; set;  }
        public Player Owner { get; set; }
        public ProjectileEffect OwnerEffect { get; internal set;  }
        public bool DestroyOnHit { get; protected set; } = true;
        public bool BlocksMovement { get; protected set; } = false;
        public abstract int Damage { get; }

        public virtual bool DestroyOnPowerUp => true;

        protected Texture2D sprite;

        protected Projectile(Rectangle rect, Vector2 speed, Player owner, ProjectileEffect ownerEffect) : base(rect)
        {
            Speed = speed;
            Owner = owner;
            IsActive = true;
            OwnerEffect = ownerEffect;
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

        public virtual void OnHit(Player target, Game game)
        {
            target.TakeDamage(Damage);

            var effects = GetEffects(target);   
            foreach(var effect in effects)
                target.Stats.AddEffect(target, effect);

            OwnerEffect.OnHit();

            PlaySound(game);
        }

        public virtual void PlaySound(Game game) =>
            Game.PlayHitSound();

        public abstract Color GetColor();

        public virtual List<PlayerEffect> GetEffects(Player target) {
            return new List<PlayerEffect>();
        }

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
