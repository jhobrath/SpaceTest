using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects.Statuses
{
    public class RadiationEffect : PlayerEffect
    {
        protected override float Duration => 5f;

        private readonly ParticleEffect _effect;
        private readonly SpriteDecoration _glowDecoration;
        private readonly SpriteDecoration _radioactiveDecoration;
        public override string IconPath => "Sprites/Effects/Status/radiation.png";

        private float _lifeTime = 0f;

        public RadiationEffect()
        {
            _effect = new ParticleEffect(ParticleEffectLibraryKeys.MagicalSparkles);
            _effect.ParticleStartSize = 4f;
            _effect.ParticleEndSize = 4f;
            _effect.EmissionRadius = 200f;
            _effect.ParticleSpeedVariation = new System.Numerics.Vector2(1000, 1000);
            _effect.GravityStrength = 0f;
            _effect.MaxParticles = 100;
            _effect.ParticleEndColor = _effect.ParticleStartColor.ApplyAlpha(0f);
            _effect.ParticleLifetime = .125f;
            _effect.Loop = true;

            _glowDecoration = new SpriteDecoration(new SpriteWrapper("Sprites/Ships/MainShipGlow.png"));
            _glowDecoration.Sprite.Color = Color.Green;

            _radioactiveDecoration = new SpriteDecoration(new SpriteWrapper("Sprites/Ships/MainShipBody_Radioactive.png"))
            {
                Size = new System.Numerics.Vector2(160, 160)
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.ParticleEffects.Add(_effect);
            modifiers.Jiggle += 2f;
            modifiers.Decorations!.Glow = _glowDecoration;
            modifiers.Decorations["Radioactive"] = _radioactiveDecoration;

            modifiers.Stats.Shield *= .9f;
        }

        public override void OnUpdate(float frameTime)
        {
            _lifeTime += frameTime;
            var opacity = Math.Abs(1 - 2 * (_lifeTime % 1f));
            _glowDecoration.Sprite.Color = Color.Green.ApplyAlpha(opacity);

            _radioactiveDecoration.Sprite.Color = Color.White.ApplyAlpha(1-opacity);

            base.OnUpdate(frameTime);
        }
    }
}
