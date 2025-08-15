using GalagaFighter;
using GalagaFighter.Models;
using GalagaFighter.Models.Effects;
using GalagaFighter.Models.Players;
using GalagaFighter.Models.PowerUps;
using GalagaFigther;
using GalagaFigther.Models.Projectiles;
using Raylib_cs;
using System.Numerics;

namespace SpaceTest.Models.Projectiles
{
    public class IceProjectile : Projectile
    {
        private SpriteWrapper _spriteWrapper;
        private bool _isPlayer1 = false;

        private readonly SpriteWrapper _collision;

        public override SpriteWrapper Collision => _collision;

        public IceProjectile(Rectangle rect, Vector2 speed, Player owner, ProjectileEffect ownerEffect) 
            : base(rect, speed, owner, ownerEffect) 
        {
            // Use ninja.png as an animated sprite (3 frames, 0.12s per frame)
            var texture = TextureLibrary.Get("Sprites/Projectiles/ice.png");
            _spriteWrapper = new SpriteWrapper(texture, 6, .33f);
            _collision = new SpriteWrapper(TextureLibrary.Get("Sprites/Effects/iceshot-collision.png"), 5, .12f);
            _isPlayer1 = owner.IsPlayer1;

            //sprite = SpriteGenerator.CreateProjectileSprite(ProjectileType.Ice, (int)rect.Width, (int)rect.Height);
        }

        public override int Damage => 0; // Ice projectiles don't do damage

        public override List<PlayerEffect> GetEffects(Player target)
        {
            return new List<PlayerEffect> { new FrozenEffect(target) };
        } 
        
        public override void PlaySound(Game game)
        {
            Game.PlayIceHitSound();
        }

        public override Color GetColor()
        {
            return Color.SkyBlue;
        }

        public override void Draw()
        {
            _spriteWrapper.DrawFromTopLeft(Rect.Position, Owner.IsPlayer1 ? 0f : 180f, Rect.Width, Rect.Height);
        }

        public override void Update(Game game)
        {
            _spriteWrapper.Update(Raylib.GetFrameTime());

            base.Update(game);
        }
    }
}