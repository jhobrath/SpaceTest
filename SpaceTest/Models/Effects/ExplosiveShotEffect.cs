using GalagaFighter.Models.Players;
using GalagaFigther;
using GalagaFigther.Models.Projectiles;
using Raylib_cs;
using SpaceTest.Models.Projectiles;
using System.Numerics;

namespace GalagaFighter.Models.Effects
{
    public class ExplosiveShotEffect : ProjectileEffect
    {
        protected override float Duration => 10.0f;

        private readonly SpriteWrapper _spriteWrapper;

        public override float FireRateMultiplier => 2f;

        public ExplosiveShotEffect(Player player) : base(player)
        {
            _spriteWrapper = new SpriteWrapper(TextureLibrary.Get("Sprites/Players/ExplosiveShotShip.png"), 3, .12f);
        }


        protected override int ProjectileWidth => 40;
        protected override int ProjectileHeight => 40;
        protected override Projectile Spawn(Rectangle rect, Vector2 speed) => new ExplosiveProjectile(rect, speed, Player, this);
        
        protected override SpriteWrapper Texture => _spriteWrapper;
        public override int? TextureFrame => _spriteWrapper.CurrentFrame;

        public override void OnUpdate(float frameTime)
        {
            _spriteWrapper.Update(frameTime);
            base.OnUpdate(frameTime);
        }

        protected override Vector2 SpawnOffset => new Vector2(-50, 15);

        public override string IconPath => "Sprites/Effects/explosiveshot.png";
    }
}