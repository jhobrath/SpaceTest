using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects.Statuses
{
    public class HealthTakeDamageEffect : StatusEffect
    {
        private ParticleEffect _effect;

        protected override float Duration => .125f;
        public override string IconPath => "Sprites/effects/statuses/Healthtakedamage.png";

        public HealthTakeDamageEffect(Vector2 point)
        {
            _effect = ParticleEffectsLibrary.Get(ParticleEffectLibraryKeys.Explosion);
            _effect.ParticleSpeed = Vector2.Zero;
            _effect.ParticleSpeedVariation = Vector2.One * 400f;
            _effect.ParticleStartSize = 5f;
            _effect.ParticleEndSize = 1f;
            _effect.ParticleStartColor = Raylib_cs.Color.Orange.ApplyAlpha(.9f + (float)Game.Random.NextDouble() / 10f);
            _effect.ParticleEndColor = Raylib_cs.Color.Gray;
            _effect.ParticleLifetime = .25f;
            _effect.Offset = -Vector2.One * _effect.ParticleStartSize / 2;
            _effect.Offset += point;
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Display.RedAlpha *= .5f;
            modifiers.ParticleEffects.Add(_effect);
        }
    }
}
