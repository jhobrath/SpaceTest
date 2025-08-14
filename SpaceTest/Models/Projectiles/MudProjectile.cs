using GalagaFighter;
using GalagaFighter.Models;
using GalagaFighter.Models.Effects;
using GalagaFighter.Models.Players;
using GalagaFigther;
using GalagaFigther.Models.Projectiles;
using Raylib_cs;
using System.Numerics;

namespace SpaceTest.Models.Projectiles
{
    public class MudProjectile : Projectile
    {
        private readonly SpriteWrapper spriteWrapper;
        private bool _isPlayer1 = false;
        private static Random _random = new Random();
        private bool _frameHasReached5 = false;

        public MudProjectile(Rectangle rect, Vector2 speed, Player owner, ProjectileEffect ownerEffect) 
            : base(rect, speed, owner, ownerEffect) 
        {
            // Use ninja.png as an animated sprite (3 frames, 0.12s per frame)
            var texture = TextureLibrary.Get("Sprites/Projectiles/mud.png");
            spriteWrapper = new SpriteWrapper(texture, 6, 0.12f);

            _isPlayer1 = owner.IsPlayer1;
        }

        public override List<PlayerEffect> GetEffects(Player target) => new List<PlayerEffect>
        {
            new MudSplatEffect(target, new Vector2(Rect.X + Rect.Width / 2f, Rect.Y + Rect.Height / 2f))
        };

        public override int Damage => 0;

        public override Color GetColor()
        {
            return Color.White;
        }

        public override void Update(Game game)
        {
            if (Rect.X < -Rect.Width || Rect.X > Raylib.GetScreenWidth())
            { 
                IsActive = false;
            }

            spriteWrapper.Update(Raylib.GetFrameTime());

            base.Update(game);
        }

        public override void Draw()
        {
            if(_frameHasReached5)
            {
                if (spriteWrapper.CurrentFrame < 4)
                    spriteWrapper.CurrentFrame = 4;
            }
            else
            {
                _frameHasReached5 = spriteWrapper.CurrentFrame >= 5;
            }

                // Draw animated ninja sprite
                spriteWrapper.DrawAnimated(
                    new Vector2(Rect.X + Rect.Width / 2f, Rect.Y + Rect.Height / 2f),
                    _isPlayer1 ? 0f : 180f,
                    Rect.Width,
                    Rect.Height
                );
        }

        public override void PlaySound(Game game)
        {
            Game.PlayMudSplat();
        }
    }
}