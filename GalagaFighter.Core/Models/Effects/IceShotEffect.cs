using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects
{
    public class IceShotEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/iceshot.png";
        public override bool IsProjectile => true;
        private readonly SpriteWrapper _sprite;
        protected override float Duration => 10f; 

        public IceShotEffect()
        {
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Players/IceShotShip.png"), 3,  .33f);
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Sprite = _sprite;
            modifiers.Projectile.Projectiles.Add(CreateProjectile);
        }

        private Projectile CreateProjectile(IProjectileUpdater updater, Player owner, Vector2 position, PlayerProjectile modifiers)
            => new IceProjectile(updater, owner, position, modifiers);
    }
}
