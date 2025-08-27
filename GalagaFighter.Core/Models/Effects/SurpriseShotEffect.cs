using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects
{
    public class SurpriseShotEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/surprise.png";
        public override bool IsProjectile => false;
        protected override float Duration => 10f;

        private float _lifetime = 0f;

        private EffectModifiers? _modifiers;

        public SurpriseShotEffect()
        {
        }

        public override void Apply(EffectModifiers modifiers)
        {
            _modifiers = modifiers;
            base.Apply(modifiers);
        }

        public override void OnUpdate(float frameTime)
        {
            if (_modifiers != null)
                if(Game.Random.NextDouble() > .992f)
                    _modifiers.Projectile.OneTimeProjectiles.Add(CreateInstantaneousBullet);

            base.OnUpdate(frameTime);
        }

        private Projectile CreateInstantaneousBullet(IProjectileController updater, Player owner, Vector2 position, PlayerProjectile modifiers)
        {
            var proj = new DefaultProjectile(updater, owner, position, modifiers, owner.PalleteSwap);
             proj.Move(y:-proj.Rect.Height / 2);
            proj.Hurry(x: 5f);
            return proj;
        }
    }
}
