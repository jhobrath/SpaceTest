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
using Raylib_cs;
using GalagaFighter.Core2.Handlers.Projectiles;

namespace GalagaFighter.Core2.GameObjects.Projectiles
{
    public class ShotGunShellProjectile : Projectile
    {
        private static Vector2 _baseSpeed => new(.0000001f, 0f);
        private static Vector2 _baseSize => new(850f, 120f);

        public override List<ParticleEffectConfig> EmitterConfigurations => [_emitterConfig];

        private readonly ParticleEffectConfig _emitterConfig = ParticleEffectTemplates.Get("SmokePoof");

        public ShotGunShellProjectile(Guid owner)
            : base(owner, Vector2.Zero, _baseSize, _baseSpeed, GetSprite())
        {
            Lifetime = .8f;
            Bounds = [
                new(0f,.5f),
                new(1f,0f),
                new(1f,1f)
            ];

            Damage = 10f;
            IsTransformChild = true;
            DestroyProjectiles = true;
            DestroyOnHit = false;

            _emitterConfig.EmitWithinParentBounds = true;
            _emitterConfig.StartColor = Palette;
            _emitterConfig.EndColor = Palette.ApplyAlpha(.15f);
            _emitterConfig.StartColor = Color.Orange;
            _emitterConfig.EndColor = Color.Orange.AdjustLightness(.5f);
            _emitterConfig.StartSize = 5f;
            _emitterConfig.EndSize = 1f;
            _emitterConfig.Textures = ["dot_1", "dot_2", "star_3", "star_4"];
            _emitterConfig.EmissionRate = 1000f;
            _emitterConfig.SizeVariation = 0f;
            _emitterConfig.Lifetime = .5f;
            _emitterConfig.LifetimeVariation = .1f;
            _emitterConfig.Speed = new(0, -1000);
            _emitterConfig.SpeedVariation = new(0, 0);
            _emitterConfig.Drag = 1700f;
            _emitterConfig.HasVerticalInertia = true;

            Behaviors.Add(typeof(LifetimeDeactivationBehavior));

            Color = Color.White.ApplyAlpha(.5f);
        }

        private static SpriteBase GetSprite()
        {
            return new NonRepeatingAnimatedDrawnSprite(new(600f,100f), 7, .8f/7, ShotGunShellProjectileSpriteGenerator.CreateAnimatedShotGunShellProjectile);
        }

        public override List<PlayerEffect> CreateEffects(Player player) => [];
    }
}
