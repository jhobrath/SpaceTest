using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.Projectiles
{
    public class ShotGunShellProjectile : Projectile
    {
        private static Vector2 _baseSpeed => new(.0000001f, 0f);
        private static Vector2 _baseSize => new(600f, 100f);

        public override float Damage => 10f;
        public override bool IsTransformChild => false;
        public override List<ParticleEffectConfig> EmitterConfigurations => [_emitterConfig];

        private readonly ParticleEffectConfig _emitterConfig = ParticleEffectTemplates.Get("SmokePoof");

        public ShotGunShellProjectile(Guid owner)
            : base(owner, Vector2.Zero, _baseSize, _baseSpeed, GetSprite())
        {
            Lifetime = .3f;
            Bounds = [
                new(0f,.5f),
                new(1f,0f),
                new(1f,1f)
            ];
            _emitterConfig.EmitWithinParentBounds = true;
            _emitterConfig.StartColor = Palette;
        }

        private static SpriteBase GetSprite()
        {
            return new NonRepeatingAnimatedDrawnSprite(new(600f,100f), 7, .31f/7f, ShotGunShellProjectileSpriteGenerator.CreateAnimatedShotGunShellProjectile);
        }

        public override List<PlayerEffect> CreateEffects(Player player) => [];
    }
}
