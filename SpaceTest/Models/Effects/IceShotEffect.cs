using GalagaFighter.Models.Players;
using GalagaFigther;
using GalagaFigther.Models.Projectiles;
using Raylib_cs;
using SpaceTest.Models.Projectiles;
using System.Numerics;

namespace GalagaFighter.Models.Effects
{
    public class IceShotEffect : ProjectileEffect
    {
        protected override float Duration => 10.0f;
        public override string IconPath => "Sprites/Effects/iceshot.png";

        private SpriteWrapper _spriteWrapper;
        public IceShotEffect(Player player) : base(player)
        {
            _spriteWrapper = new SpriteWrapper(TextureLibrary.Get("Sprites/Players/IceShotShip.png"), 3, .12f);
        }

        public override void OnUpdate(float frameTime)
        {
            _spriteWrapper.Update(frameTime);
            base.OnUpdate(frameTime);
        }

        protected override int ProjectileWidth => 95;
        protected override int ProjectileHeight => 42;
        protected override float? OnHitMaxRemainingTime => 2;
        protected override SpriteWrapper Texture => _spriteWrapper;
        public override int? TextureFrame => _spriteWrapper.CurrentFrame;
        protected override Vector2 SpawnOffset => new Vector2(-50, 0);

        protected override Projectile Spawn(Rectangle rect, Vector2 speed) => new IceProjectile(rect, speed, Player, this);

    }
}