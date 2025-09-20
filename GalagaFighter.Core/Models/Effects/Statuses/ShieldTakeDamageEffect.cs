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
    public class ShieldTakeDamageEffect : StatusEffect
    {
        protected override float Duration => .25f;
        public override string IconPath => "Sprites/effects/statuses/takedamage.png";

        public override void Apply(EffectModifiers modifiers)
        {
            if (modifiers.Decorations?.Shield == null)
                return;

            modifiers.Decorations.Shield.Sprite.Color = modifiers.Decorations.Shield.Sprite.Color.ApplyRed(.5f);


            var effect = ParticleEffectsLibrary.Get(ParticleEffectLibraryKeys.Explosion);
            effect.ParticleSpeed = Vector2.Zero;
            effect.ParticleSpeedVariation = Vector2.One*400f;
            effect.ParticleStartSize = 5f;
            effect.ParticleEndSize = 1f;
            effect.ParticleStartColor = Raylib_cs.Color.Orange.ApplyAlpha(.9f + (float)Game.Random.NextDouble()/10f);
            effect.ParticleEndColor = Raylib_cs.Color.Gray;
            effect.ParticleLifetime = .25f;

            effect.Offset = -Vector2.One * effect.ParticleStartSize / 2;

            modifiers.ParticleEffects.Add(effect);
        }
    }
}
