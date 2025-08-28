using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects.Projectiles
{
    public class NinjaShotEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/ninjashot.png";
        public override bool IsProjectile => true;
        private readonly SpriteWrapper _sprite;
        protected override float Duration => 10f;

        public NinjaShotEffect()
        {
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Players/NinjaShotShip.png"), 3, .12f);
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Sprite = _sprite;
            modifiers.Projectile.OnShootProjectiles.Add(CreateProjectile);
            modifiers.Stats.FireRateMultiplier *= .45f;
            modifiers.Projectile.RotationOffset += 10f;
            modifiers.Projectile.VerticalPositionIncrement = -150 + 300f * (float)Game.Random.NextDouble();
            modifiers.Projectile.OnClone = (projMods) => projMods.VerticalPositionIncrement = -150 + 300f * (float)Game.Random.NextDouble();
            //modifiers.Projectile.VerticalPositionMultiplier = 1.5f;
        }

        private Projectile CreateProjectile(IProjectileController controller, Player owner, Vector2 position, PlayerProjectile modifiers)
            => new NinjaProjectile(controller, owner, position, modifiers);
    }
}
