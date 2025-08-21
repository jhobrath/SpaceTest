using GalagaFighter.Core.Behaviors.Players.Interfaces;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects
{
    public class WoodShotEffect : PlayerEffect
    {
        private readonly SpriteWrapper _sprite;
        public override bool IsProjectile => true;
        public override string IconPath => "Sprites/Effects/woodshot.png";
        protected override float Duration => 0f; // Set to >0 if you want a time limit

        private int _remainingBullets = 3;

        public WoodShotEffect()
        {
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Players/WoodShotShip.png"));
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Sprite = _sprite;
            modifiers.Projectile.OnProjectileDestroyed = HandleWoodShotFired;
            modifiers.Projectile.Projectiles.Add(CreateProjectile);
        }

        private Projectile CreateProjectile(IProjectileUpdater projectileUpdater, Player owner, Vector2 position)
            => new WoodProjectile(projectileUpdater, owner, position);

        private void HandleWoodShotFired(Projectile projectile)
        {
            _remainingBullets--;
            if (_remainingBullets == 0)
                Deactivate();
        }
    }
}
